using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

/// <summary>
/// health of a Drone
/// </summary>
public class DroneHealth : MonoBehaviourPun
{
    // Start is called before the first frame update
    [Header("Health of the Drone")]
    public int health=100;
    Animator animTor;

    [Header("Score that gives a Drone kill")]
    public int scoreDrone = 50;

    public bool dead = false;
    DroneMovement DroneMovScp;

    [Header("Player that hit the Drone before dying")]
    public Player lastHitPlayer;

    [Header("Navmesh used for movement")]
    public NavMeshAgent mshAgnt;

    #region UNITY FUNCTIONS
    void Start()
    {
        //initialization
        mshAgnt = GetComponent<NavMeshAgent>();
    
        DroneMovScp = GetComponent<DroneMovement>();
      
        animTor = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //only if master client, controll the Drones's life
        if (PhotonNetwork.IsMasterClient)
        {
            if (health <= 0 && dead == false)
            {
             
                //is dead
                dead = true;

                DroneMovScp.enabled = false;
  

                //mshAgnt.isStopped = true;
                mshAgnt.enabled = false;
                
                CancelInvoke();

                //set dying animation
                animTor.SetBool("die", true);


                if(lastHitPlayer!=null)
                {                                                           
                    
                    // update score only using the score of the Drone
                    Player PY = lastHitPlayer;
                    PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"]+1,
                        (int)PY.CustomProperties["deaths"],
                        (int)PY.CustomProperties["score"]+ scoreDrone,
                        (int)PY.CustomProperties["health"],
                        (float)PY.CustomProperties["height"],
                        (int)PY.CustomProperties["skin"],
                        (int)PY.CustomProperties["team"],
                        (string)PY.CustomProperties["Gmode"],
                        (int)PY.CustomProperties["mesh"]);


                }

                Invoke("DestroyDrone", 1.5f);

            }
        }
    }

    #endregion

    #region GET HITS AND DAMAGE
    public void setDamageFalse()
    {
        animTor.SetBool("damage", false);

    }

    //when getting hit stop the Drone for a while and make it move after
    public void getHit(int damg)
    {
        if (dead == false)
        {
            DroneMovScp.enabled = false;
          
            
            mshAgnt.enabled = false;

            health -= damg;
            Invoke("setDamageFalse", 0.8f);
            Invoke("ReEnaleAgent", 2f);
        }
    }

    #endregion

    public void ReEnaleAgent()
    {
           
        DroneMovScp.enabled = true;
      
        mshAgnt.enabled = true;

    }
   

    public void DestroyDrone()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    
        

}
