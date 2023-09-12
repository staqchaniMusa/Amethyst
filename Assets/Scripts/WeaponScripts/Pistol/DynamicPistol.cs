using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class DynamicPistol : MonoBehaviour
{

    [Header("Networking")]
    public bool isNetworkVisible = true;

    [Header("Shooting")]
    public GameObject shootingBullet;
    public float timeBetweenBullets = 0f;
    public Transform gunBarrel;
    public AudioClip triggerClip;
    public Transform sliderPart;
    public float sliderTime = 0.1f;
    public Transform closedSlider, openSlider;
 
 
    [Header("Bullets")]
    public int bulletInChamber=0;
    public GameObject bulletCasePrefab;
    public GameObject bulletPrefab;
    public Transform expelTf;
    public float expelSpeed = 0.5f;
    public float randomDirection = 0.001f;
    public float randomRotation = 3.5f;

    [Header("Reloading")]
    public AudioClip reloadClip;
    public Magazine pistolClipScp=null;
    public Transform insertClipTf;
    
    [Header("GRabbing")]
    //[Header("Grabbing")]
    public ObjectGrabbing objectGrabbingScript;
    public bool isInHand = false;

    [Header("Reset position")]
    public Transform resetPos;

    float elapsed = 100;
    AudioSource audioSrc;
    private Rigidbody _rb;
    private Collider _col;


    bool pressingTriggerCondition;
    bool dettachCondition;

    [Header("PlayerReference")]
    PlayerHealth playerHealth;

    [Header("Handle/slider")]
    public bool sliderOnCorrutine = false;
    public TopPistol topPistol;

    [Header("Recoil")]
    public bool recoilOn = true;
    public float recoilFactorNomal = 0.07f;
    public float recoilFactorHold = 0.03f;
    bool isInRecoil = false;

    PhotonView PV;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        PV = GetComponent<PhotonView>();

        audioSrc = GetComponent<AudioSource>();

        objectGrabbingScript = GetComponent<ObjectGrabbing>();

        
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

            pressingTriggerCondition = playerHealth.health > 0 && isInHand
                                       && (InputManager.instance.T_R_DW && objectGrabbingScript.handGrabScp.CompareTag("handRight")
                                      || InputManager.instance.T_L_DW && objectGrabbingScript.handGrabScp.CompareTag("handLeft"));

            dettachCondition = isInHand
                                       && (InputManager.instance.One_R_DW && objectGrabbingScript.handGrabScp.CompareTag("handRight")
                                      || InputManager.instance.One_L_DW && objectGrabbingScript.handGrabScp.CompareTag("handLeft"));

        }
        else
        {
            isInHand = false;
        }



        // shooting logic
        if (pressingTriggerCondition && elapsed > timeBetweenBullets )
        {
           
            if (bulletInChamber==1)
            {
                ShootBullet();
                
                //vibration Steam VR
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

    // Update is called once per frame
    void LateUpdate()
    {

        if (PV.IsMine)
        {
            
            if (dettachCondition && pistolClipScp != null)
            {
                pistolClipScp.Dettach();
                pistolClipScp = null;
            }
        }
        else
        {
            sliderPart.position = closedSlider.position;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("environment"))
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

        //create bullets MANAGED IN THE MULTIPLAYER NOW
        /*GameObject bulletInstance = GameObject.Instantiate(shootingBullet);
        bulletInstance.transform.position = gunBarrel.position;
        bulletInstance.transform.forward = gunBarrel.forward;*/

        if (recoilOn && isInRecoil == false)
        {
            StartCoroutine(RecoilMovement_co());
        }

        ShootingManager.SM.GunShoot(gunBarrel.position, gunBarrel.rotation, PV.Owner);



        //disable effects again
        //StartCoroutine(DisableHalo());

        //move sliding part of the gun
        StartCoroutine(MoveSlider());

        //reduce the amount of bullets
        if (pistolClipScp!=null)
        {
            if (pistolClipScp.actualBullets > 0)
            {
                bulletInChamber = 1;
                pistolClipScp.actualBullets--;
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

    //old function
    /*public IEnumerator DisableHalo()
    {
        for (float ii = 0; ii < fadingTime; ii += Time.deltaTime)
        {

            foreach (Material mat in particleMaterials)
            {
                mat.SetColor("_TintColor", new Color(1, 1, 1, 1 - ii / fadingTime));
            }
            yield return new WaitForEndOfFrame();
        }

        shootingHalo.SetActive(false);
    }*/

    public void Reload()
    {
        audioSrc.clip = reloadClip;
        audioSrc.Play();

    }

    //called after when slider is oppened
    public void FeedChamberNormal()
    {
        if (pistolClipScp != null)
        {
            if (bulletInChamber == 0)
            {
                return;
            }
            else
            {
                topPistol.moving = false;
            }

            if (pistolClipScp.actualBullets > 0)
            {

                if (bulletInChamber == 0)
                {
                    bulletInChamber = 1;
                    pistolClipScp.actualBullets--;
                    CloseSlider();
                }
                else
                {

                    //create bullets
                    GameObject bulletInstance = GameObject.Instantiate(bulletPrefab);
                    bulletInstance.transform.right = expelTf.right;
                    bulletInstance.transform.position = expelTf.position;
                    bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.right * expelSpeed;

                    bulletInChamber = 1;
                    pistolClipScp.actualBullets--;
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

    public IEnumerator MoveSlider()
    {
        sliderOnCorrutine = true;

        for (float ii = 0; ii < sliderTime/2; ii += Time.deltaTime)
        {
            sliderPart.position= Vector3.Lerp(closedSlider.position,openSlider.position,2*ii/sliderTime);
            
            yield return new WaitForEndOfFrame();
        }
        sliderPart.position = openSlider.position;

        //bullet
        GameObject bulletInstance = Instantiate(bulletCasePrefab);
        bulletInstance.transform.position = expelTf.position;
        bulletInstance.transform.forward = expelTf.forward;
        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.right* expelSpeed;
        

        for (float ii = 0; ii < sliderTime/2; ii += Time.deltaTime)
        {
            sliderPart.position = Vector3.Lerp(openSlider.position, closedSlider.position, 2*ii / sliderTime);

            yield return new WaitForEndOfFrame();
        }

        sliderPart.position=closedSlider.position;

        sliderOnCorrutine = false;
    }

    public void CloseSlider()
    {
        sliderPart.position = closedSlider.position;
    }



    //called at the first movement of the slider
    public void FeedChamberInitial()
    {
        if (pistolClipScp != null)
        {
            if (bulletInChamber > 0)
            {
                return;
            }
            else
            {
                topPistol.moving = false;
            }

            if (pistolClipScp.actualBullets > 0)
            {

                if (bulletInChamber == 0)
                {
                    bulletInChamber = 1;
                    pistolClipScp.actualBullets--;
                    CloseSlider();
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


       
        objective = objectToMove.localPosition - recoilFactorHold * new Vector3(0,0,1);
        
      
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
