using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script allows the user to generate the effect of buttons in case that the OVR pointer didn't not work
/// ¡ OBSOLETE !
/// </summary>
public class Button_XR : MonoBehaviour
{
    
    //this will be the image attached to this gameobject
    Image im;

    //these are the two colors used for the enter and exit events
    public Color colEnter, colExit;


    void Start()
    {
        im = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ALTERNATIVE TO POINTER EVENTS, USING COLLISIONS
    private void OnCollisionStay(Collision collision)
    {
        im.color = colEnter;
    }
    private void OnCollisionExit(Collision collision)
    {
        im.color = colExit;
    }
}
