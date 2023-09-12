using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

/// <summary>
/// creates magazines in function of the selection
/// </summary>
public enum MagazineType { rifle, pistol, shotgun };

public class MagazineManager : MonoBehaviour
{
    
    // Start is called before the first frame update
    [Header("Type in function of weapon")]
    public MagazineType magType;

    [Header("param")]
    public float thDistance=0.35f;
    public float distCoef = 0.05f;

    public Transform[] magPositions;
    List<Vector3> pos;
    List<Quaternion> rot;

    PhotonView myPV;

    [Header("Radable existing mags")]
    public List<GameObject> mag;


    //used to prevent multiple magazines
    bool ready=false;

    void Awake()
    {

        myPV = transform.root.GetComponent<PhotonView>();

     

        pos = new List<Vector3>();
        rot = new List<Quaternion>();

        GameObject go =Resources.Load(  Path.Combine("PhotonPrefabs", "Weapon0" + (PlayerInfo.PI.myWeapon + 1)) ) as GameObject;
        
        if(go.GetComponent<ShotGun>() && magType==MagazineType.rifle)
        {
            Debug.Log("Change to shotgun magazines");
            magType = MagazineType.shotgun;

        }
        DeployMagazines();
    }

    // Update is called once per frame
    void Update()
    {
        //create a magazine if it does not exist (USED IF YOU WANT TO INSTANTIATE MORE THAN ONE)

        UpdatePositions();
        if (mag.Count>0)
        {                         
            for (int ii = 0; ii < mag.Count; ii++)
            {
                if (mag[ii] != null)
                {
                    if (mag[ii].GetComponent<PhotonView>().IsMine)
                    {
                        Magazine magScript = mag[ii].GetComponent<Magazine>();

                        if (!magScript.isInHand && !magScript.isLose)
                        {
                            mag[ii].transform.position = pos[ii];
                            mag[ii].transform.rotation = rot[ii];
                        }
                    }
                }
            }
                
            
        }
        

        /* reference to the last object
        if (lastInstantiated != null)
        {
            PV = lastInstantiated.GetComponent<PhotonView>();
        }     
        */
               
        
    }

    void UpdatePositions()
    {
        for (int ii = 0; ii < magPositions.Length; ii++)
        {
            if (magType == MagazineType.rifle || magType == MagazineType.pistol)
            {
                pos[ii] = magPositions[ii].position;
                rot[ii]=magPositions[ii].rotation;
            }
            else if(magType == MagazineType.shotgun)
            {
                pos[ii*3] = magPositions[ii].position;
                pos[ii*3 + 1] = magPositions[ii].position - magPositions[ii].forward * distCoef;
                pos[ii*3+ 2] = magPositions[ii].position + magPositions[ii].forward * distCoef;
                
            }
        }

                
               
    }


    public void CreateMagazine()
    {


        for (int ii = 0; ii < magPositions.Length; ii++)
        {
           
            if (magType == MagazineType.rifle)
            {
                mag.Add(PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "MagazineWeapon0"+(PlayerInfo.PI.myWeapon+1)), magPositions[ii].position, magPositions[ii].rotation));
                pos.Add(magPositions[ii].position);
                rot.Add(magPositions[ii].rotation);
            }
            if (magType == MagazineType.pistol)
            {
                mag.Add(PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Magazine"), magPositions[ii].position, magPositions[ii].rotation));
                pos.Add(magPositions[ii].position);
                rot.Add(magPositions[ii].rotation);
            }
            if (magType == MagazineType.shotgun)
            {
                Vector3 pos1 = magPositions[ii].position;
                Vector3 pos2 = magPositions[ii].position - magPositions[ii].forward*distCoef;
                Vector3 pos3 = magPositions[ii].position + magPositions[ii].forward * distCoef; 


                mag.Add(PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "MagazineWeapon0" + (PlayerInfo.PI.myWeapon + 1)), pos1, magPositions[ii].rotation));
                mag.Add(PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "MagazineWeapon0" + (PlayerInfo.PI.myWeapon + 1)), pos2, magPositions[ii].rotation));
                mag.Add(PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "MagazineWeapon0" + (PlayerInfo.PI.myWeapon + 1)), pos3, magPositions[ii].rotation));
                pos.Add(pos1);
                rot.Add(magPositions[ii].rotation);
                pos.Add(pos2);
                rot.Add(magPositions[ii].rotation);
                pos.Add(pos3);
                rot.Add(magPositions[ii].rotation);
            }

            //set parenting to the mag
            //mag.GetComponent<PhotonAbsoluteRepositioning>().tf =magPositions[ii];
        }




        
    }

  
    public void DestroyMagazines()
    {

        myPV = transform.root.GetComponent<PhotonView>();


        if (myPV != null)
        {
            if (myPV.IsMine)
            {

                for (int ii = 0; ii < mag.Count; ii++)
                {

                    if (mag[ii] != null)
                    {
                        PhotonNetwork.Destroy(mag[ii]);
                    }
                }
            }
        }
    }

    public void DeployMagazines()
    {
        mag = new List<GameObject>();

        if (myPV != null)
        {
            if (myPV.IsMine)
            {
                CreateMagazine();
            }
        }
    }


    public void EnableObj(bool b)
    {
        myPV = transform.root.GetComponent<PhotonView>();
        if (myPV.IsMine)
        {
            for (int ii = 0; ii < mag.Count; ii++)
            {
                if (mag[ii] != null)
                {
                    mag[ii].GetComponent<Magazine>().SetNetWorkVisible(b);
                }
            }

            for (int ii = 0; ii < mag.Count; ii++)
            {
                if (mag[ii] != null)
                {
                    mag[ii].SetActive(b);
                }
            }
        }
    }

}
