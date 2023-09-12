using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// destroys a gameobject after a certain time (life time)
/// </summary>
public class DestroyAfterTime : MonoBehaviour {

    //time to destroy the gameobject
    public float lifeTime=5;
    // time variable
    float elapsed;
    ObjectGrabbing grabScript;

	// Use this for initialization
	void OnEnable () {
        elapsed = 0;
        grabScript = GetComponent<ObjectGrabbing>();

    }


    // Update is called once per frame
    void FixedUpdate () {
        elapsed += Time.fixedDeltaTime;

        if(grabScript)
        {
            if(grabScript.handGrabScp)
            {
                this.enabled = false;
            }
        }

        if(elapsed > lifeTime)
        {
            if (GetComponent<PhotonView>() != null)
            {
                if(GetComponent<PhotonView>().IsMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}
}
