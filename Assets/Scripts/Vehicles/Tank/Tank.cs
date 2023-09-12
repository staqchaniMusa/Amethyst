using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

/// <summary>
/// manages movement and healht of a tank
/// </summary>
public class Tank : MonoBehaviour
{

    [Header("movement")]
    public bool occupied;
    PhotonView PV;
    public GameObject playerGo;
    public Rigidbody _rb;
    public Transform xrReference;
    public Transform tankHead;
    Transform XRobject;
    public float speedF;
    public float speedRotY;
    public float acceleration = 1.5f;
    public float deceleration = 0.5f;

    public float maxSpeed = 5;

    [Header("Show")]
    public Renderer[] rends;

    public float quitDistance = 3;

    GameObject tempPlayer;
    public GameObject hudTank;
    AudioSource audioS;
    public AudioClip tankMovementSound;
    public AudioClip turretMovementSound;

    [Header("UI")]
    public Image[] lifeIm;
    public Text lifeTxt;
    public Text speedTxt;
    public Image timeShoting;

    [Header("Life management")]
    public int tankHp=3000;
    public int initialHp=3000;

    [Header("Recoil")]
    public bool recoilOn = true;
    public float recoilFactor= 0.4f;
    bool isInRecoil = false;
    public float sliderTime = 0.3f;

    [Header("shooting")]
    //public GameObject shootingHalo;
    //public
    GameObject shootingBullet;
    public float timeBetweenBullets = 0f;
    public Transform muzzle;
    float elapsed;
    public float minAngle=-120, maxAngle=-60;

    [Header("Destruction")]
    public bool destroyed;
    public GameObject smokeDeath;
    public Collider col;

    [Header("Inner part HUD")]
    public GameObject innerPart;

    // Start is called before the first frame update
    void Awake()
    {

        PV = GetComponent<PhotonView>();
        audioS = GetComponent<AudioSource>();
        hudTank.SetActive(false);

        smokeDeath.SetActive(false);

        tankHp = initialHp;
    }

    /// <summary>
    /// called when the button is pressed
    /// </summary>
    public void EnterVehicle()
    {
        if (!destroyed)
        {                

            tempPlayer = GameObject.FindGameObjectWithTag("XR");


            //the player goes inside the Vehicle
            GetPlayerInsideTank(tempPlayer);


            //the Vehicle is occupied
            PV.RPC("RPC_IsOcuppied", RpcTarget.AllBuffered, true);

            PV.RequestOwnership();
        }
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            _rb.useGravity = false;
        }

        elapsed += Time.fixedDeltaTime;
        timeShoting.fillAmount =elapsed/timeBetweenBullets;


        Vector3 delta = new Vector3(0, 0, 0);

        float deltaRotation =0;
        
