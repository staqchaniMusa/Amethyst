using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class XR_follower : MonoBehaviour
{
    // Start is called before the first frame update
    
    PhotonView PV;

    [Header("This is the maximum parent of the gameobject", order = 0)]
    [Header("It is used to move the XR objects-->TRUE ", order = 1)]
    public Transform superParent;
    // use oculus feedback for animation
    public bool useXR = false;
    public bool active;


    //oculus objects
    [Header("These are the XR objects", order = 2)]
    public Transform XR_handL;
    public Transform XR_head, XR_handR, XR_body;
    //old version //public Transform XR_rifle, XR_gun;

    [Header("These are the references for the MFPS", order = 3)]
    public Transform REF_handL;
    public Transform REF_head, REF_handR, REF_body, player;
    //old version //public Transform REF_rifle, REF_gun;
    public Transform headBone;
    
    //public Transform FREE_footR,FREE_footL;


    [Header("Offset of the avatar regarding the headset and hands", order = 5)]
    public float offset=0.25f;
    public Vector3 offsetHandR=new Vector3(0,-0.15f,-0.25f);
    public Vector3 offsetHandL = new Vector3(0, -0.15f, -0.25f);

    public Vector3 offsetRotHandR = new Vector3(0, 0, 0);
    public Vector3 offsetRotHandL = new Vector3(0, 0, 0);

    [Header("Animator attached to the soldier/avatar", order = 6)]
    public Animator anim;

    [Header("Weights for right hand, left hand and head", order = 7)]
    public float w_Rhand = 0.5f;
    public float w_Rhand_rot=1, w_Lhand_rot, w_Lhand = 0.5f, w_look=0.5f, varWeight=0;


    [Header("Reference to the character controller of the XR", order = 8)]
    public CharacterController cc;
    
    //relative angle between the head and body
    float relAngle;


    [Header("Following factor", order = 9)]
    public float fwFactor = 1;

    [Header("For crouching", order = 10)]
    public float verticalOffset;
    public float crouchCorrector = 0.75f;
    public float playerHeight = 1.75f;
    //public float correctionFactor = 0.15f;

    [Header("For walking at roomScale", order = 11)]
    public PlayerWalkingAnim playerWalkingAnim;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        playerHeight = (float)PV.Owner.CustomProperties["height"];
        /*string[] muscleName = HumanTrait.MuscleName;
                for (int i = 0; i < HumanTrait.BoneCount; ++i)
        {
            Debug.Log("i=" + i+ "  "+muscleName[i]);
        }
        */

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // GETS THE OCULUS TO CORRESPOND TO THE TRANSFORMS OF THE AVATAR     
        if (useXR && PV.IsMine)
        {
            REF_handL.position = Vector3.Lerp(REF_handL.position, XR_handL.position, fwFactor);
            REF_head.position = Vector3.Lerp(REF_head.position, XR_head.position, fwFactor);
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
            Vector3 objective = proyectionHead - directionHead * offset+ (cc.transform.position.y) * new Vector3(0, 1, 0); ;

            //Vector3 objective = proyectionHead-direction* offset- verticalOffset*new Vector3(0,1,0);
            //Vector3 objective = transform.position - direction * offset;

            if (active)
            {
                player.position = Vector3.Lerp(player.position, objective, fwFactor);

                //rotate avatar if the head gets more than an angle 
                Vector3 directionPlayer = new Vector3(player.forward.x, 0, player.forward.z);

                relAngle = Mathf.Abs(Vector3.Angle(directionPlayer, directionHead));
                if ( relAngle> 35.5f)
                {
                    player.forward = Vector3.Lerp(player.forward, directionHead, fwFactor);
                   
                }
                
               
            }
        }


    }


    float ReMapValue(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void OnAnimatorIK(int layerIndex)
    {

        //vertical offset is used to place the mesh properly
        //verticalOffset = playerHeight - (REF_head.position.y - REF_body.position.y);// +correctionFactor;
        verticalOffset = (REF_head.position.y - REF_body.position.y);// +correctionFactor;

        float factor = playerHeight/ crouchCorrector;

        float animHeadPos = verticalOffset - factor;//ReMapValue(verticalOffset, playerHeight / 4, 0, playerHeight, 1);

        varWeight = 3 / playerHeight * verticalOffset;
        varWeight = Mathf.Clamp(varWeight, 0, 1);

        anim.SetFloat("headPosition", animHeadPos);


        //setting weights
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, w_Lhand);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, w_Rhand);
 
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, w_Lhand_rot);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, w_Rhand_rot);

        //for feet
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, varWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, varWeight);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, varWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, varWeight);

        //head weight
        anim.SetLookAtWeight(w_look);

        //making positin and rotation of hands
        anim.SetIKPosition(AvatarIKGoal.LeftHand, REF_handL.position
                                            +REF_handL.forward*offsetHandL.z
                                            +REF_handL.up*offsetHandL.y
                                            +REF_handL.right*offsetHandL.x);
        anim.SetIKPosition(AvatarIKGoal.RightHand, REF_handR.position 
                                            + REF_handR.forward * offsetHandR.z
                                            + REF_handR.up * offsetHandR.y
                                            + REF_handR.right * offsetHandR.x);

        anim.SetIKRotation(AvatarIKGoal.LeftHand, REF_handL.rotation*Quaternion.Euler(offsetRotHandL.x,offsetRotHandL.y, offsetRotHandL.z));
        anim.SetIKRotation(AvatarIKGoal.RightHand, REF_handR.rotation * Quaternion.Euler(offsetRotHandR.x, offsetRotHandR.y, offsetRotHandR.z));

        //feet
        //making rotation
        /*
        anim.SetIKPosition(AvatarIKGoal.LeftFoot, FREE_footL.position);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, FREE_footR.position);

        anim.SetIKRotation(AvatarIKGoal.LeftFoot,FREE_footL.rotation);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, FREE_footR.rotation);
        */

        //head
        anim.SetLookAtPosition(headBone.position+REF_head.forward*0.5f);

        if(cc.velocity.magnitude>0.1f || relAngle>35 || playerWalkingAnim.filteredSpeed.magnitude>0.1f)
        {
            anim.SetBool("running", true);
        }
        else
        {
            anim.SetBool("running", false);
        }

        //Debug.Log(cc.velocity.magnitude);
    }


    

    public void restartPosition()
    {
        if ((int)PV.Owner.CustomProperties["team"] == 0)
        {
            int spawnPicker = Random.Range(0, GameSetUp.GS.spawnPointsAlpha.Length);

            cc.transform.position = GameSetUp.GS.spawnPointsAlpha[spawnPicker].position;
        }
        else
        {
            int spawnPicker = Random.Range(0, GameSetUp.GS.spawnPointsBeta.Length);

            cc.transform.position = GameSetUp.GS.spawnPointsBeta[spawnPicker].position;
        }
    }
    
}
