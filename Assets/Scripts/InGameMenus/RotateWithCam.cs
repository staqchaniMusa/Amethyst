using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// rotates the transfom to face the camera (useful for canvases)
/// </summary>
public class RotateWithCam : MonoBehaviour
{

    public float rotationSpeed = 15.5f;
    PlayerHealth pyHealth;
    void Start()
    {
        pyHealth = transform.root.GetComponent<PlayerHealth>();
    }

        
    // Update is called once per frame
    void FixedUpdate()
    {

        //relative position
        Vector3 dirHead = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        Vector3 dirThis = new Vector3(transform.forward.x, 0, transform.forward.z); ;

        float angle = (Vector3.SignedAngle(dirHead, dirThis, Vector3.up));
        if (Mathf.Abs(angle) > 20)
        {
            transform.rotation *= Quaternion.Euler(0, -rotationSpeed * Time.fixedDeltaTime*Mathf.Sign(angle), 0);
        }

    }

    
    /// <summary>
    /// function helper to respawn the player
    /// </summary>
    public void Respawn()
    {
        pyHealth.Respawn();

        VRInputModule.instance.showRenders = false;

    }
}

