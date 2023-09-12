using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// used to instantiate the photon object when grabbing from the store.
/// </summary>
public class GrabFromStore : MonoBehaviour
{
   
    HandGrabbing handGrabScp;
    bool grabCondition;
    string handName;
    GameObject myHand;
    GameObject objectStore;
    string prefabName;

    [Header("Object prize")]
    public int prize = 50;

    

    // Start is called before the first frame update
    void Start()
    {
        prefabName = name;
    }

    // Update is called once per frame
    void Update()
    {
        if (!myHand)
        {
            return;
        }

        bool grabCondition = false;

        //check grabbing condition
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

        //If the player grabs the object
        if (grabCondition && ShopManager.instance.coins>=prize)
        {
            ShopManager.instance.coins -= prize;
            GrabObjectFromStore(handName);
        }
    }

    /// <summary>
    /// creates the actual photon object
    /// </summary>
    /// <param name="name"></param>
    public void GrabObjectFromStore(string name)
    {
        objectStore = (PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", prefabName), transform.position, transform.rotation));
       
        handGrabScp.GrabCustomObject(objectStore, name);
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "handLeft" || other.tag == "handRight")
        {
            myHand = other.gameObject;
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "handLeft" || other.tag == "handRight")
        {
            myHand = null;
        }
    }
}
