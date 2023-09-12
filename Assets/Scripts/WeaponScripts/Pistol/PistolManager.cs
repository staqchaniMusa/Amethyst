using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;


public class PistolManager : MonoBehaviour
{
    GameObject pistol;
    DynamicPistol pistolScript;

    PhotonView PV;
    // Start is called before the first frame update
    void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();

        DeployPistol();
    }

    // Update is called once per frame
    void Update()
    {
        if (pistolScript)
        {
            if (pistolScript.GetComponent<PhotonView>().IsMine)
            {
                if (!pistolScript.isInHand && pistol != null)
                {
                    //parenting for photon
                    pistol.transform.SetParent(null);

                    pistol.transform.position = transform.position;
                    pistol.transform.rotation = transform.rotation;

                }
            }
        }
    }

    public void DestroyPistol()
    {
        if (PV!=null)
        {
            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(pistol);
            }
        }
    }



    public void DeployPistol()
    {
        if (PV != null)
        {
            if (PV.IsMine)
            {
                pistol = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Pistol"), transform.position, transform.rotation);

                //references in script
                pistolScript = pistol.GetComponent<DynamicPistol>();
                pistolScript.resetPos = transform;
            }
        }
    }

    public void EnableObj(bool b)
    {
        PV = transform.root.GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            pistol.SetActive(b);
            pistol.GetComponent<DynamicPistol>().SetNetWorkVisible(b);
        }
    }
}
