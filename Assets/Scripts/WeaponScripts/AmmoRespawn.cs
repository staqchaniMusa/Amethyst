using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public enum TypeOfMagazine { pistol, rifle };

public class AmmoRespawn : MonoBehaviour
{
 
    public GameObject mag;
    public TypeOfMagazine typeMag;
    HandGrabbing handGrabScp;
    bool grabCondition;
    string handName;
    GameObject myHand;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //stop if there is no hand selectedd
        if(!myHand)
        {
            return;
        }

        //grabbing condition
        bool grabCondition = false;
        
        if (myHand.tag == "handLeft")
        {
            grabCondition = InputManager.instance.G_L_DW;

            handGrabScp = myHand.GetComponent<HandGrabbing>();

            handName = myHand.tag;
        }
        else if (myHand.tag == "handRight")
        {
            grabCondition = InputManager.instance.G_R_DW;

            handGrabScp = myHand.GetComponent<HandGrabbing>();

            handName = myHand.tag;
        }


        if (grabCondition)
        {
            CreateMagazine(handName);
        }
    }

    /// <summary>
    /// intantiate if grabbing in function of the rifle or pistol
    /// </summary>
    /// <param name="name"></param>
    public void CreateMagazine(string name)
    {
        
        if (typeMag == TypeOfMagazine.rifle)
        {
            mag=(PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "MagazineWeapon0" + (PlayerInfo.PI.myWeapon + 1)), transform.position, transform.rotation));
        }
        if (typeMag == TypeOfMagazine.pistol)
        {
            mag=(PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Magazine"), transform.position, transform.rotation));
        }

        handGrabScp.GrabCustomObject(mag,name);
        

                
    }

    public void OnTriggerStay(Collider other)
    {

        myHand = other.gameObject;

    }

    private void OnTriggerExit(Collider other)
    {
        myHand = null;
    }

}
