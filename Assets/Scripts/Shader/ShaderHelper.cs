using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// usd to change some shader parameters used to adapt the final result
/// </summary>
public class ShaderHelper : MonoBehaviour
{
    Material mat;
    PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        PV = transform.root.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            mat.SetVector("_CameraUp", Camera.main.transform.up);
            mat.SetVector("_CameraRight", Camera.main.transform.right);

            mat.SetVector("_ObjUp", transform.up);
            mat.SetVector("_ObjRight", -transform.right);
        }
        //shdaer disabled if it is not in lobby
        else if(SceneManager.GetActiveScene().buildIndex != (int)sceneIndex.lobby)
        {
            gameObject.SetActive(false);
        }
    }
}
