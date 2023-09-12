using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ShootingManager : MonoBehaviour
{
    public static ShootingManager SM;
    [Header("Bullet prefab and settings by weapons in order")]
    public GameObject[] bulletPrefab;
    public float bulletSpeed;
    public int gunDamageBody;
    public int gunDamageHead;
    public int[] rifleDamageBody;
    public int[] rifleDamageHead;
    public float[] rifleCadence;
    public float[] rifleRange;
    public bool[] rifleIsAuto;

    [Header("Turret")]
    public int turretDamageBody;
    public int turretDamageHead;

    [Header("Tank")]
    public GameObject bulletTankPrefab;
    public int damageTankBullet;
    public float rangeTankBullet;

    [Header("Plane")]
    public GameObject bulletPlanePrefab;
    public int damagePlaneBullet;
    public float rangePlaneBullet;

    [Header("Rocket")]
    public GameObject rocketPrefab;
    public int damageRocket;
    public float maxDistanceRocket = 15;

    [Header("shotgun")]
    public GameObject bulletSHotGun;   
    public float precissionShotGun=0.7f;


    [Header("Enemy Bullet settings")]
    public GameObject bulletEnemy;
    public int enemyDamage=10;
    public float enemyRange = 25;


    [Header("Max parameters to display on menu")]
    public float maxRange;
    public float maxCadence;
    public float maxDamage;


    private PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        SM = this;
        PV = GetComponent<PhotonView>();

        //set up singleton
        if (ShootingManager.SM == null)
        {
            ShootingManager.SM = this;
        }
        else
        {
            if (ShootingManager.SM != this)
            {
                Destroy(ShootingManager.SM.gameObject);
                ShootingManager.SM = this;
            }

        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //for debbuging porpuses
        if(Input.GetKeyDown("h"))
        {
            RifleShoot(Camera.main.transform.position+new Vector3(0,-0.5f,0)+Camera.main.transform.forward*0.68f, Camera.main.transform.rotation, PhotonNetwork.LocalPlayer);
        }
    }

    public void RifleShoot(Vector3 pos, Quaternion rot,Player py )
    {

        PV.RPC("RPC_Shooting", RpcTarget.All, rifleDamageBody[PlayerInfo.PI.myWeapon], rifleDamageHead[PlayerInfo.PI.myWeapon], pos, rot, py);

    }


    public void TankShoot(Vector3 pos, Quaternion rot, Player py)
    {

        PV.RPC("RPC_ShootingTank", RpcTarget.All, damageTankBullet, pos, rot, py);
    }


    [PunRPC]
    public void RPC_ShootingTank(int damage, Vector3 positionWeapon, Quaternion rotationWeapon, Player py)
    {

        //create the shooting bullet
        GameObject inst_bullet = GameObject.Instantiate(bulletTankPrefab, positionWeapon, rotationWeapon);

        inst_bullet.transform.SetParent(transform);

        VehicleBullet bulletSc = inst_bullet.transform.GetChild(0).GetComponent<VehicleBullet>();
        bulletSc.maxDist = rangeTankBullet;
        bulletSc.maxDamage = damage;
        bulletSc.speed = bulletSpeed;

        bulletSc.playerORigin = py;

        //to know which player has created the bullet
        //Debug.Log(bulletSc.playerORigin.NickName);

    }


    public void PlaneShoot(Vector3 pos, Quaternion rot, Player py)
    {

        PV.RPC("RPC_ShootingPlane", RpcTarget.All, damagePlaneBullet, pos, rot, py);
    }


    [PunRPC]
    public void RPC_ShootingPlane(int damage, Vector3 positionWeapon, Quaternion rotationWeapon, Player py)
    {

        //create the shooting bullet
        GameObject inst_bullet = GameObject.Instantiate(bulletPlanePrefab, positionWeapon, rotationWeapon);

        inst_bullet.transform.SetParent(transform);

        VehicleBullet bulletSc = inst_bullet.transform.GetChild(0).GetComponent<VehicleBullet>();
        bulletSc.maxDist = rangeTankBullet;
        bulletSc.maxDamage = damage;
        bulletSc.speed = bulletSpeed;

        bulletSc.playerORigin = py;

        //to know which player has created the bullet
        //Debug.Log(bulletSc.playerORigin.NickName);

    }



    public void RocketLauncherShoot(Vector3 pos, Quaternion rot, Player py, int PVindex)
    {

        PV.RPC("RPC_Rocket", RpcTarget.All, damageRocket,pos, rot, py, PVindex);
    }

    [PunRPC]
    public void RPC_Rocket(int damage, Vector3 positionWeapon, Quaternion rotationWeapon, Player py, int PVindex)
    {

        //create the shooting bullet
        GameObject inst_bullet = GameObject.Instantiate(rocketPrefab, positionWeapon, rotationWeapon);

        inst_bullet.transform.SetParent(transform);

        Rocket rocketScp = inst_bullet.transform.GetChild(0).GetComponent<Rocket>();
        rocketScp.speed = bulletSpeed;
        rocketScp.maxDamage = damage;
        rocketScp.maxDist = maxDistanceRocket;
        rocketScp.playerOrigin = py;
        rocketScp.objetivePVindex = PVindex;

        //to know which player has created the bullet
        //Debug.Log(bulletSc.playerORigin.NickName);

    }


    public void GunShoot(Vector3 pos, Quaternion rot, Player py)
    {

        PV.RPC("RPC_Shooting", RpcTarget.All, gunDamageBody, gunDamageHead, pos, rot,py);
    }
    [PunRPC]
    public void RPC_Shooting(int damageBody, int damageHead, Vector3 positionWeapon, Quaternion rotationWeapon, Player py)
    {

        //create the shooting bullet
        GameObject inst_bullet = GameObject.Instantiate(bulletPrefab[PlayerInfo.PI.myWeapon], positionWeapon, rotationWeapon);

        inst_bullet.transform.SetParent(transform);

        Bullet bulletSc = inst_bullet.transform.GetChild(0).GetComponent<Bullet>();
        bulletSc.speed = bulletSpeed;
        bulletSc.damageBody = damageBody;
        bulletSc.damageHead = damageHead;
        bulletSc.range = rifleRange[PlayerInfo.PI.myWeapon];

        bulletSc.playerORigin = py;

        //to know which player has created the bullet
        //Debug.Log(bulletSc.playerORigin.NickName);

    }

    public void GunShootGun(Vector3 pos, Quaternion rot, Player py)
    {

        PV.RPC("RPC_ShootingShotGun", RpcTarget.All, rifleDamageBody[PlayerInfo.PI.myWeapon], rifleDamageHead[PlayerInfo.PI.myWeapon], pos, rot, py);
    }

    [PunRPC]
    public void RPC_ShootingShotGun(int damageBody, int damageHead, Vector3 positionWeapon, Quaternion rotationWeapon, Player py)
    {

        //create the shooting bullet
        GameObject inst_bullet = GameObject.Instantiate(bulletSHotGun, positionWeapon, rotationWeapon);

        inst_bullet.transform.SetParent(transform);

        Bullet[] bulletSc = new Bullet[inst_bullet.transform.childCount];
        for (int ii = 0; ii < bulletSc.Length-3; ii++)
        {
            bulletSc[ii]=inst_bullet.transform.GetChild(ii).GetComponent<Bullet>();
            bulletSc[ii].speed = bulletSpeed;
            bulletSc[ii].damageBody = damageBody;
            bulletSc[ii].damageHead = damageHead;
            bulletSc[ii].range = rifleRange[PlayerInfo.PI.myWeapon];
            bulletSc[ii].anglePrecission = precissionShotGun;
            bulletSc[ii].playerORigin = py;
        }
        //to know which player has created the bullet
        //Debug.Log(bulletSc.playerORigin.NickName);
       
    }

    public void ShootEnemy(Vector3 pos, Quaternion rot, Player py)
    {
        PV.RPC("RPC_EnemyShooting", RpcTarget.All, enemyDamage,  pos, rot, py);

    }

    [PunRPC]
    public void RPC_EnemyShooting(int damageBody, Vector3 positionWeapon, Quaternion rotationWeapon, Player py)
    {

        //create the shooting bullet
        GameObject inst_bullet = GameObject.Instantiate(bulletEnemy, positionWeapon, rotationWeapon);

        inst_bullet.transform.SetParent(transform);


        Bullet bulletSc = inst_bullet.transform.GetChild(0).GetComponent<Bullet>();
        bulletSc.speed = bulletSpeed;
        bulletSc.damageBody = damageBody;
        bulletSc.damageHead = damageBody;
        bulletSc.range = enemyRange;

        bulletSc.playerORigin = py;

        //to know which player has created the bullet
        //Debug.Log(bulletSc.playerORigin.NickName);

    }


    public void TurretShot(Vector3 pos, Quaternion rot, Player py)
    {

        PV.RPC("RPC_Shooting", RpcTarget.All, turretDamageBody, turretDamageHead, pos, rot, py);

    }
}
