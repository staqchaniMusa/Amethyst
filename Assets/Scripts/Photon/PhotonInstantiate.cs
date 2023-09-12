using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonInstantiate : MonoBehaviour
{
    public string[] prefabs;
    public Transform[] positions;
    public bool isPlayerDependent=false;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            for (int ii = 0; ii < prefabs.Length; ii++)
            {
                if (isPlayerDependent)
                {
                    GameObject goInst = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", prefabs[ii]), positions[ii].position, positions[ii].rotation);
                }
                else
                {
                    GameObject goInst = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", prefabs[ii]), positions[ii].position, positions[ii].rotation);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
