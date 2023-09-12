using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.AI;

/// <summary>
/// this is managed by the master in the room and only him
/// </summary>
/// 
public class DroneManager : MonoBehaviourPun
{
    // Start is called before the first frame update
    float elapsed;

    [Header("Spawing time of each Drone")]
    public float timeToSpawn;
    //public float
    [Header("Spawn points (set in script --> childs of this GO)")]
    public Transform[] spawnP;
    [Header("If you want to intantiate more than a Drone type")]
    public string[] DroneNames;

    void Start()
    {
        //get the spawn points
        spawnP = new Transform[transform.childCount];
        for(int ii=0;ii<transform.childCount;ii++)
        {
            spawnP[ii] =transform.GetChild(ii);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;

        //Debug.Log((string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"]);

        if(PhotonNetwork.IsMasterClient && (string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"]==TypeMode.drone.ToString())
        {
           
            //start putting Drones
            if(elapsed> timeToSpawn)
            {
                // random point to spawn
                int randomIndex =Random.Range(0,spawnP.Length);
                //random Drone avatar from range
                int randomAvatar = Random.Range(0, DroneNames.Length);

                GameObject goInst = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", DroneNames[randomAvatar]),spawnP[randomIndex].position , spawnP[randomIndex].rotation);
                //set destination to default
                goInst.GetComponent<NavMeshAgent>().SetDestination(new Vector3(0,0,0));

                //restore time to check the spawing threshold again 
                elapsed = 0;
            }

        }
    }
}
