using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attached to the hands. Used to grab objects
/// </summary>
public class HandGrabbing : MonoBehaviour
{
    //public OVRInput.Controller controller;

    // Start is called before the first frame update
    [Header("Grabbing")]
    public bool isGrabbingSecondary=false;
    public GameObject objectInHand=null;
    public List<GameObject> potentialOnjectInHand=null;
    //public float filterValue = 0.05f;
    //public Transform parentHand;
    //public Transform controllerAnchor;
    
    public Vector3 filteredSpeed;
    public float filterValue = 0.1f;
    private Vector3 speed, pos_i, pos_i_1;

    public Vector3 filteredAngularSpeed;
    private Quaternion filteredQuat;
    private Quaternion angularSpeed, rot_i,rot_i_1;

    [Header("Other hand")]
    public HandGrabbing otherHand;
    [Header("This renderer")]
    public Renderer rend;
    [Header("Watch/PDA")]
    public GameObject watch;
    [Header("This animator")]
    public Animator animT;
    AudioSource audioS;

    Coroutine corr;
    


    private void Awake()
    {
        potentialOnjectInHand = new List<GameObject>();
    }

    void Start()
    {
        audioS = GetComponent<AudioSource>();
        rot_i = transform.rotation;
        rot_i_1 = transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {
      
        if(potentialOnjectInHand.Count>0)
        {
            //clean null elements
            bool hasNull = true;
            while(hasNull)
            {
                int indx = -1;
                for (int ii = 0; ii < potentialOnjectInHand.Count; ii++)
                {
                    if (potentialOnjectInHand[ii] == null)
                    {
                        indx = ii;
                    }

                }

                if(indx>=0)
                {
                    potentialOnjectInHand.RemoveAt(indx);
                    hasNull = true;
                }
                else
                {
                    hasNull = false;
                }
            }

          



            //find nearest gameobject
            float min = 10000000;
            int indxNearest = -1;
            
            for(int ii=0;ii<potentialOnjectInHand.Count;ii++)
            {
                if (potentialOnjectInHand[ii]!=null)
                {
                    if ((potentialOnjectInHand[ii].transform.position - transform.position).magnitude < min)
                    {
                        indxNearest = ii;
                    }
                }

            }
            ObjectGrabbing objScp = potentialOnjectInHand[indxNearest].GetComponent<ObjectGrabbing>();

            //if ( OVRInput.GetDown(grabButton) && objScp.isGrabbable)

            bool grabCondition = false;

            if (gameObject.tag == "handLeft")
            {
                grabCondition= InputManager.instance.G_L_DW && otherHand.objectInHand != potentialOnjectInHand[indxNearest];
            }
            else if (gameObject.tag == "handRight")
            {
                grabCondition = InputManager.instance.G_R_DW && otherHand.objectInHand != potentialOnjectInHand[indxNearest];
            }




            if (grabCondition)
            {
                audioS.Play();
                animT.SetInteger("Pose", objScp.pose);

                objScp.SetRendHandEnabled(true, gameObject.tag);
                rend.enabled = false;
                if(watch!=null)
                {
                    watch.SetActive(false);
                }

                objectInHand = potentialOnjectInHand[indxNearest];

                if(objectInHand.GetComponent<PhotonView>())
                {
                    objectInHand.GetComponent<PhotonView>().RequestOwnership();
                    
                    //this is used to show the hand
                    if (corr == null)
                    {
                        corr = StartCoroutine(SetHandsAfterPhotonRequest(gameObject.tag, objScp));
                    }
                }                

                objScp.RigidBodyToKn();

                //_col.enabled = false;
                objectInHand.layer = (gameObject.layer);

                SetParent();
                objScp.handGrabScp = this;

                //DebugCanvas.DC.Log(objScp.gameObject.name + "-- pick--> " +gameObject.name);

                
            }

        }


        if(objectInHand!=null)
        {
            rend.enabled = false;

            ObjectGrabbing objHandScp = objectInHand.GetComponent<ObjectGrabbing>();

            //parenting for photon
            objectInHand.transform.SetParent(transform);

            objectInHand.transform.position=transform.position;
            objectInHand.transform.rotation = transform.rotation;

            //offset
            if (gameObject.CompareTag("handLeft"))
            {
                if(objHandScp.offsetL!=null)
                {
                    objectInHand.transform.localPosition = new Vector3(-objHandScp.offsetL.localPosition.x * objHandScp.offsetL.parent.localScale.x,
                                                                       -objHandScp.offsetL.localPosition.y * objHandScp.offsetL.parent.localScale.y,
                                                                       -objHandScp.offsetL.localPosition.z * objHandScp.offsetL.parent.localScale.z
                                                            );
                    objectInHand.transform.localRotation = objHandScp.offsetL.localRotation;


                }
                
            }
            else if (gameObject.CompareTag("handRight"))
            {
                if (objHandScp.offsetR != null)
                {
                    objectInHand.transform.localPosition =new Vector3( -objHandScp.offsetR.localPosition.x* objHandScp.offsetR.parent.localScale.x,
                                                                       -objHandScp.offsetR.localPosition.y * objHandScp.offsetR.parent.localScale.y,
                                                                       -objHandScp.offsetR.localPosition.z * objHandScp.offsetR.parent.localScale.z
                                                            );
                    objectInHand.transform.localRotation = objHandScp.offsetR.localRotation;
                }
            }

            bool releaseCondition = false;
            if (gameObject.tag == "handLeft")
            {
                releaseCondition = InputManager.instance.G_L_UP;
            }
            else if (gameObject.tag == "handRight")
            {
                releaseCondition = InputManager.instance.G_R_UP;
            }

            //release actions
            if (releaseCondition)
            {
                potentialOnjectInHand.Clear();
                otherHand.potentialOnjectInHand.Clear();

                otherHand.animT.SetInteger("Pose", 0);
                objHandScp.SetRendHandEnabled(false, gameObject.tag);
                rend.enabled = true;

                if (watch != null)
                {
                    watch.SetActive(true);
                }

                objHandScp.Release();

                //DebugCanvas.DC.Log(objHandScp.gameObject.name + "-- rlse --> " + gameObject.name);

                objectInHand = null;
            }

            //parenting for photon
            if (objectInHand != null)
            {
                objectInHand.transform.SetParent(null);
            }
        }
        else
        {
            if (isGrabbingSecondary==false)
            {
                rend.enabled = true;
                if(watch)
                {
                    watch.SetActive(true);
                }
            }
            else
            {
                rend.enabled = false;
                if (watch)
                {
                    watch.SetActive(false);
                }
            }


        }


              

    }

    public void FixedUpdate()
    {
      

        //obtain the speed of the hand
        pos_i = transform.position;

        speed = ( pos_i- pos_i_1) / Time.fixedDeltaTime;

        filteredSpeed = Vector3.Lerp(filteredSpeed, speed, filterValue);

        pos_i_1 = pos_i;

        //obtain the angular speed
        rot_i = transform.rotation;
                
        filteredQuat =Quaternion.Lerp( filteredQuat, rot_i * Quaternion.Inverse(rot_i_1), filterValue);

        filteredAngularSpeed = filteredQuat.eulerAngles;

        //recalculate angles if bigger than 180
        float angleX, angleY, angleZ;
        angleX = filteredAngularSpeed.x;
        angleY = filteredAngularSpeed.y;
        angleZ = filteredAngularSpeed.z;


        if (angleX > 180)
        {
            angleX -= 360;
        }
        if (angleY > 180)
        {
            angleY -= 360;
        }
        if (angleZ > 180)
        {
            angleZ -= 360;
        }

        filteredAngularSpeed = new Vector3(angleX,angleY,angleZ)/Time.fixedDeltaTime;

        rot_i_1 = rot_i;
    }

    public void SetParent()
    {
        objectInHand.transform.SetParent(transform);

    }

    public void EnabledRender(bool b)
    {
        rend.enabled = b;
    }

    public IEnumerator SetHandsAfterPhotonRequest(string hand, ObjectGrabbing scp)
    {
        while (!transform.root.GetComponent<PhotonView>().IsMine)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        scp.SetRendHandEnabled(true, gameObject.tag);

        corr = null;

    }

    public void GrabCustomObject(GameObject obj, string name)
    {

        ObjectGrabbing objScp = obj.GetComponent<ObjectGrabbing>();
        objScp.SetRendHandEnabled(true,name);

        objectInHand = obj;
                
        objScp.RigidBodyToKn();

        //_col.enabled = false;
        objectInHand.layer = (gameObject.layer);

        SetParent();
        objScp.handGrabScp = this;
    }

}
