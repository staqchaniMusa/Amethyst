using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// THE flag is used in the flag gamemode and scores whent he user take is to their base
/// </summary>
public class Flag : MonoBehaviour
{
    [Header("Team index 0=alpha 1= bravo")]
    public int team=0;

    PhotonView PV;

    [Header("param")]
    public bool snapped;
    
    float elapsed;

    Collider _col;
    Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        elapsed = 0;
        PV = GetComponent<PhotonView>();

        _col = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
     
    }


    private void OnTriggerEnter(Collider other)
    {
        // snaps the flag in these two tags
        if (other.tag == "snapFlagAlpha" && team==0
         || other.tag == "snapFlagBravo"  && team ==1)
        {
            ObjectGrabbing grabbingScp = GetComponent<ObjectGrabbing>();

            if (grabbingScp.handGrabScp)
            {
                grabbingScp.handGrabScp.objectInHand = null;
                grabbingScp.handGrabScp.potentialOnjectInHand.Clear();
            }

            //releasing process

            //_col.enabled = false;
            _col.isTrigger = false;
            _col.enabled = false;
            _rb.useGravity = false;
            _rb.isKinematic = true;

            other.GetComponent<Renderer>().enabled = false;

            transform.position = other.transform.position;
            transform.forward = -other.transform.forward;

            grabbingScp.enabled = false;
            grabbingScp.SetRendHandEnabled(false, "handLeft");
            grabbingScp.SetRendHandEnabled(false, "handRight");

            snapped = true;


            // update scores and destroy
            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC("RPC_UpdateScore",RpcTarget.AllBuffered,team,1);
            }

        }
    }

    [PunRPC]
    public void RPC_UpdateScore(int tm, int sc)
    {
        FlagManager.instance.UpdateScore(tm, sc);
        PhotonNetwork.Destroy(gameObject);
    }
}
