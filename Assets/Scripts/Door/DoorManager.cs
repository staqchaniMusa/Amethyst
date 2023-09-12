using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorManager : MonoBehaviour
{
    [Header("Door parts")]
    public Transform rotator;
    public Transform upper;
    public Transform lower;
    public Transform refDoor;

    [Header("Door properties")]
    public float rotTime=0.5f;
    public float openTime = 1.2f;
    public float maxOpenDistance = 1.5f;

    public bool isOpen = false;


    Coroutine corr;

    PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = transform.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDoor()
    {
        if (corr == null && isOpen==false)
        {
            PV.RequestOwnership();
            corr = StartCoroutine(OpenDoorCoroutine());
            
        }
      

           
    }

    /// <summary>
    /// function that opens the door and closes it after a certain time
    /// </summary>
    /// <returns></returns>
    public IEnumerator OpenDoorCoroutine()
    {


        PV.RPC("RPC_SetOpen", RpcTarget.AllBuffered, true);

        float elapsed = 0;


        //rotate
        while (elapsed<rotTime)
        {
            elapsed += Time.fixedDeltaTime;
            rotator.transform.Rotate(transform.parent.forward, elapsed*20/rotTime);

            yield return new WaitForFixedUpdate();

        }

        //wait
        yield return new WaitForSeconds(0.5f);

        //open
        elapsed = 0;
        while (elapsed < openTime)
        {
            elapsed += Time.fixedDeltaTime;
            upper.transform.position = refDoor.position + new Vector3(0, elapsed*maxOpenDistance/openTime, 0);
            lower.transform.position = refDoor.position - new Vector3(0, elapsed*maxOpenDistance/openTime, 0);

            yield return new WaitForFixedUpdate();
        }

        //clamp values
        upper.transform.position =refDoor.position+ new Vector3(0,maxOpenDistance,0);
        lower.transform.position =refDoor.position+ new Vector3(0, -maxOpenDistance, 0);



        yield return new WaitForSeconds(4);


        //cole
        elapsed = 0;
        while (elapsed < openTime)
        {
            elapsed += Time.fixedDeltaTime;
            upper.transform.position = refDoor.position + new Vector3(0, maxOpenDistance-elapsed * maxOpenDistance / openTime, 0);
            lower.transform.position = refDoor.position + new Vector3(0, -maxOpenDistance+elapsed * maxOpenDistance / openTime, 0);

            yield return new WaitForFixedUpdate();
        }

        //clamp values
        upper.transform.position = refDoor.position;
        lower.transform.position = refDoor.position;


        elapsed = 0;
        //rotate
        while (elapsed < rotTime)
        {
            elapsed += Time.fixedDeltaTime;
            rotator.transform.Rotate(transform.parent.forward,elapsed * 20 / rotTime,Space.Self);

            yield return new WaitForFixedUpdate();

        }


        PV.RPC("RPC_SetOpen", RpcTarget.AllBuffered, false);
        corr = null;
    }
    
    [PunRPC]
    public void RPC_SetOpen(bool b)
    {
        isOpen = b;
    }

}
