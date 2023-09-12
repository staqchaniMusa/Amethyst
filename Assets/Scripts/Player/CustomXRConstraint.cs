using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class CustomXRConstraint : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Reference to controller and player's head and rig")]
    public CharacterController chrController;
    public Transform head;
    public Transform rig;
    public Transform rightHand, leftHand;

    [Header("Stick and speeds")]
    public float speedF_normal = 3.75f;
    public float speedF_running = 5.5f;
    public float distanceToRun = 1.2f;
    public bool canMove = true;

    float speedF = 0.01f;
    public float speedR = 0.005f;

    [Header("Update Frequency and distance")]
    public float timeUpdate=1f;
    Vector3 delta;

    [Header("Snaping")]
    public Transform xrObject;
    bool snapped = false;
    public float rotationApplied = 45;


    Vector3 proyectionHead, proyectionController;
    Vector3 savedHead, savedController;
    float elapsed;

    public bool climbing;

    public float grav = -0.05f;
    public float initialGrav=-0.05f;

    private float distance;
    void Start()
    {
        grav = initialGrav;
        savedHead = head.position;
        savedController = chrController.transform.position;

        //StartCoroutine(FollowHead());
        elapsed = 0;
    }

    //this overrides the movement of the controller
    public void Update()
    {

        if((head.position- rightHand.position).y> distanceToRun
            && (head.position - rightHand.position).y > distanceToRun
            )
        {
            speedF = Mathf.Lerp(speedF,speedF_running,0.1f);
            
        }
        else
        {
            speedF = Mathf.Lerp(speedF_normal, speedF_running, 0.1f); ;
        }

        elapsed += Time.deltaTime;

        //SAVED POSITION OF HEAD AND CONTROLLER
        savedHead = head.position;
        savedController = chrController.transform.position;

       
        //Forward and right directions
        Vector3 dirF = head.forward;
        dirF = new Vector3(dirF.x, 0, dirF.z);
        Vector3 dirR = head.right;
        dirR = new Vector3(dirR.x, 0, dirR.z);

        //THE DIFFERENTIAL MOVEMENT THAT OCCURS AT THE LEVEL OF THE HEAD AND THE CONTROLLER
        //delta = savedHead - savedController + speedF * InputManager.instance.axisL.y * dirF + speedR * InputManager.instance.axisL.x * dirR;
        delta = + speedF*Time.deltaTime * InputManager.instance.axisL.y * dirF + speedR * Time.deltaTime * InputManager.instance.axisL.x * dirR;

        //OVERRIDE MOVEMENT
        if (chrController.enabled && canMove)
        {

            if (chrController.isGrounded || climbing==false)
            {
                grav = initialGrav; // grounded character has vSpeed = 0...
                
            }
            else
            {
                grav += initialGrav*Time.fixedDeltaTime;
            }

            if(climbing==true)
            {
                grav = 0;
            }


            chrController.Move(new Vector3(delta.x, grav, delta.z));
        }

        /*Vector3 delta2 = chrController.transform.position - head.transform.position;
        rig.position += new Vector3(delta2.x, 0, delta2.z);
        */


        // OVERRIDE SNAPPING
        if (Mathf.Abs(InputManager.instance.axisR.x) > 0.9f && snapped == false)
        {
            snapped = true;

            // apply rotation

            xrObject.rotation *= Quaternion.Euler(0, Mathf.Sign(InputManager.instance.axisR.x) * rotationApplied, 0);

        }
        else if (Mathf.Abs(InputManager.instance.axisR.x) < 0.2f)
        {
            snapped = false;
        }




    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="tank"|| collision.gameObject.tag == "plane")
        {
            canMove = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "tank" || collision.gameObject.tag == "plane")
        {
            canMove = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
        //if colision with the environment, 
        if(other.tag=="environmnet")
        {
            //GetComponent<TrackedPoseDriver>().trackingType=TrackedPoseDriver.TrackingType.RotationOnly;
            Vector3 direction = transform.position - other.transform.position;
            direction = new Vector3(direction.x, 0, direction.z);

            chrController.Move(direction/direction.magnitude*0.01f);
            
        }
        
    }

    

    ///////////////////////
    // CORRUTINE TESTS: /// USEFUL IN SOME CASES
    ///////////////////////
    /* Update is called once per frame
    void LateUpdate()
    {

        //one frame to move ovr rig onether to move head

            //save the position
            

            //get the proyection in x and z of the controller and the head
            proyectionController = new Vector3(chrController.transform.position.x, 0, chrController.transform.position.z);
            proyectionHead = new Vector3(head.position.x, 0, head.position.z);

            distance = (proyectionHead - proyectionController).magnitude;

            if (distance>dist && !inCor)
            {
              StartCoroutine(Recenter());
            }
  
        
    }

    public IEnumerator Recenter()
    {

        yield return null;

        inCor = true;

        savedHead = head.position;
        savedController = chrController.transform.position;

        delta = savedHead - savedController;

        chrController.Move(new Vector3(delta.x, 0, delta.z));

        Debug.Log("Moved controller");
        yield return new WaitForSeconds(0.05f);

        Vector3 delta2 =chrController.transform.position-head.transform.position;
        rig.position += new Vector3(delta2.x, 0, delta2.z) ;

        Debug.Log("Moved head");
        

        yield return new WaitForSeconds(0.05f);
        inCor = false;
        yield return new WaitForSeconds(0.05f);
    }*/

}
