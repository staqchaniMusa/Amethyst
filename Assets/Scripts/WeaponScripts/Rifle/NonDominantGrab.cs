using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NonDominantGrab : MonoBehaviour
{
    private DynamicRifle rifleScp;
    private RocketLauncher launcherScp;
    public GameObject rendHand_L;
    public GameObject rendHand_R;
    PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = transform.root.GetComponent<PhotonView>();
        rendHand_L.SetActive(false);
        rendHand_R.SetActive(false);
        rifleScp = transform.parent.parent.GetComponent<DynamicRifle>();
        launcherScp = transform.parent.parent.GetComponent<RocketLauncher>();

    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.G_R_UP ||InputManager.instance.G_L_UP)
        {
                     
            //case if it is a rifle
            if (rifleScp)
            {
                if (rifleScp.objectGrabbingScript.handGrabScp)
                {
                    rifleScp.objectGrabbingScript.handGrabScp.otherHand.rend.enabled = true;
                    if (rifleScp.objectGrabbingScript.handGrabScp.otherHand.watch != null)
                    {
                        rifleScp.objectGrabbingScript.handGrabScp.otherHand.watch.SetActive(true);
                        rifleScp.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = false;
                    }
                }
                rifleScp.secondaryGrabb = null;
               
            }
            //case if it is a launcher
            else if(launcherScp)
            {
                if (launcherScp.objectGrabbingScript.handGrabScp)
                {
                    launcherScp.objectGrabbingScript.handGrabScp.otherHand.rend.enabled = true;
                    if (launcherScp.objectGrabbingScript.handGrabScp.otherHand.watch != null)
                    {
                        launcherScp.objectGrabbingScript.handGrabScp.otherHand.watch.SetActive(true);
                        launcherScp.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = false;
                    }
                }
                launcherScp.secondaryGrabb = null;
               
            }

             //set renders to false
             rendHand_L.SetActive(false);
             rendHand_R.SetActive(false);
            
            

        }

    }

    private void LateUpdate()
    {
        if (!PV.IsMine && PhotonNetwork.InRoom)
        {
            rendHand_L.SetActive(false);
            rendHand_R.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //if rifle
        if (rifleScp)
        {
            if ((other.gameObject.CompareTag("handRight") && rifleScp.objectGrabbingScript.handGrabScp
                && (InputManager.instance.G_R_DW))
                || (other.gameObject.CompareTag("handLeft")
                && (InputManager.instance.G_L_DW)))

            {
                if ((rifleScp.objectGrabbingScript.handGrabScp.CompareTag("handLeft")
                && (InputManager.instance.G_R_DW))
                || (other.gameObject.CompareTag("handLeft")
                && rifleScp.objectGrabbingScript.handGrabScp.CompareTag("handRight")
                && (InputManager.instance.G_L_DW)))
                {
                    if (rifleScp)
                    {
                        rifleScp.secondaryGrabb = this;
                    }

                    // prepare the grabbing

                    //if (PV.IsMine  )
                    //{
                        SetRendHandEnabled(true, other.gameObject.tag);
                    //}
                    //else
                    //{
                        //SetRendHandEnabled(false, other.gameObject.tag);
                    //}

                   
                    rifleScp.objectGrabbingScript.handGrabScp.otherHand.rend.enabled = false;
                    rifleScp.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = true;

                    if (rifleScp.objectGrabbingScript.handGrabScp.otherHand.watch != null)
                    {
                        rifleScp.objectGrabbingScript.handGrabScp.otherHand.watch.SetActive(false);
                    }
                  


                }
            }
        }
        //if launcher
        else if (launcherScp)
        {
            //Debug.Log("HEre");

            if ((other.gameObject.CompareTag("handRight") && launcherScp.objectGrabbingScript.handGrabScp
            && (InputManager.instance.G_R_DW))
            || (other.gameObject.CompareTag("handLeft")
            && (InputManager.instance.G_L_DW)))

            {
                if ((launcherScp.objectGrabbingScript.handGrabScp.CompareTag("handLeft")
                && (InputManager.instance.G_R_DW))
                || (other.gameObject.CompareTag("handLeft")
                && launcherScp.objectGrabbingScript.handGrabScp.CompareTag("handRight")
                && (InputManager.instance.G_L_DW)))
                {
                    if (launcherScp)
                    {
                        launcherScp.secondaryGrabb = this;
                    }

                    //Debug.Log("HEre2");

                    //if (PV.IsMine)
                    //{
                        SetRendHandEnabled(true, other.gameObject.tag);
                    //}
                    //else
                    //{
                        //SetRendHandEnabled(false, other.gameObject.tag);
                    //}

                  
                    launcherScp.objectGrabbingScript.handGrabScp.otherHand.rend.enabled = false;
                    launcherScp.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = true;

                    if (launcherScp.objectGrabbingScript.handGrabScp.otherHand.watch != null)
                    {
                        launcherScp.objectGrabbingScript.handGrabScp.otherHand.watch.SetActive(false);
                    }
                   


                }
            }
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("handRight")
           || other.gameObject.CompareTag("handLeft"))
        {
            if (rifleScp)
            {
                if(rifleScp.objectGrabbingScript.handGrabScp)
                {
                    rifleScp.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = false;
                }
                
                rifleScp.secondaryGrabb = null;
            }
            else if (launcherScp)
            {
                if (launcherScp.objectGrabbingScript.handGrabScp)
                {
                    launcherScp.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = false;
                }

                launcherScp.secondaryGrabb = null;
            }

            SetRendHandEnabled(false, other.gameObject.tag);
           


        }
    }

    /// <summary>
    /// rend handle
    /// </summary>
    /// <param name="b"></param>
    /// <param name="name"></param>
    public void SetRendHandEnabled(bool b, string name)
    {
        if (name == "handLeft")
        {
            rendHand_L.SetActive(b);
        }
        else if (name == "handRight")
        {
            rendHand_R.SetActive(b);
        }
    }

    private void OnDestroy()
    {
        //if rifle
        if (rifleScp)
        {
            if (rifleScp.objectGrabbingScript.handGrabScp)
            {
                rifleScp.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = false;
            }

            rifleScp.secondaryGrabb = null;
        }
        //if launcher
        else if (launcherScp)
        {
            if (launcherScp.objectGrabbingScript.handGrabScp)
            {
                launcherScp.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = false;
            }

            launcherScp.secondaryGrabb = null;
        }
    }

}
