using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    [Header("Bullet attributes")]
    //bullet speed
    public float speed=10.5f;
    //rigidbody of the bullet
    Rigidbody rb;
    //particle
    public ParticleSystem partcl, partclEnv;
    //mesh renderer of the bullet
    Renderer mshR;
    Collider col;
    public float destroyTime = 1.1f;
    public string playerName;
    public float range;
    public float anglePrecission = 0;

    [Space(5)]
    [Header("Damage to for head and body")]
    public int damageBody, damageHead;

    [Space(5)]
    [Header("Shotting effect")]
    //shooting effect
    public Renderer shootHalo;
    public Renderer[] meshRenderOfBullet;
    public float haloTime = 0.08f;
    public float travellingTime;
    float elapsed;

    [Space(5)]
    [Header("The player who has created the bullet")]

    //this is the name of the player that has sent the bullet
    public Player playerORigin;
    
    #region UNITY FUNCTIONS

   
    private void Awake()
    {
        //precission
        transform.localRotation = Quaternion.Euler(Random.Range(-anglePrecission,anglePrecission), Random.Range(-anglePrecission,anglePrecission), 0);

        //initialize the variables
        rb = transform.GetComponent<Rigidbody>();
        col = transform.GetComponent<Collider>();

        //move the bullet
        rb.velocity = transform.forward * speed;
        mshR = GetComponent<Renderer>();

        if (partcl != null)
        {
            partcl.Stop();
        }

        if (partclEnv != null)
        {
            partclEnv.Stop();
        }
               
        //enable AND call disable effect of the halo
        shootHalo.enabled = true;
        Invoke("disableEffects", haloTime);

        //rotate halo randomly to give more realism
        shootHalo.transform.Rotate(new Vector3(0, 0, 1), Random.Range(0, 360));


        
    }

    private void Start()
    {
        travellingTime = (float)range / (float)speed;
    }

    private void FixedUpdate()
    {

        elapsed += Time.fixedDeltaTime;

        if(playerORigin!=null)
        {
            playerName = playerORigin.NickName;
        }

        if(elapsed>travellingTime)
        {
            ColisionBulletEnv();
        }

        /*for (int ii = 0; ii < meshRenderOfBullet.Length; ii++)
        {
            meshRenderOfBullet[ii].enabled = true;
        }*/
    }

    private void OnTriggerEnter(Collider collision)
    {

        if(collision.gameObject.tag == "bullet")
        {
            return;
         
        }

        //check imacted gameobject
        //Debug.Log(collision.gameObject.name);
        string gMode = (string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"];

        //-->IMPACT WITH ENVIRONMENT
        if (collision.gameObject.tag =="environment" || collision.gameObject.tag == "ground")
        {
            //only if the particle system exists, play it
            if (partcl != null)
            {

                ColisionBulletEnv();

            }

            return;
        }
        //<----
        

        else if(collision.gameObject.tag!="Untagged"
                && collision.gameObject.tag != "handLeft"
                && collision.gameObject.tag != "handRight"
                && collision.gameObject.tag != "environment")
        {

           

            //only if the particle system exists, play it
            if (partcl != null)
            {
                ColisionBulletPlayer();

            }

            //drones
            if (collision.gameObject.tag == "drone")
            {
                collision.gameObject.transform.root.GetComponent<DroneHealth>().getHit(damageHead);
                collision.gameObject.transform.root.GetComponent<DroneHealth>().lastHitPlayer = playerORigin;

            }

            //vehicles
            if (collision.gameObject.name == "tank" || collision.gameObject.tag=="tank")
            {
                collision.gameObject.transform.root.GetComponent<Tank>().GetHit(damageBody);
            }

            if (collision.gameObject.name == "plane" || collision.gameObject.tag == "plane")
            {
                collision.gameObject.transform.root.GetComponent<Plane>().GetHit(damageBody);
            }


            //check team mode
            if (gMode== TypeMode.team.ToString())
            {
                Debug.Log(playerORigin.CustomProperties["team"]+" X "+ PlayerInfo.PI.myTeam);
                if(playerORigin!=null)
                {
                    //check the teams when hitting an avatar
                    if (collision.gameObject.tag == "bodyCollider" /*&& (int)playerORigin.CustomProperties["team"] != PlayerInfo.PI.myTeam*/)
                    {
                        PlayerHealth plyHealtScript = collision.gameObject.transform.root.GetComponent<PlayerHealth>();


                        PhotonView PV = collision.gameObject.transform.root.GetComponent<PhotonView>();

                        if (PV.IsMine && PV.Owner != playerORigin)
                        {
                            Player PY = PV.Owner;

                            plyHealtScript.lastPlayerHit = playerORigin;

                            //decrease health of the player
                            PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                                (int)PY.CustomProperties["deaths"],
                                (int)PY.CustomProperties["score"],
                                (int)PY.CustomProperties["health"] - damageBody,
                                (float)PY.CustomProperties["height"],
                                (int)PY.CustomProperties["skin"],
                                (int)PY.CustomProperties["team"],
                                (string)PY.CustomProperties["Gmode"],
                                (int)PY.CustomProperties["mesh"]);


                        }
                        Debug.Log("1");
                    }
                  
                }
                else
                {
                    if (collision.gameObject.tag == "bodyCollider")
                    {
                        PlayerHealth plyHealtScript = collision.gameObject.transform.root.GetComponent<PlayerHealth>();


                        PhotonView PV = collision.gameObject.transform.root.GetComponent<PhotonView>();

                        if (PV.IsMine && PV.Owner != playerORigin)
                        {
                            Player PY = PV.Owner;

                            plyHealtScript.lastPlayerHit = playerORigin;

                            //decrease health of the player
                            PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                                (int)PY.CustomProperties["deaths"],
                                (int)PY.CustomProperties["score"],
                                (int)PY.CustomProperties["health"] - damageBody,
                                (float)PY.CustomProperties["height"],
                                (int)PY.CustomProperties["skin"],
                                (int)PY.CustomProperties["team"],
                                (string)PY.CustomProperties["Gmode"],
                                (int)PY.CustomProperties["mesh"]);


                        }
                        Debug.Log("2");
                    }
                }

                //check the teams when hitting a head
                if (collision.gameObject.tag == "head" /*&& (int)playerORigin.CustomProperties["team"] != PlayerInfo.PI.myTeam*/)
                {
                    PlayerHealth plyHealtScript = collision.gameObject.transform.root.GetComponent<PlayerHealth>();

                    PhotonView PV = collision.gameObject.transform.root.GetComponent<PhotonView>();

                    if (PV.IsMine && PV.Owner != playerORigin)
                    {
                        Player PY = PV.Owner;
                        plyHealtScript.lastPlayerHit = playerORigin;

                        //decrease health of the player
                        PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                        (int)PY.CustomProperties["deaths"],
                        (int)PY.CustomProperties["score"],
                        (int)PY.CustomProperties["health"] - damageHead,
                        (float)PY.CustomProperties["height"],
                        (int)PY.CustomProperties["skin"],
                        (int)PY.CustomProperties["team"],
                        (string)PY.CustomProperties["Gmode"],
                        (int)PY.CustomProperties["mesh"]);


                    }
                    Debug.Log("3");
                }

            }




            //check other modes
            if (gMode != TypeMode.team.ToString())
            {
                //check the teams when hitting an avatar
                if (collision.gameObject.tag == "bodyCollider")
                {
                    PlayerHealth plyHealtScript = collision.gameObject.transform.root.GetComponent<PlayerHealth>();

                    PhotonView PV = collision.gameObject.transform.root.GetComponent<PhotonView>();

                    if (PV.IsMine && (PV.Owner != playerORigin || playerORigin==null))
                    {
                        Player PY = PV.Owner;
                        plyHealtScript.lastPlayerHit = playerORigin;

                        //decrease health of the player
                        PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                        (int)PY.CustomProperties["deaths"],
                        (int)PY.CustomProperties["score"],
                        (int)PY.CustomProperties["health"] - damageBody,
                        (float)PY.CustomProperties["height"],
                        (int)PY.CustomProperties["skin"],
                        (int)PY.CustomProperties["team"],
                        (string)PY.CustomProperties["Gmode"],
                        (int)PY.CustomProperties["mesh"]);


                    }

                }

                //check the teams when hitting a head
                if (collision.gameObject.tag == "head")
                {
                    PlayerHealth plyHealtScript = collision.gameObject.transform.root.GetComponent<PlayerHealth>();

                    PhotonView PV = collision.gameObject.transform.root.GetComponent<PhotonView>();

                    if (PV.IsMine && PV.Owner != playerORigin)
                    {
                        Player PY = PV.Owner;
                        plyHealtScript.lastPlayerHit = playerORigin;

                        //decrease health of the player
                        PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                        (int)PY.CustomProperties["deaths"],
                        (int)PY.CustomProperties["score"],
                        (int)PY.CustomProperties["health"] - damageHead,
                        (float)PY.CustomProperties["height"],
                        (int)PY.CustomProperties["skin"],
                        (int)PY.CustomProperties["team"],
                        (string)PY.CustomProperties["Gmode"],
                        (int)PY.CustomProperties["mesh"]);


                    }
                }

            }
        }
    }

    #endregion

    
    
    #region COLLISION WITH OBJECTS
    //creates the particle effect and disables the other effects such as the render

    public void ColisionBulletPlayer()
    {
        //stop the bullet
        rb.velocity = new Vector3(0, 0, 0);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        col.enabled = false;
        //set the mesh renderer to false
        mshR.enabled = false;

        //play the hit particle

        if (partcl!=null)
        {
            partcl.Play();

            Destroy(partcl.gameObject, 0.1f);

            //destroy the bullet after "x" seconds
            Destroy(gameObject, destroyTime);
        }

       
        
    }

    public void ColisionBulletEnv()//Vector3 pos)
    {

       
        //stop the bullet
        rb.velocity = new Vector3(0, 0, 0);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        col.enabled = false;
        //set the mesh renderer to false
        mshR.enabled = false;

        //put the particles in place
        //partclEnv.transform.position = pos;

        if (partcl!=null)
        {
            partcl.Play();

            Destroy(partcl.gameObject, 0.1f);

            //destroy the bullet after "x" seconds
            Destroy(gameObject, destroyTime);
        }

    
    }

    #endregion



    public void disableEffects()
    {
        //disable the halo effect
        shootHalo.enabled = false;

    }

}
