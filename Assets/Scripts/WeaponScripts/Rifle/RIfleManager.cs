using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

/// <summary>
/// used to spawn the rifles/shotgun
/// </summary>
public class RIfleManager : MonoBehaviour
{
    public GameObject rifle;
    public DynamicRifle rifleScript;
    public ShotGun shotGunScript;

    PhotonView PV;
    // Start is called before the first frame update
    void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();

        DeployRifle();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (rifleScript)
        {
            if (rifleScript.GetComponent<PhotonView>().IsMine)
            {

                if (!rifleScript.isInHand && rifle != null)
                {
                    //parenting for photon
                    rifle.transform.SetParent(null);

                    rifle.transform.position = transform.position;
                    rifle.transform.rotation = transform.rotation;

                }

            }
        }
        else if (shotGunScript != null)
        {
            if (shotGunScript.GetComponent<PhotonView>().IsMine)
            {

                if (!shotGunScript.isInHand && rifle != null)
                {
                    //parenting for photon
                    rifle.transform.SetParent(null);

                    rifle.transform.position = transform.position;
                    rifle.transform.rotation = transform.rotation;

                }
            }
        }
        
        
    }

    public void DestroyRifle()
    {
        if (PV != null)
        {
            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(rifle);
            }
        }
    }


    public void DeployRifle()
    {
        if (PV.IsMine)
        {

            rifle = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Weapon" + "0" + (PlayerInfo.PI.myWeapon + 1)), transform.position, transform.rotation);

            if(rifle.GetComponent<DynamicRifle>())
            {
                //references in script for shooting 
                rifleScript = rifle.GetComponent<DynamicRifle>();
                rifleScript.resetPos = transform;
                rifleScript.timeBetweenBullets = ShootingManager.SM.rifleCadence[PlayerInfo.PI.myWeapon];
                rifleScript.isAuto = ShootingManager.SM.rifleIsAuto[PlayerInfo.PI.myWeapon];


                if (PlayerInfo.PI.myscope >= 0)
                {
                    rifleScript.ShowScope(PlayerInfo.PI.myscope);
                }
            }
            else if(rifle.GetComponent<ShotGun>())
            {
                //references in script for shooting 
                shotGunScript = rifle.GetComponent<ShotGun>();
                shotGunScript.resetPos = transform;
                shotGunScript.timeBetweenBullets = ShootingManager.SM.rifleCadence[PlayerInfo.PI.myWeapon];
            }
               

        }
    }

    public void EnableObj(bool b)
    {
        PV = transform.root.GetComponent<PhotonView>();

        if (PV.IsMine)
        {
            rifle.SetActive(b);
            if (rifle.GetComponent<DynamicRifle>())
            {
                rifle.GetComponent<DynamicRifle>().SetNetWorkVisible(b);
            }
            else if(rifle.GetComponent<ShotGun>())
            {
                rifle.GetComponent<ShotGun>().SetNetWorkVisible(b);
            }
           
        }

    }
}


