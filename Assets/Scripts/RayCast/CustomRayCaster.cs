using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// used to perform a raycaster
/// </summary>
public class CustomRayCaster : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("The hand to raycast")]
    public Transform raycastOrigin;
    [Header("The button to trigger the actions of the menu")]

    LineRenderer linR;

    [Header("Pointer indicator of the raycast")]
    public Transform pointer;



    void Awake()
    {
        linR = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //raycast to place the grabbing object/volume
        
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, 60))
        {
            //check the collision object
            //Debug.Log(hit.collider.gameObject.name);

            //Debug.Log(hit.collider.gameObject.name);
                    
            //set both line points of the renderer
            pointer.position = hit.point;

            linR.SetPosition(0, raycastOrigin.transform.position);
            linR.SetPosition(1, hit.point);

        }
        else
        {
            linR.SetPosition(0, raycastOrigin.transform.position);
            linR.SetPosition(1, raycastOrigin.transform.position+raycastOrigin.transform.forward*10);

            pointer.position = raycastOrigin.transform.position + raycastOrigin.transform.forward * 10;
        }




    }


    public void SetActiveLine(bool b)
    {
        linR.enabled = b;
        pointer.gameObject.SetActive(b);
    }
}
