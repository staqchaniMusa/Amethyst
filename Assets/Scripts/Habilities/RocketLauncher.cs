using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [Header("Networking")]
    public bool isNetworkVisible = true;


    [Header("Shooting")]
    public Transform gunBarrel;
    public Material[] particleMaterials;
    public float fadingTime = 0.15f;
    public AudioClip triggerClip;
    public float sliderTime = 0.15f;
    public bool hasMissile=true;
    bool pressingTriggerCondition;
    PlayerHealth playerHealth;
    public GameObject shootObjective;
    public GameObject potentialShootObjective;

    public float timeToFocus=2.5f;
    public float elapsedFocus = 0;

    [Header("UI")]
    public GameObject reticle;

    [Header("GRabbing")]
    //[Header("Grabbing")]
    public ObjectGrabbing objectGrabbingScript;
    public bool isInHand = false;
    Vector3 dir;

    [Header("Secondary grabb")]
    public NonDominantGrab secondaryGrabb = null;

    [Header("Recoil")]
    public bool recoilOn = true;
    public float recoilFactorNomal = 0.07f;
    public float recoilFactorHold = 0.03f;
    bool isInRecoil = false;


    AudioSource audioSrc;
    private Rigidbody _rb;
    private Collider _col;

    PhotonView PV;


    // projections
    Vector3 vectorToProject;
    float projZ;
    float projY;
    float projX;


    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(null);
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        audioSrc = GetComponent<AudioSource>();

        objectGrabbingScript = GetComponent<ObjectGrabbing>();



        PV = GetComponent<PhotonView>();

        // projections
        Vector3 vectorToProject = (objectGrabbingScript.offsetR.position - transform.position);
        projZ = Vector3.Dot(vectorToProject, transform.forward);
        projY = Vector3.Dot(vectorToProject, transform.up);
        projX = Vector3.Dot(vectorToProject, transform.right);
    }



    // Update is called once per frame
    void Update()
    {
        //NEtworking visible
        if (!PV.IsMine && PhotonNetwork.InRoom)
        {
            if (!isNetworkVisible)
            {
                for (int ii = 0; ii < transform.childCount; ii++)
                {
                    transform.GetChild(ii).gameObject.SetActive(false);
                }
                _col.enabled = false;
            }
            else
            {
                for (int ii = 0; ii < transform.childCount; ii++)
                {
                    transform.GetChild(ii).gameObject.SetActive(true);
                }
                _col.enabled = true;
            }
        }


        //condition used for android
        pressingTriggerCondition = false;
       
        
        if (objectGrabbingScript.handGrabScp != null)
        {
            playerHealth = objectGrabbingScript.handGrabScp.transform.root.GetComponent<PlayerHealth>();


            isInHand = objectGrabbingScript.handGrabScp.objectInHand == gameObject;


          
            pressingTriggerCondition = isInHand
                                        && (InputManager.instance.T_R_DW && objectGrabbingScript.handGrabScp.CompareTag("handRight")
                                        || InputManager.instance.T_L_DW && objectGrabbingScript.handGrabScp.CompareTag("handLeft"));
          
           
        }
        else
        {
            isInHand = false;
        }



        //if there is second grabbing
        if (objectGrabbingScript.handGrabScp != null && secondaryGrabb != null)
        {

            if (objectGrabbingScript.handGrabScp.objectInHand == gameObject)
            {

                Vector3 destination = objectGrabbingScript.handGrabScp.otherHand.transform.position;
                Vector3 origin = objectGrabbingScript.handGrabScp.transform.position;

                dir = destination - objectGrabbingScript.handGrabScp.transform.position;




                if (objectGrabbingScript.handGrabScp.CompareTag("handLeft"))
                {
                    if (objectGrabbingScript.offsetL != null)
                    {

                        transform.position = origin - dir.normalized * projZ;
                    }

                }
                else if (objectGrabbingScript.handGrabScp.CompareTag("handRight"))
                {
                    if (objectGrabbingScript.offsetR != null)
                    {
                        transform.position = origin - dir.normalized * projZ; //-transform.up*projY-transform.right*projX;

                    }

                }

                transform.rotation = Quaternion.LookRotation(dir, -objectGrabbingScript.handGrabScp.transform.right);
            }
        }
        else
        {
            //transform.forward =;
        }



        // shooting logic
        if (pressingTriggerCondition && hasMissile && shootObjective!=null)
        {
            hasMissile = false;
                       
            ShootBullet();

            //vibration Steam VR example... not compatible with XR at the moment
            VibrationManager.VM.TriggerVibration(objectGrabbingScript.handGrabScp.tag );

        }



    }

    private void FixedUpdate()
    {

        // in this case the shooting can oly ocour while focusing on the vehicle (needs a raycast)
        RaycastHit rayCastHit;

        if (Physics.Raycast(gunBarrel.transform.position, gunBarrel.transform.forward, out rayCastHit, 5000))
        {
            if(potentialShootObjective!=rayCastHit.collider.transform.root.gameObject)
            {
                reticle.SetActive(false);
                shootObjective = null;
                elapsedFocus = 0;
                potentialShootObjective = rayCastHit.collider.transform.root.gameObject;

            }
            else
            {
                elapsedFocus += Time.fixedDeltaTime;
            }

            // focus and shoot            
            if(elapsedFocus>timeToFocus)
            {
                if (potentialShootObjective.tag == "tank" || potentialShootObjective.tag == "plane")
                {
                    reticle.SetActive(true);
                    shootObjective = potentialShootObjective;
                }
            }

        }

    }


    //function that creates the bullet
    public void ShootBullet()
    {
                /*create bullets NOW MANAGED FROM MULTIPLAYER
        GameObject bulletInstance = GameObject.Instantiate(shootingBullet);
        bulletInstance.transform.position = gunBarrel.position;
        bulletInstance.transform.forward = gunBarrel.forward;
        */
        //move sliding part of the gun
       

        if (recoilOn && isInRecoil == false)
        {
            StartCoroutine(RecoilMovement_co());
        }

        
        ShootingManager.SM.RocketLauncherShoot(gunBarrel.position, gunBarrel.rotation, PhotonNetwork.LocalPlayer, shootObjective.GetComponent<PhotonView>().ViewID);


        

    }

    /// <summary>
    /// recoil using corroutine
    /// </summary>
    /// <returns></returns>
    public IEnumerator RecoilMovement_co()
    {
        isInRecoil = true;

        Transform objectToMove1 = objectGrabbingScript.handGrabScp.transform.parent;
        Transform objectToMove2 = objectGrabbingScript.handGrabScp.otherHand.transform.parent;

        Vector3 original1 = objectToMove1.localPosition;
        Vector3 objective1 = new Vector3();


        Vector3 original2 = objectToMove2.localPosition;
        Vector3 objective2 = new Vector3();

        if (secondaryGrabb != null)
        {
            objective1 = objectToMove1.localPosition - recoilFactorHold * Vector3.forward;
            objective2 = objectToMove2.localPosition - recoilFactorHold * Vector3.forward;
        }
        else
        {
            objective1 = objectToMove1.localPosition - recoilFactorNomal * Vector3.forward;
            objective2 = objectToMove2.localPosition;
        }

        for (float ii = 0; ii < sliderTime / 2; ii += Time.deltaTime)
        {
            objectToMove1.localPosition = Vector3.Lerp(original1, objective1, 2 * ii / sliderTime);
            objectToMove2.localPosition = Vector3.Lerp(original2, objective2, 2 * ii / sliderTime);

            yield return new WaitForEndOfFrame();
        }
        objectToMove1.localPosition = objective1;
        objectToMove2.localPosition = objective2;


        for (float ii = 0; ii < sliderTime / 2; ii += Time.deltaTime)
        {
            objectToMove1.localPosition = Vector3.Lerp(objective1, original1, 2 * ii / sliderTime);
            objectToMove2.localPosition = Vector3.Lerp(objective2, original2, 2 * ii / sliderTime);

            yield return new WaitForEndOfFrame();
        }
        objectToMove1.localPosition = original1;
        objectToMove2.localPosition = original2;

        isInRecoil = false;
    }


    /// <summary>
    /// this is used to hide the OBEJCT in the network
    /// </summary>
    /// <param name="b"></param>
    public void SetNetWorkVisible(bool b)
    {
        PV.RPC("RPC_SetNetVis", RpcTarget.AllBuffered, b);
    }

    [PunRPC]
    public void RPC_SetNetVis(bool b)
    {
        isNetworkVisible = b;

    }


}
