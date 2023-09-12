using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Start is called before the first frame update

    //the line renderer attached to this gameobject
    LineRenderer linR;


    void Start()
    {
        linR = GetComponent<LineRenderer>();
        linR.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        linR.SetPosition(0,transform.position);

       
        //draw line
        linR.SetPosition(1, transform.position + transform.forward * 10);
       
      
    }
}
