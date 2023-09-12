using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEvntCam : MonoBehaviour
{


     // Update is called once per frame
    void FixedUpdate()
    {
        //notice that some canvases are disabled or some appear on the scene... this is not very efficient, but works
        SetEventCameras();
            
    }

    public void SetEventCameras()
    {
        //get all the canvas and set them to the custom event camera
        Canvas[] go = GameObject.FindObjectsOfType<Canvas>();
        if (GameObject.FindGameObjectWithTag("pointerCam") != null)
        {
            for (int ii = 0; ii < go.Length; ii++)
            {
                go[ii].worldCamera = GameObject.FindGameObjectWithTag("pointerCam").GetComponent<Camera>();
                                       
            }
        }
        
    }
}
