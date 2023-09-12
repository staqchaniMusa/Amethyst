using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSimpleAvatar : MonoBehaviour
{

    //oculus objects
    [Header("These are the XR objects", order = 2)]
    public Transform XR_handL;
    public Transform XR_head, XR_handR, XR_body;
    //old version //public Transform XR_rifle, XR_gun;

    [Header("These are the references for the MFPS", order = 3)]
    public Transform REF_handL;
    public Transform REF_head, REF_handR, REF_body;
    //old version //public Transform REF_rifle, REF_gun;
    //public Transform headBone;


    //public Transform FREE_footR,FREE_footL;


    [Header("Offset of the avatar regarding the headset and hands", order = 5)]
    public float offset = 0.25f;
    public Vector3 offsetHandR = new Vector3(0, -0.15f, -0.25f);
    public Vector3 offsetHandL = new Vector3(0, -0.15f, -0.25f);

    [Header("Following factor", order = 9)]
    public float fwFactor = 1;

    public float playerHeight = 1.75f;
    PhotonView PV;
    public Vector3 offsetHead;
    // Start is called before the first frame update
    void Start()
    {
        offsetHead = REF_head.position - XR_head.position;
        PV = GetComponent<PhotonView>();
        playerHeight = (float)PV.Owner.CustomProperties["height"];
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            REF_handL.position = Vector3.Lerp(REF_handL.position, XR_handL.position, fwFactor);
            REF_head.position = Vector3.Lerp(REF_head.position, XR_head.position + offsetHead, fwFactor);
            REF_handR.position = Vector3.Lerp(REF_handR.position, XR_handR.position, fwFactor);
            REF_body.position = Vector3.Lerp(REF_body.position, XR_body.position, fwFactor);
            //old version //REF_gun.position = Vector3.Lerp(REF_gun.position, XR_gun.position, fwFactor);
            //old version // REF_rifle.position = Vector3.Lerp(REF_rifle.position, XR_rifle.position, fwFactor);


            REF_handL.rotation = Quaternion.Lerp(REF_handL.rotation, XR_handL.rotation, fwFactor);
            REF_head.rotation = Quaternion.Lerp(REF_head.rotation, XR_head.rotation, fwFactor);
            REF_handR.rotation = Quaternion.Lerp(REF_handR.rotation, XR_handR.rotation, fwFactor);
            REF_body.rotation = Quaternion.Lerp(REF_body.rotation, XR_body.rotation, fwFactor);
            //old version //REF_gun.rotation = Quaternion.Lerp(REF_gun.rotation, XR_gun.rotation, fwFactor);
            //old version //REF_rifle.rotation = Quaternion.Lerp(REF_rifle.rotation, XR_rifle.rotation, fwFactor);


            Vector3 proyectionHead = new Vector3(REF_head.position.x, 0, REF_head.position.z);
            Vector3 directionHead = new Vector3(REF_head.forward.x, 0, REF_head.forward.z);
            Vector3 directionB = new Vector3(REF_body.forward.x, 0, REF_body.forward.z);

            //Vector3 objective = proyectionHead - directionHead * offset +(cc.transform.position.y-verticalOffset) * new Vector3(0, 1, 0);
            //Vector3 objective = proyectionHead - directionHead * offset + (cc.transform.position.y) * new Vector3(0, 1, 0); ;

            //Vector3 objective = proyectionHead-direction* offset- verticalOffset*new Vector3(0,1,0);
            //Vector3 objective = transform.position - direction * offset;

           /* if (active)
            {
                player.position = Vector3.Lerp(player.position, objective, fwFactor);

                //rotate avatar if the head gets more than an angle 
                Vector3 directionPlayer = new Vector3(player.forward.x, 0, player.forward.z);

                relAngle = Mathf.Abs(Vector3.Angle(directionPlayer, directionHead));
                if (relAngle > 35.5f)
                {
                    player.forward = Vector3.Lerp(player.forward, directionHead, fwFactor);

                }


            }*/
        }
    }
}
