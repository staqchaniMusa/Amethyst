using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// it is used to move with simple AI the drones in the game
/// </summary>
public class DroneMovement : MonoBehaviourPun
{
   
    NavMeshAgent navMeshAgnt;

    [Header("Initial destination")]
    public Vector3 destination;
    Animator animTor;
    GameObject[] playerAvatars;


    [Header("MALE Attack settings (activate in script)")]
    public float attackDistance =0.6f;
    public float timeBetweenAttaks = 2.5f;
    float attackTime;
    int damage = 10;
    PhotonView PV;
    public GameObject objectiveShooting;


    [Header("Movement settings")]
    public float speed;
    public float rotSpeed;
    public float acceleraton;
    public float stopDistance;
    float elapsed;
    Coroutine corr;
    DroneAttack droneAttacScript;

    string roomMode;

    #region UNITY FUNCTIONS
    void Awake()
    {

        //initialization
        navMeshAgnt = GetComponent<NavMeshAgent>();
        droneAttacScript = transform.GetChild(0).GetComponent<DroneAttack>();

        //set navmesh parameters
        navMeshAgnt.speed = speed;
        navMeshAgnt.angularSpeed = rotSpeed;
        navMeshAgnt.acceleration = acceleraton;
        navMeshAgnt.stoppingDistance = stopDistance;

        if (navMeshAgnt.enabled && PhotonNetwork.IsMasterClient)
        {
            navMeshAgnt.SetDestination(destination);
        }

        roomMode = (string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"];
        PV = GetComponent<PhotonView>();
        animTor = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animTor.SetFloat("speed", navMeshAgnt.velocity.magnitude);

        attackTime += Time.fixedDeltaTime;

        if (!droneAttacScript.isFromPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                navMeshAgnt.enabled = true;
                if (corr == null)
                {
                    corr = StartCoroutine(FindClosestPlayer_Co());
                }
            }
            else
            {
                navMeshAgnt.enabled = false;
            }
        }
        else 
        {
            if (PV.IsMine)
            {
                navMeshAgnt.enabled = true;
                if (corr == null)
                {
                    corr = StartCoroutine(FindClosestPlayer_Co());
                }
            }
            else
            {
                navMeshAgnt.enabled = false;
            }
        }
          
    }

    #endregion


    /// <summary>
    /// finds the closeest player if it is not from the same team
    /// </summary>
    /// <returns></returns>
    public IEnumerator FindClosestPlayer_Co()
    {

        if (PV != null)
        {
            //use team variable        
            int team = 0;

            if (PV.Owner!=null)
            {
                if ((int)PV.Owner.CustomProperties["team"] == 0)
                {
                    team = 1;
                }
                else
                {
                    team = 0;
                }
            }


            //these are all the avatars
            playerAvatars = GameObject.FindGameObjectsWithTag("Avatar");

            //get lower distance to player
            float distance = 10000000.0f;
            int indxMin = -1;
            objectiveShooting = null;

            //if the search is not null
            if (playerAvatars != null)
            {
                for (int ii = 0; ii < playerAvatars.Length; ii++)
                {
                    float distanceToPlayer = (playerAvatars[ii].transform.position - transform.position).magnitude;


                    if (droneAttacScript.isFromPlayer)
                    {

                        if (distanceToPlayer < distance
                            && (
                            (int)playerAvatars[ii].transform.root.GetComponent<PhotonView>().Owner.CustomProperties["team"] == team
                            && roomMode != TypeMode.drone.ToString()
                            || roomMode==TypeMode.drone.ToString()
                            )

                            && (int)playerAvatars[ii].transform.root.GetComponent<PhotonView>().Owner.CustomProperties["health"] > 0)
                        {
                            distance = distanceToPlayer;
                            indxMin = ii;
                        }
                    }
                    else
                    {
                        //Debug.Log("distplayer:"+ distanceToPlayer+" distnace="+distance);
                        if (distanceToPlayer < distance)
                        {                            
                            distance = distanceToPlayer;
                            indxMin = ii;
                        }
                    }
                }

                //if there are avatars in the scene, go attack the closest
                if (playerAvatars.Length > 0 && indxMin>=0)
                {
                    objectiveShooting = playerAvatars[indxMin];

                    if (navMeshAgnt.enabled == true)
                    {
                        navMeshAgnt.SetDestination(playerAvatars[indxMin].transform.position);
                    }

                    //////////////////////////////////////////
                    // if you want MALE ATTACK USE THIS
                    //////////////////////////////////////////

                    /*check distance if is lower than the attack distance and weather timing is correct
                    if (distance <= attackDistance && attackTime > timeBetweenAttaks)
                    {
                        navMeshAgnt.enabled = false;

                        animTor.SetBool("attack", true);

                        Invoke("setAttackToFalse", 1.2f);
                        attackTime = 0;

                        // when distance is low enough, perform an attack to the player, in which the health is reduced the damage ammount
                        AttackByContactOnPlayer(indxMin);

                    }
                    */
                }
                else
                {
                    objectiveShooting = null;

                }
                yield return null;
            }
        }
        yield return null;

        corr = null;
    }




    #region
    /// <summary>
    /// This allows a meele atack
    /// </summary>
    /// <param name="indxMin"></param>
    public void AttackByContactOnPlayer(int indxMin)
    {
        //get and set custom properties
        Player PY = playerAvatars[indxMin].GetComponent<PhotonView>().Owner;

        if ((int)PY.CustomProperties["health"] > 0)
        {
            PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                        (int)PY.CustomProperties["deaths"],
                        (int)PY.CustomProperties["score"],
                        (int)PY.CustomProperties["health"] - damage,
                        (float)PY.CustomProperties["height"],
                        (int)PY.CustomProperties["skin"],
                        (int)PY.CustomProperties["team"],
                        (string)PY.CustomProperties["Gmode"],
                           (int)PY.CustomProperties["mesh"]);
        }

    }

    #endregion
    [PunRPC]
    public void RPC_attactToPlayer(GameObject gameObj)
    {
        //gameObj.GetComponent<PlayerHealth>().getHit(damage, transform.position);
    }

    public void setAttackToFalse()
    {
        navMeshAgnt.enabled = true;
        animTor.SetBool("attack", false);
    }

    
}
