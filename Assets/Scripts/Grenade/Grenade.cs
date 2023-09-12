using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Grenade : MonoBehaviour
{
    // Start is called before the first frame update
    //[Header("When ready=true, it is prepared to explode")]
    [Header("Networking")]
    public bool isNetworkVisible = true;

    float elapsed = 0;

    [Header("Grenade parameters")]
    public float timeToExplode = 5;
    public float maxDist = 5;
    public float maxDamage = 200;
    public Renderer[] rends;
    public bool ready;
    public bool end = false;

    [Header("The particles systems")]
    public ParticleSystem[] particles;

    ObjectGrabbing objsScp;

    PhotonView PV;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        //NEtworking visible
        if (!PV.IsMine)
        {
            if (!isNetworkVisible)
            {
                for (int ii = 0; ii < transform.childCount; ii++)
                {
                    transform.GetChild(ii).gameObject.SetActive(false);
                }
                //_col.enabled = false;
            }
            else
            {
                for (int ii = 0; ii < transform.childCount; ii++)
                {
                    transform.GetChild(ii).gameObject.SetActive(true);
                }
                //_col.enabled = true;
            }
        }

        elapsed += Time.deltaTime;
        
        if (elapsed > timeToExplode && ready && end == false)
        {           
            Explode();
            end = true;
        }               
    }

    public void Activate()
    {
        ready = true;
        elapsed = 0;

        objsScp = GetComponent<ObjectGrabbing>();

        if (objsScp.handGrabScp == null)
        {
            objsScp.GetComponent<Rigidbody>().useGravity = true;
            transform.SetParent(null);
        }
    }

    public void Explode()
    {

        PV.RPC("RPC_Explode", RpcTarget.All);

        //hit the players, avatars in range
        GameObject[] playerAvatars = GameObject.FindGameObjectsWithTag("Avatar");


        //if the search is not null
        if (playerAvatars != null)
        {
            for (int ii = 0; ii < playerAvatars.Length; ii++)
            {
                float distanceToGrenade = (playerAvatars[ii].transform.position - transform.position).magnitude;

                int damage = (int)Mathf.Clamp(maxDamage*(1-distanceToGrenade/maxDist),0,maxDamage);



                Player PY = playerAvatars[ii].GetComponent<PhotonView>().Owner;

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

                playerAvatars[ii].transform.root.GetComponent<PlayerHealth>().SetLasPlayerHit(PhotonNetwork.LocalPlayer);


            }

        }




        //hit the tanks, avatars in range
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("tank");
        
        //if the search is not null
        if (tanks != null)
        {
            for (int ii = 0; ii < tanks.Length; ii++)
            {
                float distanceToGrenade = (tanks[ii].transform.position - transform.position).magnitude;

                int damage = (int)Mathf.Clamp(maxDamage * (1 - distanceToGrenade / maxDist), 0, maxDamage);

                
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
                float distanceToGrenade = (planes[ii].transform.position - transform.position).magnitude;

                int damage = (int)Mathf.Clamp(maxDamage * (1 - distanceToGrenade / maxDist), 0, maxDamage);


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
                float distanceToGrenade = (drones[ii].transform.position - transform.position).magnitude;

                int damage = (int)Mathf.Clamp(maxDamage * (1 - distanceToGrenade / maxDist), 0, maxDamage);


                if (damage > 0)
                {
                    Player PY = PV.Owner;

                    drones[ii].gameObject.transform.root.GetComponent<DroneHealth>().getHit(damage);
                    drones[ii].gameObject.transform.root.GetComponent<DroneHealth>().lastHitPlayer = PY;

                    Debug.Log("BulletTank hit: " + drones[ii].gameObject.name + "  damage-->" + damage);
                }
            }

        }


        Invoke("DestroyGrenade", 1);
    }


    [PunRPC]
    public void RPC_Explode()
    {

        for (int ii = 0; ii <rends.Length; ii++)
        {
            rends[ii].enabled = false;
        }

        for (int ii = 0; ii < particles.Length; ii++)
        {
            if (!particles[ii].isPlaying)
            {
                particles[ii].Play();
            }
        }
    }

    public void DestroyGrenade()
    {
        PhotonNetwork.Destroy(gameObject);
    }


    /// <summary>
    /// to shot in network
    /// </summary>
    /// <param name="b"></param>
    public void SetNetWorkVisible(bool b)
    {
        PV.RPC("RPC_SetNetVis", RpcTarget.AllBuffered, b);
    }

    [PunRPC]
    public void RPC_SetNetVis(bool b)
    {
        isNetworkVisible = b;

    }

}
