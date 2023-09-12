using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// shot gun gun type
/// </summary>
public class ShotGun : MonoBehaviour
{
    [Header("Networking")]
    public bool isNetworkVisible = true;

    [Header("Shooting")]
    public GameObject shootingBullet;
    public float timeBetweenBullets = 0f;
    public Transform gunBarrel;
    public AudioClip triggerClip;
    
    [Header("Bullets")]
    public int bulletChamber = 0;
    public int bulletsLoaded = 0;
    public GameObject bulletCasePrefab;
    public GameObject bulletPrefab;
    public Transform expelTf;
    public float expelSpeed = 0.5f;
    public float randomDirection = 0.001f;
    public float randomRotation = 3.5f;

    [Header("Reloading")]
    public AudioClip reloadClip;
    public Transform insertClipTf;
    public SliderSecondGrab sliderPart;

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

    [Header("PlayerReference")]
    PlayerHealth playerHealth;

    [Header("Recoil")]
    public bool recoilOn = true;
    public float recoilFactorNomal = 0.3f;
    public float recoilFactorHold = 0.1f;
    bool isInRecoil = false;
    public float recoilTime=0.2f;

    PhotonView PV;


    // projections
    Vector3 vectorToProject;
    float projZ;
    float projY;
    float projX;
    Vector3 dir;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        PV = GetComponent<PhotonView>();

        audioSrc = GetComponent<AudioSource>();

        objectGrabbingScript = GetComponent<ObjectGrabbing>();

        // projections
        Vector3 vectorToProject = (objectGrabbingScript.offsetR.position - transform.position);
        projZ = Vector3.Dot(vectorToProject, transform.forward);
        projY = Vector3.Dot(vectorToProject, transform.up);
        projX = Vector3.Dot(vectorToProject, transform.right);

    }

    public void Update()
    {

        //NEtworking visible
        if (!PV.IsMine)
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

        if (objectGrabbingScript.handGrabScp != null)
        {
            playerHealth = objectGrabbingScript.handGrabScp.transform.root.GetComponent<PlayerHealth>();


            isInHand = objectGrabbingScript.handGrabScp.objectInHand == gameObject;

            pressingTriggerCondition = playerHealth.health > 0 && isInHand
                                       && (InputManager.instance.T_R_DW && objectGrabbingScript.handGrabScp.CompareTag("handRight")
                                      || InputManager.instance.T_L_DW && objectGrabbingScript.handGrabScp.CompareTag("handLeft"));

         
        }
        else
        {
            isInHand = false;
        }

        dir = gunBarrel.forward;

        //if there is second grabbing
        if (objectGrabbingScript.handGrabScp != null && sliderPart != null)
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

        }






        // shooting logic
        if (pressingTriggerCondition && elapsed > timeBetweenBullets)
        {

            if (bulletChamber == 1)
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


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("environment"))
        {


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

        //create bullets
        GameObject bulletInstance = GameObject.Instantiate(bulletPrefab);
        bulletInstance.transform.right = expelTf.right;
        bulletInstance.transform.position = expelTf.position;
        bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.right * expelSpeed;
        bulletChamber = 0;

        ShootingManager.SM.GunShootGun(gunBarrel.position, gunBarrel.rotation, PV.Owner);


        //reduce the amount of bullets
        if (bulletChamber>0)
        {
            bulletChamber=0;
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

        bulletsLoaded++;

    }

    //called after when slider is oppened
    public void FeedChamberNormal()
    {

        if (bulletsLoaded >= 0)
        {
            if (bulletChamber==1)
            {
                //create bullets
                GameObject bulletInstance = GameObject.Instantiate(bulletPrefab);
                bulletInstance.transform.right = expelTf.right;
                bulletInstance.transform.position = expelTf.position;
                bulletInstance.GetComponent<Rigidbody>().velocity = bulletInstance.transform.right * expelSpeed;
                bulletChamber = 0;

            }
            

            if (bulletsLoaded > 0)
            {
                bulletChamber = 1;
                bulletsLoaded--;
                
            }
        }
    }
      

  

    public IEnumerator RecoilMovement_co()
    {
        isInRecoil = true;

        Transform objectToMove = objectGrabbingScript.handGrabScp.transform.parent;

        Vector3 original = objectToMove.localPosition;
        Vector3 objective = new Vector3();



        objective = objectToMove.localPosition - recoilFactorHold * Vector3.forward; 


        for (float ii = 0; ii < recoilTime / 2; ii += Time.deltaTime)
        {
            objectToMove.localPosition = Vector3.Lerp(original, objective, 2 * ii / recoilTime);

            yield return new WaitForEndOfFrame();
        }
        objectToMove.localPosition = objective;


        for (float ii = 0; ii < recoilTime / 2; ii += Time.deltaTime)
        {
            objectToMove.localPosition = Vector3.Lerp(objective, original, 2 * ii / recoilTime);

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
