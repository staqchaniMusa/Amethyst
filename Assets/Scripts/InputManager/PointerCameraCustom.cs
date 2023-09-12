using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerCameraCustom : MonoBehaviour
{
    // Start is called before the first frame update
    public float defaultLenght=50;
    [Header("The dot used to display the hit")]
    public GameObject dot;
    [Header("The Imput module for VR")]
    public VRInputModule inputManager;
    [Header("the line rend to draw the distance from the hand to the hit")]
    public LineRenderer lineR;
   

    // Update is called once per frame
    void Update()
    {
        //draw line if it is possible
        UpdateLine();
    }

    private void UpdateLine()
    {
        //pointer event data custom
        PointerEventData data = inputManager.GetData();

        float targetLenght = data.pointerCurrentRaycast.distance;

        
        dot.transform.position = transform.position+targetLenght*transform.forward;

        
        lineR.SetPosition(0, transform.position);
        lineR.SetPosition(1, dot.transform.position);

        //choosing a way of rendering the line render with physics or UI
        lineR.enabled = false;
        dot.SetActive(false);

        if (targetLenght==0)
        {
            // used to find the object raycast
            if(VRInputModule.instance.currentObject!=null)
            {
                dot.transform.position = VRInputModule.instance.currentObjectRaycast;

                lineR.SetPosition(0, transform.position);
                lineR.SetPosition(1, dot.transform.position);

                if (inputManager.showRenders)
                {
                    lineR.enabled = true;
                    dot.SetActive(true);
                }
            }
            else
            {
                dot.transform.position = transform.position + transform.forward * defaultLenght;
                lineR.SetPosition(0, transform.position);
                lineR.SetPosition(1, dot.transform.position);
            }
        }

        //show the render
        if(inputManager.showRenders)
        {
            
            lineR.enabled = true;
            dot.SetActive(true);
            
        }

      


    }

    /// <summary>
    /// creates a raycast with a lenght
    /// </summary>
    /// <param name="lenght"></param>
    /// <returns></returns>
    private RaycastHit CreateRayCast(float lenght)
    {
        RaycastHit hit;
        

        Ray ray = new Ray(transform.position, transform.forward);

        Physics.Raycast(ray, out hit, defaultLenght);

        return hit;
    }
}

