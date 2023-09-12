using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



/// <summary>
///   SCRIPT THAT MAKES A RIGIDBODY MOVE ACCORDING TO ANOTHER WITH A APPLIED FORCE (FOLLOWING EFFECT)
/// </summary>
public class RigidBodyFollower : MonoBehaviour
{
    [Header("Object to follow")]
    public Transform objective;
    Rigidbody rb;
  
    [Header("Force factor")]
    public float forceFactor = 100;

    [Header("Render 0-->L 1-->R")]
    public GameObject[] hands;


    [Header("Forced distance to release hand")]
    public float distanceToRelease;
    public float dist;

    [Header("Potential hand to grab the handle")]
    public GameObject potentialHand;
    public bool holding;

    PhotonView PV;

    // Start is called before the first frame update
    void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if you want to limit the movement of the  hinglejoint (causes vibration)
        //if (hj.angle<hj.limits.max*0.99f && hj.angle > hj.limits.min*0.99f)
        //{

        hands[0].SetActive(false);
        hands[1].SetActive(false);



        if (objective != null)
        {
            if(objective.tag== "handLeft")
            {
                hands[0].SetActive(true);

            }

            if (objective.tag == "handRight")
            {
                hands[1].SetActive(true);
            }

            //not working properly
            rb.MovePosition(objective.position);
        }


        //rb.MoveRotation(objective.rotation);
        /*Vector3 dir = (objective.position - transform.position);
        if (dir.magnitude>th)
        {
            rb.AddForceAtPosition(forceFactor * (objective.position - transform.position), transform.position);
        }*/

        //rb.velocity = SimpleCarController.instance.GetComponent<Rigidbody>().velocity;


        // this is the most stable solution
        //rb.AddForceAtPosition(forceFactor * (objective.position - transform.position), transform.position);


        //}
        if (potentialHand != null)
        {
            // if the user is picking the object inside the item box,
            if (InputManager.instance.G_L_DW && potentialHand.tag == "handLeft"
                || InputManager.instance.G_R_DW && potentialHand.tag == "handRight")
            {
                if (PhotonNetwork.InRoom)
                {
                    if (!PV.IsMine)
                    {
                        PV.RequestOwnership();
                    }
                }

                objective = potentialHand.transform;


                //potentialHand.GetComponent<HandGrabbing>().EnabledRender(false, transform);

                if (!objective.root.GetComponent<PlayerHealth>().dead)
                {
                    if (objective.tag == "handLeft")
                    {
                        objective.GetComponent<HandGrabbing>().EnabledRender(false);
                        objective.GetComponent<HandGrabbing>().isGrabbingSecondary = true;
                    }
                    else if (objective.tag == "handRight")
                    {
                        objective.GetComponent<HandGrabbing>().EnabledRender(false);
                        objective.GetComponent<HandGrabbing>().isGrabbingSecondary = true;
                    }
                    holding = true;
                }
                else
                {
                    objective= null;
                    holding = false;
                }

            }

            // if the user is picking the object inside the item box,
            if (InputManager.instance.G_L_UP && potentialHand.tag == "handLeft"
                || InputManager.instance.G_R_UP && potentialHand.tag == "handRight")
            {
                potentialHand.GetComponent<HandGrabbing>().EnabledRender(true);
                potentialHand.GetComponent<HandGrabbing>().isGrabbingSecondary = false;
                objective = null;
                Invoke("SetSpeed", 0.05f);

                holding = false;
            }
        }


        //distance cheker
        if (objective != null)
        {
            dist = (transform.position - objective.transform.position).magnitude;

            if(dist>distanceToRelease)
            {
                objective.GetComponent<HandGrabbing>().EnabledRender(true);
                objective.GetComponent<HandGrabbing>().isGrabbingSecondary = false;
                potentialHand = null;                   
                objective = null;
                holding = false;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if ((other.tag == "handLeft" || other.tag == "handRight") )
        {
            potentialHand = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (objective != null)
        {            
            if ((other.gameObject.tag == "handLeft" || other.gameObject.tag == "handRight"))
            {
                potentialHand = null;

                other.GetComponent<HandGrabbing>().EnabledRender(true);
                other.GetComponent<HandGrabbing>().isGrabbingSecondary=false;

                objective = null;
                Invoke("SetSpeed", 0.05f);

                holding = false;
            }
        }
        else if(potentialHand!=null)
        {
            if ((potentialHand.tag == other.tag || potentialHand.tag == other.tag))
            {
                other.GetComponent<HandGrabbing>().EnabledRender(true);
                other.GetComponent<HandGrabbing>().isGrabbingSecondary = false;
                potentialHand = null;

                holding = false;
            }
        }
    }

    

    public void SetSpeed()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
    }
}

