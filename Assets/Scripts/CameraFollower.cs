using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private CharacterController _cc;
    public Transform cameraRig;
    public Transform cameraTf;

    Vector3 projPosOfCam;
    // Start is called before the first frame update
    void Start()
    {
        _cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        projPosOfCam = new Vector3(cameraRig.position.x,0, cameraRig.position.z);
        Vector3 projPosOfCC= new Vector3(_cc.transform.position.x, 0, _cc.transform.position.z);

        Vector3 deltaMovement =projPosOfCam-projPosOfCC;

        _cc.height = cameraTf.position.y-projPosOfCC.y;

        //if (deltaMovement.magnitude > 0.1f)
        //{
            _cc.Move(deltaMovement);
            projPosOfCC = new Vector3(_cc.transform.position.x, 0, _cc.transform.position.z);
            cameraRig.position = new Vector3(projPosOfCC.x, cameraRig.position.y, projPosOfCC.z);
        //}
        

    }



}
