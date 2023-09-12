using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// this is used to move randomly the drone inside its tansform parent and shoot to closes player
/// </summary>
public class DroneAttack : MonoBehaviour
{
    [Header("If the player has spawn it or it is an enemy")]
    public bool isFromPlayer = false;

    [Header("Relative movement")]
    public float sinSpeed = 25.6f;
    public float sinAmplitude = 0.2f;
    public Vector3 objective;
    public float yInitial;
    public float RadiusX = 1.2f;

    [Header("Points where bullets appear")]
    public Transform[] barrels;

    float elapsed, elapsedShoot;
    float randomTime;

    [Header("Random time param for shooting")]
    public float minTime = 1;
    public float maxTime = 5;
    public float shootingTime = 0.4f;
    public float shootingDistance = 2.5f;


    DroneMovement droneMov;
    DroneHealth droneHealth;


    float randomInitPos;
        
    bool dead = false;
    PhotonView PV;

    // Start is called before the first frame update
    void Awake()
    {
        elapsed = 0;
        elapsedShoot = 0;

        randomInitPos = Random.Range(0, 5);
        randomTime = Random.Range(minTime, maxTime);

        droneHealth = transform.root.GetComponent<DroneHealth>();
        droneMov = transform.root.GetComponent<DroneMovement>();

        PV = GetComponent<PhotonView>();

        objective = + yInitial * new Vector3(0, 1, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ( (PV.IsMine && isFromPlayer) || (PhotonNetwork.IsMasterClient && !isFromPlayer))
        {
            // get the dead condition to simulate that the drone falls to the ground
            if (droneHealth.dead == true && dead == false)
            {
                PV.RPC("RCP_AddRb", RpcTarget.AllBuffered);                              
                dead = true;
            }

            elapsed += Time.fixedDeltaTime;

            // move relative to its parent using random param
            if (elapsed > randomTime)
            {
                objective = yInitial * new Vector3(0, 1, 0) + new Vector3(Random.Range(-RadiusX, RadiusX), 0, 0);

                elapsed = 0;
                randomTime = Random.Range(minTime, maxTime);
            }

            elapsedShoot += Time.fixedDeltaTime;

            transform.localPosition = Vector3.Lerp(transform.localPosition, objective + sinAmplitude * Mathf.Sin(sinSpeed * Time.fixedTime + randomInitPos) * Vector3.up, 0.1f);


            // perform shootig if the objective exista and the PV is mine
            if (droneMov.objectiveShooting != null && PV.IsMine)
            {
                // check distance
                if (elapsedShoot > shootingTime && droneHealth.dead == false
                    && (transform.position - droneMov.objectiveShooting.transform.position).magnitude < shootingDistance
                    && droneMov.objectiveShooting.transform.root.GetComponent<PlayerHealth>().dead==false)
                {
                    //perform shoiting in the different barrel points
                    for (int ii = 0; ii < barrels.Length; ii++)
                    {
                        if (!isFromPlayer)
                        {
                            barrels[ii].right = -(droneMov.objectiveShooting.transform.position + Vector3.up * 0.8f - barrels[ii].position);
                        }
                        else
                        {
                            barrels[ii].forward = (droneMov.objectiveShooting.transform.position + Vector3.up * 0.8f - barrels[ii].position);
                        }

                        ShootingManager.SM.ShootEnemy(barrels[ii].transform.GetChild(0).position, barrels[ii].transform.GetChild(0).rotation, null);
                    }

                    elapsedShoot = 0;

                }

            }
        }


    }

    [PunRPC]
    public void RCP_AddRb()
    {
        GetComponent<Collider>().isTrigger = false;
        gameObject.AddComponent<Rigidbody>();
        this.enabled = false;
    }

}
