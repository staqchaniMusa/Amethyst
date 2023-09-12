using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;


public class KnifeWeapon : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Networking")]
    public bool isNetworkVisible = true;


    [Header("Damage to for head and body")]
    public int damageBody, damageHead;

    public Player playerORigin;
    [Header("Reset position")]
    public Transform resetPos;
    Rigidbody _rb;
    public bool isInHand;
    public ObjectGrabbing grabbingScp;
    public float stabSpeed=1.5f;

    public PhotonView PV;
    public Collider _col;
    

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        _col = GetComponent<Collider>();
        grabbingScp = GetComponent<ObjectGrabbing>();
        _rb = GetComponent<Rigidbody>();
        playerORigin = transform.root.gameObject.GetComponent<PhotonView>().Owner;
    }

    // Update is called once per frame
    void Update()
    {

        //NEtworking visible
        if (!PV.IsMine && PhotonNetwork.InRoom)
        {
            if (!isNetworkVisible)
            {
                for (int ii = 0; ii < transform.childCount; ii++)
                {
                    transform.GetChild(ii).gameObject.SetActive(false);
                }
                _col.enabled = false;
            }
            else
            {
                for (int ii = 0; ii < transform.childCount; ii++)
                {
                    transform.GetChild(ii).gameObject.SetActive(true);
                }
                _col.enabled = true;
            }
        }

        //check if it is in hand
        if (grabbingScp.handGrabScp != null)
        {
            isInHand = grabbingScp.handGrabScp.objectInHand == gameObject;
        }
        else
        {
            isInHand = false;
        }

    }


    private void OnTriggerEnter(Collider collision)
    {
        //check if the the knife is in a hand
        if (!GetComponent<PhotonView>().IsMine || grabbingScp.handGrabScp==null)
        {
            return;
        }

        //check speed
        if(grabbingScp.handGrabScp.filteredSpeed.magnitude<stabSpeed)
        {
            return;
        }



        //in function of gamemode
        string gMode = (string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"];

        //check collision with robot
        if (collision.gameObject.tag == TypeMode.drone.ToString())
        {

            collision.gameObject.transform.root.GetComponent<DroneHealth>().getHit(damageHead);
            collision.gameObject.transform.root.GetComponent<DroneHealth>().lastHitPlayer = playerORigin;

        }

        
        //check team mode
        if (gMode == TypeMode.team.ToString())
        {
            PhotonView PV = collision.gameObject.transform.root.GetComponent<PhotonView>();

            //check the teams when hitting an avatar
            if (collision.gameObject.tag == "bodyCollider"&& ((int)PV.Owner.CustomProperties["team"] != (int)PV.Owner.CustomProperties["team"]))
            {


                PlayerHealth plyHealtScript = collision.gameObject.transform.root.GetComponent<PlayerHealth>();


                if (!PV.IsMine)
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
                        (int)PY.CustomProperties["mesh"]
                        );
                }
            }

            //check the teams when hitting a head
            if (collision.gameObject.tag == "head" && (int)playerORigin.CustomProperties["team"] != (int)PV.Owner.CustomProperties["team"])
            {
                
                PlayerHealth plyHealtScript = collision.gameObject.transform.root.GetComponent<PlayerHealth>();

                if (!PV.IsMine)
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
                    (int)PY.CustomProperties["mesh"]
                    );


                }
            }

        }




        //check battle royale mode
        if (gMode == TypeMode.royale.ToString())
        {
            //check the teams when hitting an avatar
            if (collision.gameObject.tag == "bodyCollider")
            {


                PlayerHealth plyHealtScript = collision.gameObject.transform.root.GetComponent<PlayerHealth>();

                PhotonView PV = collision.gameObject.transform.root.GetComponent<PhotonView>();

                if (!PV.IsMine)
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
                    (int)PY.CustomProperties["mesh"]
                    );

                }

            }

            //check the teams when hitting a head
            if (collision.gameObject.tag == "head")
            {


                PlayerHealth plyHealtScript = collision.gameObject.transform.root.GetComponent<PlayerHealth>();

                PhotonView PV = collision.gameObject.transform.root.GetComponent<PhotonView>();

                if (!PV.IsMine)
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
                    (int)PY.CustomProperties["mesh"]
                    );


                }
            }

        }
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("environment"))
        {


            transform.position = resetPos.position;
            transform.rotation = resetPos.rotation;
            _rb.velocity = new Vector3(0, 0, 0);
            _rb.angularVelocity = new Vector3(0, 0, 0);
            _rb.useGravity = false;
            _rb.isKinematic = true;

            transform.SetParent(resetPos);

        }

    }

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
