using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// this special bullet followst he objective and then explodes.
/// </summary>
public class Rocket : MonoBehaviour
{
    [Header("Parameters")]
    public float maxDist = 5;
    public int maxDamage;
    public float speed;
    public Player playerOrigin;
    public int objetivePVindex;
    public Transform lerpObjective;
    public float lerpSpeed = 0.05f;

    Rigidbody rb;

    [Header("The particles systems")]
    public ParticleSystem partcl;

    [Header("Shotting effect")]
    //shooting effect
    public Renderer shootHalo;
    public Renderer[] mshR;
    public float haloTime = 0.08f;
    public float travellingTime;
    float elapsed;


    PhotonView PV;
    Collider _col;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        _col = GetComponent<Collider>();

        //initialize the variables
        rb = transform.GetComponent<Rigidbody>();

        //move the bullet
        rb.velocity = transform.forward * speed;
        
        //enable AND call disable effect of the halo
        shootHalo.enabled = true;
        Invoke("disableEffects", haloTime);
    }

    public void disableEffects()
    {
        //disable the halo effect
        shootHalo.enabled = false;
    }

    private void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;

        //the lerp object is used to smooth the following movement
        lerpObjective.forward=(PhotonView.Find(objetivePVindex).transform.position- lerpObjective.position);

        rb.transform.forward = Vector3.Lerp(rb.transform.forward, lerpObjective.forward, lerpSpeed);
        
        //move the bullet
        rb.velocity = transform.forward * speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (((other.gameObject.tag == "ground" || other.gameObject.tag == "environment"
            || other.gameObject.tag == "drone"
            || other.gameObject.tag == "tank"
            || other.gameObject.tag == "plane"
            ) && elapsed > Time.fixedDeltaTime * 1.01f)
        )
        {

            StartCoroutine(Explode());

        }


    }

    /// <summary>
    /// in this case, it educes the health of other players or vehicles
    /// </summary>
    /// <returns></returns>
    public IEnumerator Explode()
    {
        ColisionBullet();

        _col.enabled = false;
        //PV.RPC("RPC_Explode", RpcTarget.All);

        //hit the players, avatars in range
        GameObject[] playerAvatars = GameObject.FindGameObjectsWithTag("Avatar");

        yield return null;

        //if the search is not null
        if (playerAvatars != null)
        {
            for (int ii = 0; ii < playerAvatars.Length; ii++)
            {
                float distanceToImpact = (playerAvatars[ii].transform.position - transform.position).magnitude;

                int damage = (int)Mathf.Clamp(maxDamage * (1 - distanceToImpact / maxDist), 0, maxDamage);

                if (damage > 0)
                {
                    Player PY = playerAvatars[ii].transform.root.GetComponent<PhotonView>().Owner;


                    //error check
                    if (PY == null)
                        Debug.LogError("PY null");

                    if (playerOrigin == null)
                        Debug.LogError("Player origin null");


                    if ((int)PY.CustomProperties["health"] > 0
                        && (int)PY.CustomProperties["team"] != (int)playerOrigin.CustomProperties["team"])
                    {
                        PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                                    (int)PY.CustomProperties["deaths"],
                                    (int)PY.CustomProperties["score"],
                                    (int)PY.CustomProperties["health"] - damage,
                                    (float)PY.CustomProperties["height"],
                                    (int)PY.CustomProperties["skin"],
                                    (int)PY.CustomProperties["team"],
                                    (string)PY.CustomProperties["Gmode"],
                                    (int)PY.CustomProperties["mesh"]
                                    );
                    }
                }
                yield return null;

                //playerAvatars[ii].transform.root.GetComponent<PlayerHealth>().SetLasPlayerHit(playerORigin);


            }

        }



        //hit the tanks, avatars in range
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("tank");

        //if the search is not null
        if (tanks != null)
        {
            for (int ii = 0; ii < tanks.Length; ii++)
            {
                float distanceToImpact = (tanks[ii].transform.position - transform.position).magnitude;

                int damage = (int)Mathf.Clamp(maxDamage * (1 - distanceToImpact / maxDist), 0, maxDamage);

                
                Player PY = tanks[ii].GetComponent<PhotonView>().Owner;

                tanks[ii].transform.root.GetComponent<Tank>().GetHit(damage);


            }

        }


        //hit the planes, avatars in range
        GameObject[] planes = GameObject.FindGameObjectsWithTag("plane");

        //if the search is not null
        if (planes != null)
        {
            for (int ii = 0; ii < planes.Length; ii++)
            {
                float distanceToImpact = (planes[ii].transform.position - transform.position).magnitude;

                int damage = (int)Mathf.Clamp(maxDamage * (1 - distanceToImpact / maxDist), 0, maxDamage);


                Player PY = planes[ii].GetComponent<PhotonView>().Owner;

                planes[ii].transform.root.GetComponent<Plane>().GetHit(damage);


            }

        }






        //hit the drones, avatars in range
        GameObject[] drones = GameObject.FindGameObjectsWithTag("drone");
        //if the search is not null
        if (drones != null)
        {
            for (int ii = 0; ii < drones.Length; ii++)
            {
                float distanceToImpact = (drones[ii].transform.position - transform.position).magnitude;

                int damage = (int)Mathf.Clamp(maxDamage * (1 - distanceToImpact / maxDist), 0, maxDamage);


                if (damage > 0)
                {
                    Player PY = playerOrigin;

                    drones[ii].gameObject.transform.root.GetComponent<DroneHealth>().getHit(damage);
                    drones[ii].gameObject.transform.root.GetComponent<DroneHealth>().lastHitPlayer = PY;

                    Debug.Log("BulletTank hit: " + drones[ii].gameObject.name + "  damage-->" + damage);
                }





                yield return null;

            }

        }


        Invoke("DestroyBullet", 2);
    }


    public void ColisionBullet()
    {
        //stop the bullet
        rb.velocity = new Vector3(0, 0, 0);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        _col.enabled = false;
        //set the mesh renderer to false
        mshR[0].enabled = false;

        //play the hit particle
        partcl.Play();

    }

    public void DestroyBullet()
    {
        //PhotonNetwork.Destroy(gameObject);
        Destroy(gameObject);
    }
}
