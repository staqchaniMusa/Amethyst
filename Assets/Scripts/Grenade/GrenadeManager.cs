using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

/// <summary>
/// manages the position of the grenade in game
/// </summary>
public class GrenadeManager : MonoBehaviour
{
    PhotonView PV;
    GameObject grenadeObj;
    public Transform posG;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        AddGrenade();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (PV.IsMine)
        {
            if (grenadeObj != null)
            {
                //snap position
                if (grenadeObj.GetComponent<ObjectGrabbing>().handGrabScp == null)
                {
                    if (!grenadeObj.GetComponent<Grenade>().ready)
                    {
                        grenadeObj.transform.rotation = posG.rotation;
                        grenadeObj.transform.position = posG.position;
                    }
                }

            }
        }
    }

    #region ADDObjects

    /// <summary>
    /// add the grenade
    /// </summary>
    public void AddGrenade()
    {
        if (PV.IsMine)
        {
            grenadeObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "grenade"), posG.position, posG.rotation);
            grenadeObj.transform.SetParent(posG);                        
        }

    }

    /// <summary>
    /// delete the grenade
    /// </summary>
    public void DestroyGrenade()
    {
        if(grenadeObj!=null)
        {
            PhotonNetwork.Destroy(grenadeObj);
        }
    }

    #endregion

    /// <summary>
    /// to show in the network
    /// </summary>
    /// <param name="b"></param>
    public void EnableObj(bool b)
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            if (grenadeObj)
            {
                grenadeObj.SetActive(b);
                grenadeObj.GetComponent<Grenade>().SetNetWorkVisible(b);
            }
        }
    }


}
