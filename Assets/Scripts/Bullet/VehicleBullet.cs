using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// This is a specific bulelt for vehicles that can explode ant affects the other players/vehicles 
/// </summary>
public class VehicleBullet : MonoBehaviour
{
    // Start is called before the first frame update
    //[Header("When ready=true, it is prepared to explode")]

    [Header("Parameters")]
    public float maxDist = 50;
    public float maxDamage = 200;
    public float speed =30;
    Rigidbody rb;


    [Header("The particles systems")]
    public ParticleSystem partcl;
    public Player playerORigin;

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
        _col=GetComponent<Collider>();

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
    }


    private void OnTriggerEnter(Collider other)
    {
        if(((other.gameObject.tag=="ground"|| other.gameObject.tag=="environment" 
            || other.gameObject.tag=="drone"
            || other.gameObject.name == "tank" || other.gameObject.tag == "tank"
            || other.gameObject.name == "plane" || other.gameObject.tag == "plane"
            ) && elapsed>Time.fixedDeltaTime*1.01f)
        )
        {
            Debug.Log("Vehicle bullet collision-->" + other.gameObject.name);
            StartCoroutine(Explode());
            
        }


    }

    /// <summary>
    /// This part allows the bullet to find all the obects in range and performs the hits
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

                int damage = (int)Mathf.Clamp(maxDamage*(1-distanceToImpact/maxDist),0,maxDamage);

                if (damage > 0)
                {
                    Player PY = playerAvatars[ii].transform.root.GetComponent<PhotonView>().Owner;

                    if ((int)PY.CustomProperties["health"] > 0 
                        && (int)PY.CustomProperties["team"]!= (int)playerORigin.CustomProperties["team"])
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
                    Player PY = playerORigin;

                    drones[ii].gameObject.transform.root.GetComponent<DroneHealth>().getHit(damage);
                    drones[ii].gameObject.transform.root.GetComponent<DroneHealth>().lastHitPlayer = PY;

                    Debug.Log("BulletTank hit: " + drones[ii].gameObject.name+ "  damage-->"+ damage);
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
