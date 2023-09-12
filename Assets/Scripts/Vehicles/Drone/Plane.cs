using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Plane : MonoBehaviour
{
    [Header("COM")]
    public Transform _COM;

    [Header("movement")]
    public bool occupied;
    PhotonView PV;
    public GameObject playerGo;
    public Rigidbody _rb;

    public float rotationCoef=5f;
    public float acceleration = 1.5f;
    public float deceleration = 15;
    public float speedF;
    public float speedR;
    public float speedUp;
    public float speedRotY;
    public float maxSpeed = 3.5f;
    float Yangle;

    Transform XRobject;
    [Header("Show")]
    public Renderer[] rends;
    public Transform xrReference;

    public float quitDistance = 3;

    public GameObject droneHud;
    AudioSource audioS;
    public AudioClip droneMovementSound;

    [Header("Limits")]
    public float maxHeight;
    public float minHeight;

    [Header("UI")]
    public Image[] lifeIm;
    public Text lifeTxt;
    public Text speedTxt;
    public Image timeShoting;

    [Header("Life management")]
    public int droneHp=3000;
    public int initialHp=3000;

    GameObject tempPlayer;


    [Header("shooting")]
    //public GameObject shootingHalo;
    //public
    GameObject shootingBullet;
    public float timeBetweenBullets = 0f;
    public float timeLoadMissiles = 5;
    int nbMissiles = 5;
    int nbMisilesStart;

    public Transform[] muzzle;
    float elapsed;
    public float minAngle=-120, maxAngle=-60;
    int lefright;

    [Header("Destruction")]
    public bool destroyed;
    public GameObject smokeDeath;
    public Collider col;

    [Header("Inner part HUD")]
    public GameObject innerPart;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        nbMisilesStart = nbMissiles;
        PV = GetComponent<PhotonView>();
        audioS = GetComponent<AudioSource>();
        droneHud.SetActive(false);

        smokeDeath.SetActive(false);

        droneHp = initialHp;
        
    

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!PV.IsMine)
        {
            _rb.useGravity = false;
        }


        elapsed += Time.fixedDeltaTime;
        timeShoting.fillAmount = elapsed / timeLoadMissiles;
        
        if(elapsed>timeLoadMissiles)
        {
            nbMissiles = nbMisilesStart;
        }

        Vector3 delta = new Vector3(0, 0, 0);

        float deltaRotation =0;
        
        if (occupied && PV.IsMine && playerGo != null)
        {
            _rb.useGravity = false;
            innerPart.SetActive(true);

            //Vehicle body movement
            deltaRotation = speedRotY * Time.deltaTime * (InputManager.instance.axisR.x );
            Yangle += deltaRotation;
            //_rb.MoveRotation(_rb.rotation * Quaternion.Euler(0, deltaRotation,0));


            /*
            Vector3 eulers = _rb.transform.eulerAngles;
            eulers.x =0;
            eulers.y = 0;
            _rb.transform.eulerAngles = eulers;
            */

            if (Mathf.Abs(deltaRotation)>0.01f && !audioS.isPlaying)
            {
                audioS.clip = droneMovementSound;
                audioS.Play();
            }


            if (Mathf.Abs(InputManager.instance.axisL.y) > 0.1f)
            {
                speedF += Time.fixedDeltaTime * InputManager.instance.axisL.y * acceleration;
            }
            else
            {
                speedF = Mathf.Lerp(speedF, 0, deceleration);
            }
            speedF = Mathf.Clamp(speedF, -maxSpeed, maxSpeed);


            if (Mathf.Abs(InputManager.instance.axisL.x) > 0.1f)
            {
                speedR += Time.fixedDeltaTime * InputManager.instance.axisL.x * acceleration;
            }
            else
            {
                speedR = Mathf.Lerp(speedR, 0, deceleration);
            }
            speedR = Mathf.Clamp(speedR, -maxSpeed, maxSpeed);

            if (Mathf.Abs(InputManager.instance.axisR.y)>0.1f)
            {
                speedUp += Time.fixedDeltaTime * InputManager.instance.axisR.y * acceleration;
            }
            else
            {
                speedUp = Mathf.Lerp(speedUp,0,deceleration);
            }
            speedUp = Mathf.Clamp(speedUp, -maxSpeed, maxSpeed);

            //clamp max min height
            if(transform.position.y>maxHeight)
            {
                speedUp = Mathf.Clamp(speedUp, -maxSpeed, 0);
            }
            else if(transform.position.y<minHeight)
            {
                speedUp = Mathf.Clamp(speedUp, 0, maxSpeed);
            }

            //delta movement
            delta = speedF * Time.deltaTime * new Vector3(_rb.transform.forward.x, 0, _rb.transform.forward.z)
                + speedR * Time.deltaTime * new Vector3(_rb.transform.right.x, 0, _rb.transform.right.z)
                + speedUp * Time.deltaTime * new Vector3(0, 1, 0);

            _rb.rotation = Quaternion.Euler(rotationCoef * speedF, Yangle, -rotationCoef * speedR);

            _rb.MovePosition(_rb.position + delta);
                                    


        }
        else
        {
            _rb.useGravity = true;
            innerPart.SetActive(false);
        }
        
        speedTxt.text = Mathf.Round(10 * new Vector3(speedF,speedR,speedUp).magnitude* 3.6f) / 10 + " kph";
    }

    private void LateUpdate()
    {
        if (occupied && PV.IsMine && playerGo != null)
        {
            playerGo.transform.position = _rb.transform.position;

            XRobject = Camera.main.transform.parent.parent.transform;
            //position tankHud by head
            XRobject.position = xrReference.position;
            XRobject.rotation = Quaternion.LookRotation(transform.forward, transform.up);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="XR" && occupied==false && destroyed==false)
        {
            Yangle = _rb.transform.rotation.eulerAngles.y;

            tempPlayer = other.gameObject;
        }

    }

    private void Start()
    {
        _rb.centerOfMass = _COM.localPosition;      

    }

    /// <summary>
    /// called from button
    /// </summary>
    public void EnterVehicle()
    {
        if (!destroyed)
        {
            tempPlayer = GameObject.FindGameObjectWithTag("XR");


            //the player goes inside the Vehicle
            GetPlayerInsideVehicle(tempPlayer);


            //the Vehicle is occupied
            PV.RPC("RPC_IsOcuppied", RpcTarget.AllBuffered, true);

            PV.RequestOwnership();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "XR" && occupied == true)
        {

            //the player goes ouside the Vehicle
            GetPlayerOutsideVehicle(other.gameObject);

            //the Vehicle is occupied
            PV.RPC("RPC_IsOcuppied", RpcTarget.AllBuffered,false);

        }

    }


    /// <summary>
    /// called from button
    /// </summary>
    public void QuitVehicle()
    {
        if ( occupied == true && PV.IsMine)
        {

            occupied = false;
            playerGo.transform.position = _rb.transform.position + _rb.transform.right * quitDistance;

            GetPlayerOutsideVehicle(playerGo);

            PV.RPC("RPC_IsOcuppied", RpcTarget.AllBuffered, false);
        }
    }

    private void Update()
    {           

        // shooting logic
        if ((InputManager.instance.T_R|| Input.GetMouseButton(0))
            && elapsed > timeBetweenBullets && occupied == true && PV.IsMine
            && nbMissiles>0)
        {           
            Shoot();
            nbMissiles -= 1;

            elapsed = 0;
            
        }



        //hud life of the Vehicle
        float lifeValue = (float)droneHp / (float)initialHp;
        for (int ii = 0; ii < lifeIm.Length; ii++)
        {
            lifeIm[ii].fillAmount = lifeValue;
        }
        lifeTxt.text = ""+Mathf.Round(100* lifeValue)+"%";


        if (droneHp <= 0 && destroyed==false)
        {
            destroyed = true;

            //destroy the Vehicle if it is mine and send quit messages
            if(PV.IsMine)
            {
                PV.RPC("RPC_QuitAllPlayersInVehicleAndDestroy", RpcTarget.AllBuffered);

            }

        }

    }
    //shoot logic
    public void Shoot()
    {
        if(lefright==0)
        {
            lefright = 1;
            ShootingManager.SM.PlaneShoot(muzzle[0].position, muzzle[0].rotation, PV.Owner);
        }
        else
        {
            lefright = 0;
            ShootingManager.SM.PlaneShoot(muzzle[1].position, muzzle[1].rotation, PV.Owner);

        }

        
    }
    /// <summary>
    /// called when entering the vehicle
    /// </summary>
    /// <param name="player"></param>
    public void GetPlayerInsideVehicle(GameObject player)
    {
        playerGo = player;

        AvatarSetUp avtScript= player.transform.root.GetComponent<AvatarSetUp>();

        avtScript.InsideVehicle();

        droneHud.SetActive(true);

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = _rb.transform.position;

        for (int ii=0;ii< rends.Length;ii++)
        {
            rends[ii].enabled = false;
        }

      
    }

    /// <summary>
    /// called when exit the vehicle
    /// </summary>
    /// <param name="player"></param>
    public void GetPlayerOutsideVehicle(GameObject player)
    {

        droneHud.SetActive(false);

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
    public void RPC_QuitAllPlayersInVehicleAndDestroy()
    {
        destroyed = true;
        smokeDeath.SetActive(true);
        col.enabled = false;
        if (PV.IsMine && playerGo!=null)
        {
            playerGo.transform.position = _rb.transform.position + _rb.transform.right * quitDistance;
        }


        if (occupied && playerGo!=null)
        {
            droneHud.SetActive(false);

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
 

    public void GetHit(float damage)
    {
        droneHp -= (int)damage;

    }
}