        if (occupied && PV.IsMine && playerGo!=null )
        {
            innerPart.SetActive(true);

            //tank head rotation
            float deltaHeadZ = speedRotY * Time.deltaTime * (InputManager.instance.axisR.y );
                                   

            //apply rotation
            tankHead.Rotate(new Vector3(0,0,1),deltaHeadZ,Space.Self);

                      
            if ( Mathf.Abs(deltaHeadZ) > 0.01f && !audioS.isPlaying)
            {
                audioS.clip = turretMovementSound;
                audioS.Play();
            }

            //clamp
            float eulerZ = tankHead.localEulerAngles.z;
            if(eulerZ>180)
            {
                eulerZ -= 360;
            }
            
            tankHead.localEulerAngles= new Vector3(tankHead.localEulerAngles.x, tankHead.localEulerAngles.y, Mathf.Clamp(eulerZ, minAngle, maxAngle));



                //tank body movement
            deltaRotation = speedRotY * Time.deltaTime * (InputManager.instance.axisL.x );
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0, deltaRotation, 0));


            if (Mathf.Abs(deltaRotation)>0.01f && !audioS.isPlaying)
            {
                audioS.clip = tankMovementSound;
                audioS.Play();
            }

            if (Mathf.Abs(InputManager.instance.axisL.y) > 0.1f)
            {                
                speedF += Time.fixedDeltaTime * InputManager.instance.axisL.y*acceleration;

                if(Mathf.Sign(InputManager.instance.axisL.y*speedF)<0)
                {
                    speedF = Mathf.Lerp(speedF, 0, deceleration);
                }

            }
            else
            {
                speedF= Mathf.Lerp(speedF, 0, deceleration);
            }

            speedF = Mathf.Clamp(speedF, -maxSpeed, maxSpeed);



            delta = speedF * Time.deltaTime  * _rb.transform.forward;
            _rb.MovePosition(_rb.position + delta);


        }
        else
        {
            innerPart.SetActive(false) ;
        }


        speedTxt.text = Mathf.Round(10 * speedF * 3.6f) /10+" kph";
    }

    private void LateUpdate()
    {
        if (occupied && PV.IsMine && playerGo!=null)
        {
            playerGo.transform.position = _rb.transform.position;

            XRobject = Camera.main.transform.parent.parent.transform;
            //position tankHud by head
            XRobject.position= xrReference.position;
            XRobject.rotation=Quaternion.LookRotation(transform.forward, transform.up);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="XR" && occupied==false && destroyed==false)
        {
            tempPlayer = other.gameObject;
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "xR" && occupied == true)
        {
            //the player goes ouside the tank
            GetPlayerOutsideTank(other.gameObject);

            //the tank is occupied
            PV.RPC("RPC_IsOcuppied", RpcTarget.AllBuffered,false);

        }

    }

    private void Update()
    {
        if(InputManager.instance==null)
        {
            return;
        }
        
       
        // shooting logic
        if ((InputManager.instance.T_R|| Input.GetMouseButton(0)) && elapsed > timeBetweenBullets && occupied == true && PV.IsMine)
        {           
                Shoot();

                elapsed = 0;
            
        }

        //hud life of the tank
        float lifeValue = (float)tankHp / (float)initialHp;
        for (int ii = 0; ii < lifeIm.Length; ii++)
        {
            lifeIm[ii].fillAmount = lifeValue;
        }
        lifeTxt.text = ""+Mathf.Round(100* lifeValue)+"%";


        if (tankHp<=0 && destroyed==false)
        {
            destroyed = true;

            col.enabled = false;

            //destroy the tank if it is mine and send quit messages
            if (PV.IsMine)
            {
                PV.RPC("RPC_QuitAllPlayersInTankAndDestroy", RpcTarget.AllBuffered);

            }

        }

    }

    /// <summary>
    /// creating the bullets
    /// </summary>
    public void Shoot()
    {
        if (recoilOn && isInRecoil == false)
        {
            StartCoroutine(RecoilMovement_co());
        }

        ShootingManager.SM.TankShoot(muzzle.position, muzzle.rotation, PV.Owner);
    }

    /// <summary>
    /// recoil using coroutine
    /// </summary>
    /// <returns></returns>
    public IEnumerator RecoilMovement_co()
    {
        isInRecoil = true;

        Transform objectToMove = tankHead;
        
        Vector3 original = tankHead.localPosition;
        Vector3 objective = objectToMove.localPosition - recoilFactor * new Vector3(0,-1,0);
        
        
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


    /// <summary>
    /// Perform all the actions needed when the player enter the tank
    /// </summary>
    /// <param name="player"></param>
    public void GetPlayerInsideTank(GameObject player)
    {
        playerGo = player;

        AvatarSetUp avtScript= player.transform.root.GetComponent<AvatarSetUp>();

        avtScript.InsideVehicle();

        hudTank.SetActive(true);

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = _rb.transform.position;

        for (int ii=0;ii< rends.Length;ii++)
        {
            rends[ii].enabled = false;
        }

       
    }

    // exit the vehicle
    public void QuitVehicle()
    {
        if (occupied == true && PV.IsMine)
        {
            occupied = false;
            playerGo.transform.position = _rb.transform.position + _rb.transform.right * quitDistance;

            GetPlayerOutsideTank(playerGo);

            PV.RPC("RPC_IsOcuppied", RpcTarget.AllBuffered, false);
        }
    }


    /// <summary>
    /// perform the actions needed 
    /// </summary>
    /// <param name="player"></param>
    public void GetPlayerOutsideTank(GameObject player)
    {


        hudTank.SetActive(false);

        AvatarSetUp avtScript = player.transform.root.GetComponent<AvatarSetUp>();

        avtScript.OutsideVehicle();
        

        player.GetComponent<CharacterController>().enabled = true;

        for (int ii = 0; ii < rends.Length; ii++)
        {
            rends[ii].enabled = true;
        }

        
        playerGo = null;

    }

    [PunRPC]
    public void RPC_IsOcuppied(bool b)
    {
        occupied = b;
    }


    [PunRPC]
    public void RPC_QuitAllPlayersInTankAndDestroy()
    {
        destroyed = true;
        smokeDeath.SetActive(true);
        col.enabled = false;
        if (PV.IsMine && playerGo != null)
        {
            playerGo.transform.position = _rb.transform.position + _rb.transform.right * quitDistance;
        }

        if (occupied && playerGo!=null)
        {
            hudTank.SetActive(false);

            AvatarSetUp avtScript = playerGo.transform.root.GetComponent<AvatarSetUp>();

            avtScript.OutsideVehicle();

            playerGo.GetComponent<CharacterController>().enabled = true;

            for (int ii = 0; ii < rends.Length; ii++)
            {
                rends[ii].enabled = true;
            }

            
            playerGo = null;
        }
    }





    public float ClampAngle(float angle, float min, float max )
    {
 
         if (angle<90 || angle>270)
         {       // if angle in the critic region...
             if (angle>180) angle -= 360;  // convert all angles to -180..+180
             if (max>180) max -= 360;
             if (min>180) min -= 360;
         }

         angle = Mathf.Clamp(angle, min, max);
         if (angle<0) angle += 360;  // if angle negative, convert to 0..360

        return angle;
    }
 
    /// <summary>
    /// called when the 
    /// </summary>
    /// <param name="damage"></param>
    public void GetHit(float damage)
    {
        tankHp -= (int)damage;

    }
}
