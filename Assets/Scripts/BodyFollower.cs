using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BodyFollower : MonoBehaviour
{
    [Header("Objective and direction to follow")]
    public CharacterController objectiveController;
    public Transform head;
    [Header("Relative position to objective")]
    public float verticalOffset;
    //PhotonView PV;
    // Start is called before the first frame update
    void Awake()
    {
        // PV= transform.root.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //float height =Mathf.Max(minVerticalHeight, objective.position.y+verticalOffset);

        //obtain the position of the following object

        /*if (PV.IsMine && tran)
        {
            transform.SetParent(null);
                       
            float height = objectiveController.transform.position.y + verticalOffset;
            
            transform.position = new Vector3(head.transform.position.x, height, head.transform.position.z);
            transform.forward = new Vector3(head.forward.x, 0, head.forward.z);

            
            transform.SetParent(objectiveController.transform);
        }*/

        transform.forward = new Vector3(head.forward.x, 0, head.forward.z);
        transform.position = new Vector3(head.position.x, head.position.y- verticalOffset, head.position.z);

    }
}
