using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attached to the jumping plataform
/// </summary>
public class PlatformJump : MonoBehaviour
{
    //local used to move the player
    Transform playerTf;
    CharacterController cc;

    [Header("Jumping parameters")]
    public float maxHeight=10;
    public float jumpTime = 3.5f;
    public Transform destinationTf;


    Vector3 origin;
    Vector3 destination;

    Vector3 dir;
    

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        destination = destinationTf.position;

    }


    private void OnTriggerEnter(Collider other)
    {
        //if the player gets in
        if(other.tag=="XR")
        {
            //dump values 
            playerTf = other.transform;
            cc = playerTf.GetComponent<CharacterController>();

            StartCoroutine(Jump_Co());
        }
    }
    
    /// <summary>
    /// jumping corrutine
    /// </summary>
    /// <returns></returns>
    IEnumerator Jump_Co()
    {
        cc.enabled = false;

        float elapsed = 0;

        while(elapsed< jumpTime/2)
        {
            playerTf.position =Vector3.Lerp(origin,destination,elapsed/jumpTime)+Vector3.up*Mathf.Lerp(0,maxHeight, elapsed / jumpTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        while (elapsed < jumpTime)
        {
            playerTf.position = Vector3.Lerp(origin, destination, elapsed / jumpTime) + Vector3.up * Mathf.Lerp(maxHeight,0, elapsed / jumpTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


        yield return new WaitForFixedUpdate();

        cc.enabled = true;
        playerTf = null;
        cc = null;
    }
}
