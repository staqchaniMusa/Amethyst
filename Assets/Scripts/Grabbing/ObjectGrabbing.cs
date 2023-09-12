using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// contains the information to grab an object
/// </summary>
public class ObjectGrabbing : MonoBehaviour
{
    // Start is called before the first frame update

    //this is to know which hand has picked an object
    [Header("Grabbing this object")]
    //public bool isGrabbable=true;

    //basic gamecomponents
    //private Collider _col;
    private Rigidbody _rb;

    public Transform offsetR, offsetL;

    Vector3 deltaOffsetR, deltaOffsetL;

    public HandGrabbing handGrabScp;
    
    public int maskDefault;

    public int pose;

    [Header("RRenders to show while grabbing")]
    public GameObject rendHand_L;
    public GameObject rendHand_R;

    [Header("Releasing this object")]
    public float releaseSpeedFactor=1.5f;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (rendHand_L)
        {
            rendHand_L.SetActive(false);
            rendHand_R.SetActive(false);
        }

        _rb = GetComponent<Rigidbody>();
        //_col = GetComponent<Collider>();

        maskDefault =gameObject.layer;
            
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine || PV.Owner==null )
        {
            _rb.useGravity = false;
        }
    }

    void LateUpdate()
    {

        //if it is not mine, do not show renders
        if (!PV.IsMine && PhotonNetwork.InRoom)
        {
            rendHand_L.SetActive(false);
            rendHand_R.SetActive(false);
        }

        /*        
        if(handGrabScp!=null)
        {
            rendHand_L.SetActive(true);
            rendHand_R.SetActive(true);
        }
        */
    }


    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.name);
        if (other.CompareTag("handRight") || other.CompareTag("handLeft"))
        {
            if (CheckIfExists(gameObject, other.gameObject.GetComponent<HandGrabbing>().potentialOnjectInHand) ==false)
            {
                // add the gameobject to the list
                other.gameObject.GetComponent<HandGrabbing>().potentialOnjectInHand.Add(gameObject);
            }
        }        

    }

    /// <summary>
    /// Used to know if the gameobect exists in the list
    /// </summary>
    /// <param name="go"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    bool CheckIfExists(GameObject go, List<GameObject> list)
    {
        bool b = false;

        for (int ii = 0; ii < list.Count; ii++)
        {
            if (list[ii] == gameObject)
            {
                b=true;

                ii = 10000000;
                break;
            }

        }

        
        return b;
    }

    /// <summary>
    /// erase the gameobjec from the list  when exiting the collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("handRight"))
        {
            int indxEquivalent = -1;
            for (int ii = 0; ii < other.gameObject.GetComponent<HandGrabbing>().potentialOnjectInHand.Count; ii++)
            {
                if (other.gameObject.GetComponent<HandGrabbing>().potentialOnjectInHand[ii]==gameObject)
                {
                    indxEquivalent = ii;
                }

            }
            //remove if it is found
            if (indxEquivalent >= 0)
            {
                other.gameObject.GetComponent<HandGrabbing>().potentialOnjectInHand.RemoveAt(indxEquivalent);
            }
        }
        else if (other.CompareTag("handLeft") )
        {
            int indxEquivalent = -1;
            for (int ii = 0; ii < other.gameObject.GetComponent<HandGrabbing>().potentialOnjectInHand.Count; ii++)
            {
                if (other.gameObject.GetComponent<HandGrabbing>().potentialOnjectInHand[ii] == gameObject)
                {
                    indxEquivalent = ii;
                }

            }

            //remove if it is found
            if (indxEquivalent>=0)
            {
                other.gameObject.GetComponent<HandGrabbing>().potentialOnjectInHand.RemoveAt(indxEquivalent);
            }
        }


    }

    public void Release()
    {

        gameObject.transform.SetParent(null);

        _rb.isKinematic = false;
        _rb.useGravity = true;
        //_col.enabled = true;
        gameObject.layer = maskDefault;

        _rb.velocity =releaseSpeedFactor*handGrabScp.filteredSpeed;
        _rb.angularVelocity = releaseSpeedFactor * handGrabScp.filteredAngularSpeed;

        handGrabScp = null;
    }

    public void RigidBodyToKn()
    {
        _rb.useGravity = false;
        _rb.isKinematic = true;
    }

        
    /// <summary>
    /// Used to show in network
    /// </summary>
    /// <param name="b"></param>
    /// <param name="name"></param>
    public void SetRendHandEnabled(bool b, string name)
    {
        PV = GetComponent<PhotonView>();
        //if (PV.IsMine)
        //{
            
        if (name == "handLeft")
        {
            if (rendHand_L)
            {
                rendHand_L.SetActive(b);
            }

            PV.RPC("RPC_SetRendHandEnabled_L", RpcTarget.OthersBuffered, b);
        }
        else if (name == "handRight")
        {
            if (rendHand_R)
            {
                rendHand_R.SetActive(b);
            }

            PV.RPC("RPC_SetRendHandEnabled_R",RpcTarget.OthersBuffered,b);
        }
            
        //}
    }

    [PunRPC]
    public void RPC_SetRendHandEnabled_R(bool b)
    {
        if (rendHand_R)
        {
            rendHand_R.SetActive(b);
        }
    }
    [PunRPC]
    public void RPC_SetRendHandEnabled_L(bool b)
    {
        if (rendHand_L)
        {
            rendHand_L.SetActive(b);
        }
    }

}
