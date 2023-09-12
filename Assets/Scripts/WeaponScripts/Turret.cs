using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary>
/// manages the shooting interface for the turret
/// </summary>
public class Turret : MonoBehaviour
{
    [Header("Reference scripts for grabbing")]
    public RigidBodyFollower[] follScript;

    [Header("Shooting")]
    public GameObject shootingBullet;
    public float timeBetweenBullets = 0.1f;
    public Transform gunBarrel;
    public Material[] particleMaterials;
    public float fadingTime = 0.15f;
    public AudioClip triggerClip;

    [Header("Bullets")]
    public GameObject bulletCasePrefab;
    public Transform expelTf;
    public float expelSpeed = 0.5f;
    public float randomDirection = 0.001f;
    public float randomRotation = 3.5f;

    
    [Header("Recoil")]
    public bool recoilOn = true;
    public float recoilFactorNomal = 0.2f;
    bool isInRecoil = false;
    public Transform movingPart;
    
    [Header("Temperature")]
    public bool isHot = false;
    public float temperature =0;
    public float maxTemperature = 500;
    public float shootIncremetTemp = 25;
    public float coolingTime=3.5f;
    float elapsedTemp = 0;
    public float coolingSpeed=5;
    public Material matTemp;
    Material myMat;
    public MeshRenderer myMesh;

    //timer
    float elapsed = 0;
    AudioSource audioSrc;
    PhotonView PV;


    // Start is called before the first frame update
    void Start()
    {
        myMat = new Material(matTemp.shader);
        myMesh.material = myMat;
        audioSrc = GetComponent<AudioSource>();
        PV = transform.root.GetComponent<PhotonView>();
    }


    private void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;

        //cooling
        temperature -= coolingSpeed * Time.fixedDeltaTime;
        
        if(temperature<=0)
        {
            isHot = false;
            temperature = 0;
        }

        myMat.SetFloat("temp", temperature/maxTemperature);

    }

    // Update is called once per frame
    void Update()
    {
        bool pressingTriggerCondition=false;
        if(follScript[0].holding && follScript[1].holding)
        {
            pressingTriggerCondition = (InputManager.instance.T_R && InputManager.instance.T_L);

        }
        

        // shooting logic
        if (pressingTriggerCondition && elapsed > timeBetweenBullets)
        {

            if (isHot == false)
            {
                ShootBullet();               
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


    //function that creates the bullet
    public void ShootBullet()
    {
        //temperature
        temperature += shootIncremetTemp;
        if(temperature>maxTemperature)
        {
            isHot = true;
        }

        //recoil
        if (recoilOn && isInRecoil == false)
        {
            StartCoroutine(RecoilMovement_co());
        }

        ShootingManager.SM.TurretShot(gunBarrel.position, gunBarrel.rotation, PV.Owner);
              

    }

    public IEnumerator RecoilMovement_co()
    {
        isInRecoil = true;

        Transform objectToMove1 = movingPart;

        Vector3 original = objectToMove1.localPosition;
        Vector3 objective = new Vector3();

        objective = movingPart.localPosition - recoilFactorNomal * new Vector3(-1,0,0);
     

        for (float ii = 0; ii < timeBetweenBullets / 2; ii += Time.deltaTime)
        {
            objectToMove1.localPosition = Vector3.Lerp(original, objective, 2 * ii / timeBetweenBullets);

            yield return new WaitForEndOfFrame();
        }
        objectToMove1.localPosition = objective;


        for (float ii = 0; ii < timeBetweenBullets / 2; ii += Time.deltaTime)
        {
            objectToMove1.localPosition = Vector3.Lerp(objective, original, 2 * ii / timeBetweenBullets);
    
            yield return new WaitForEndOfFrame();
        }
        objectToMove1.localPosition = original;
    
        isInRecoil = false;
    }

}
