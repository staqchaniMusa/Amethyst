using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// object that can be climbed with hands
/// </summary>
public class Ladder : MonoBehaviour
{
    
    public GameObject[] insideHand;
    CharacterController cc;
    [Header("Character controller")]
    CustomXRConstraint customXRmov;
    Vector3[] grabPosition;
    [Header("Impose speed for climbing")]
    public float climbFactor;

    // Start is called before the first frame update
    void Start()
    {
        grabPosition = new Vector3[2];
        //cc = GameObject.FindGameObjectWithTag("XR").GetComponent<CharacterController>();
        //customXRmov =Camera.main.gameObject.GetComponent<CustomXRConstraint>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // find the objects needed
        if(GameObject.FindGameObjectWithTag("XR")!=null && cc==null)
        {
            cc = GameObject.FindGameObjectWithTag("XR").GetComponent<CharacterController>();
            customXRmov = Camera.main.gameObject.GetComponent<CustomXRConstraint>();
        }
        
        // Left hand
        if (insideHand[0])
        {
            // set the grabing position
            if (InputManager.instance.G_L_DW)
            {
                grabPosition[0] = insideHand[0].transform.position;
              

            }

            //actual movement in function of the distance between the grabbing point and the the hand position
            if (InputManager.instance.G_L)                
            {
                customXRmov.climbing = true;
                Vector3 dir = grabPosition[0]-insideHand[0].transform.position;

                if (dir.magnitude > 0)
                {
                    cc.Move(dir * climbFactor);
                }
            }
            


        }

        //right hand
        if (insideHand[1])
        {

            if (InputManager.instance.G_R_DW)
            {
                grabPosition[1] = insideHand[1].transform.position;
                

            }

            //actual movement in function of the distance between the grabbing point and the the hand position
            if (InputManager.instance.G_R)
            {
                customXRmov.climbing = true;

                Vector3 dir = grabPosition[1] - insideHand[1].transform.position;

                if (dir.magnitude>0)
                {
                    cc.Move(dir * climbFactor);
                }
            }



        }

        //stop climbing when release
        if (InputManager.instance)
        {

            if (InputManager.instance.G_L_UP || InputManager.instance.G_R_UP)
            {
                customXRmov.climbing = false;
            }
        }



    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag== "handLeft")
        {
            insideHand[0] = other.gameObject;
        }
        if(other.tag=="handRight")
        {
            insideHand[1] = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "handLeft")
        {
            insideHand[0] = null;
        }
        if (other.tag == "handRight")
        {
            insideHand[1] = null;
        }
    }
}