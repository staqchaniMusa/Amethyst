using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

/// <summary>
/// used to create the knif in the server
/// </summary>
public class KnifeManager : MonoBehaviour
{
    GameObject knife;
    KnifeWeapon knifeScript;
    PhotonView PV;

    // Start is called before the first frame update
    void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();

        DeployKnife();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (knifeScript)
        {
            if (knifeScript.GetComponent<PhotonView>().IsMine)
            {
                if (!knifeScript.isInHand && knife != null)
                {
                    //parenting for photon
                    knife.transform.SetParent(null);

                    knife.transform.position = transform.position;
                    knife.transform.rotation = transform.rotation;

                }
            }
        }
    }

    public void DestroyKnife()
    {
        if (PV != null)
        {
            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(knife);
            }
        }
    }


    public void DeployKnife()
    {
        if (PV.IsMine)
        {

            knife = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "knife"), transform.position, transform.rotation);

            knifeScript = knife.GetComponent<KnifeWeapon>();

            //references in script for shooting 
            knifeScript = knife.GetComponent<KnifeWeapon>();
            knifeScript.resetPos = transform;

         

        }
    }

    public void EnableObj(bool b)
    {
        PV = transform.root.GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            knife.SetActive(b);
            knife.GetComponent<KnifeWeapon>().SetNetWorkVisible(b);
        }
    }
}


