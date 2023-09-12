using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// cures the health of the player
/// </summary>
public class Life : MonoBehaviour
{
    // Start is called before the first frame update
    public int lifeValue = 35;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="XR")
        {
            collision.gameObject.transform.root.GetComponent<PlayerHealth>().GetHit(-lifeValue);

            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void IncreaseLife()
    {
        // of the player that hold the object
        GameObject.FindGameObjectWithTag("XR").transform.root.GetComponent<PlayerHealth>().GetHit(-lifeValue);
        PhotonNetwork.Destroy(gameObject);
    }
}

