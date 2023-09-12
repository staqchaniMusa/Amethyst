using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Used to make objects look to the main player (OVR CAMERA)
/// </summary>
public class LookAtMainPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    Transform player;
    void Start()
    {
        //LookAtMainPlayer for the player
        /*if( GameObject.FindGameObjectWithTag("head")!=null)
         {
             player = GameObject.FindGameObjectWithTag("head").transform;
         }
         */

        //player = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //get the main OVR gameobject (camera of the player called "head")
        //if (GameObject.FindGameObjectWithTag("head") != null)
        //{
        //player = GameObject.FindGameObjectWithTag("head").transform;

        if (Camera.main!=null)
        {
            Vector3 objective = Camera.main.transform.position - transform.position;

            //this is needed to change the orientation to the correct direction
            transform.forward = -objective;
            //}

        }  
       
    }
}
