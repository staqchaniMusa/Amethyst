using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DynamicRifle : MonoBehaviour
{

    [Header("Networking")]
    public bool isNetworkVisible = true;


    [Header("Weapon config")]
    public int weaponIndex = 0;
    

    [Header("Shooting")]
    public GameObject shootingBullet;
    public float timeBetweenBullets = 0f;
    public Transform gunBarrel;
    public Material[] particleMaterials;
    public float fadingTime = 0.15f;
    public AudioClip triggerClip;
    public Transform sliderPart;
    public float sliderTime = 0.1f;
    public bool isAuto = true;

    [Header("Bullets")]
    public int bulletInChamber = 0;
    public GameObject bulletCasePrefab;
    public GameObject bulletPrefab;
    public Transform expelTf;
    public float expelSpeed = 0.5f;
    public float randomDirection=0.001f;
    public float randomRotation=3.5f;

    [Header("Reloading")]
    public AudioClip reloadClip;
    public Magazine rifleMag = null;
    public Transform insertClipTf;
    

    [Header("GRabbing")]
    //[Header("Grabbing")]
    public ObjectGrabbing objectGrabbingScript;
    public bool isInHand=false;

    [Header("Secondary grabb")]
    public NonDominantGrab secondaryGrabb=null;

    [Header("Reset position")]
    public Transform resetPos;

    [Header("Handle/slider")]
    public RifleHandle handle;
    public bool sliderOnCorrutine = false;

    [Header("Recoil")]
    public bool recoilOn=true;
    public float recoilFactorNomal=0.07f;
    public float recoilFactorHold=0.03f;
    bool isInRecoil = false;

    float elapsed = 100;
    AudioSource audioSrc;
    private Rigidbody _rb;
    private Collider _col;

    [Header("Scopes")]
    public GameObject selectedScope;
    public Transform scopeTf;
    public GameObject[] scopes;
        


    bool pressingTriggerCondition;
    bool dettachCondition;

    // projections
    Vector3 vectorToProject;
    float projZ;
    float projY;
    float projX ;

    Vector3 dir;

    [Header("PlayerReference")]
    //public GameObject player;
    PlayerHealth playerHealth;

    PhotonView PV;

    void Awake()
    {
        isInRecoil = false;

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

    public void Update()
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


        elapsed += Time.deltaTime;

        //condition used for android
        pressingTriggerCondition = false;
        dettachCondition = false;

        
       

        if (objectGrabbingScript.handGrabScp != null)
        {
            playerHealth = objectGrabbingScript.handGrabScp.transform.root.GetComponent<PlayerHealth>();


            isInHand = objectGrabbingScript.handGrabScp.objectInHand == gameObject;


            if (isAuto)
            {
                pressingTriggerCondition = isInHand
                                           && (InputManager.instance.T_R && objectGrabbingScript.handGrabScp.CompareTag("handRight")
                                          || InputManager.instance.T_L && objectGrabbingScript.handGrabScp.CompareTag("handLeft"));
            }
            else
            {
                pressingTriggerCondition = isInHand
                                          && (InputManager.instance.T_R_DW && objectGrabbingScript.handGrabScp.CompareTag("handRight")
                                         || InputManager.instance.T_L_DW && objectGrabbingScript.handGrabScp.CompareTag("handLeft"));

            }

            dettachCondition = isInHand
                                       && (InputManager.instance.One_R_DW && objectGrabbingScript.handGrabScp.CompareTag("handRight")
                                      || InputManager.instance.One_L_DW && objectGrabbingScript.handGrabScp.CompareTag("handLeft"));

        }
        else
        {
            isInHand = false;
        }


        dir = gunBarrel.forward;
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


        if (bulletInChamber == 0 && !handle.moving)
        {
            //sliderPart.position = handle.newOpenPos;
        }
        else if (!handle.moving && sliderOnCorrutine == false)
        {
            //sliderPart.position = handle.newClosedPos;
        }


        if (dettachCondition && rifleMag != null)
        {
            rifleMag.Dettach();
            rifleMag = null;
        }


        // shooting logic
        if (pressingTriggerCondition && elapsed > timeBetweenBullets)
        {

            if (bulletInChamber == 1)
            {
                ShootBullet();

                //vibration Steam VR example... not compatible with XR at the moment
                VibrationManager.VM.TriggerVibration(objectGrabbingScript.handGrabScp.tag);

            }
            else
            {
                audioSrc.clip = triggerClip;
                if (audioSrc.isPlaying == false)
                {
                    audioSrc.Play();
                }
            }


            elapsed = 0;
        }





    }


    public void ShowScope(int a)
    {
        PV.RPC("RPC_ShowScope", RpcTarget.AllBuffered, a);
    }

    [PunRPC]
    public void RPC_ShowScope(int a)
    {
        scopes[a].SetActive(true);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("environment"))
        {
            if(resetPos==null)
            {
                return;
            }
      

            transform.position = resetPos.position;
            transform.rotation = resetPos.rotation;
            _rb.velocity = new Vector3(0, 0, 0);
            _rb.angularVelocity = new Vector3(0, 0, 0);
            _rb.useGravity = false;
            _rb.isKinematic = true;
            transform.SetParent(resetPos);

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
        handle.ShotSlider();

        if(recoilOn && isInRecoil==false)
        {
            StartCoroutine(RecoilMovement_co());
        }

        ShootingManager.SM.RifleShoot(gunBarrel.position, gunBarrel.rotation, PV.Owner);
              

        //reduce the amount of bullets
        if (rifleMag != null)
        {
            if (rifleMag.actualBullets > 0)
            {
                bulletInChamber = 1;
                rifleMag.actualBullets--;
            }
            else
            {
                bulletInChamber = 0;
            }
        }
        else
        {

            bulletInChamber = 0;
        }

    }


    public void Reload()
    {
        audioSrc.clip = reloadClip;
        audioSrc.Play();

    }

    //called after when slider is oppened
    public void FeedChamberNormal()
    {
        if (rifleMag != null)
        {
            if(bulletInChamber==0)
            {
                return;
            }
            else
            {
                handle.moving = false;
            }

            if (rifleMag.actualBullets > 0)
            {

                if (bulletInChamber == 0)
                {
                    bulletInChamber = 1;
                    rifleMag.actualBullets--;
                    //CloseSlider(); 
                }
                else
                {
                    //create bullets
                    GameObject bulletInstance = GameObject.Instantiate(bulletPrefab);
                    bulletInstance.transform.right = expelTf.right;
                    bulletInstance.transform.position = expelTf.position;
                    bulletInstance.GetComponent<Rigidbody>().velocity = expelTf.right * expelSpeed;

                    bulletInChamber = 1;
                    rifleMag.actualBullets--;
                }
            }
            else if (bulletInChamber == 1)
            {
                //create bullets
                GameObject bulletInstance = GameObject.Instantiate(bulletPrefab);
                bulletInstance.transform.right = expelTf.right;
                bulletInstance.transform.position = expelTf.position;
                bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.right * expelSpeed;

                bulletInChamber = 0;

            }
        }
    }

    //called at the first movement of the slider
    public void FeedChamberInitial()
    {
        if (rifleMag != null)
        {
            if (bulletInChamber > 0)
            {
                return;
            }
            else
            {
                handle.moving = false;
            }

            if (rifleMag.actualBullets > 0)
            {

                if (bulletInChamber == 0)
                {
                    bulletInChamber = 1;
                    rifleMag.actualBullets--;
                    //CloseSlider();
                }
                
            }
            
        }
    }


    public IEnumerator RecoilMovement_co()
    {
        isInRecoil = true;

        Transform objectToMove = objectGrabbingScript.handGrabScp.transform.parent;
   
        Vector3 original = objectToMove.localPosition;
        Vector3 objective=new Vector3();

        if (secondaryGrabb!=null)
        {
            objective = objectToMove.localPosition - recoilFactorHold * Vector3.forward; 

        }
        else
        {
            objective = objectToMove.localPosition - recoilFactorNomal * Vector3.forward; 
        }

        for (float ii = 0; ii < sliderTime / 2; ii += Time.deltaTime)
        {
            objectToMove.localPosition = Vector3.Lerp(original, objective, 2 * ii / sliderTime);

            yield return new WaitForEndOfFrame();
        }

        objectToMove.localPosition = objective;


        for (float ii = 0; ii < sliderTime / 2; ii += Time.deltaTime)
        {
            objectToMove.localPosition = Vector3.Lerp(objective, original, 2 * ii / sliderTime);
          

            yield return new WaitForEndOfFrame();
        }
        objectToMove.localPosition = original;


        isInRecoil = false;
    }



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
