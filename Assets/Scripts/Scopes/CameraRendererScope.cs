using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRendererScope : MonoBehaviour
{
    //public Transform reference;
    public Renderer rend;
    Material scopeMaterial;
    public float amplification = 20.0f;
   
    
    // Start is called before the first frame update
    void Start()
    {
    


    }

    // Update is called once per frame
    void Update()
    {
        scopeMaterial = rend.material;
        //scopeMaterial.mainTextureScale = new Vector2(1/amplification, 1/amplification);
        //scopeMaterial.mainTextureOffset = new Vector2((1 - 1/amplification) / 2 , (1 - 1/amplification) / 2 );

        scopeMaterial.SetVector("_tilling", new Vector2(1 / amplification, 1 / amplification));
        scopeMaterial.SetVector("_offset", new Vector2((1 - 1 / amplification) / 2, (1 - 1 / amplification) / 2));

        /* THIS PROTITYPE DOES NOT WORK
        Camera cam = GetComponent<Camera>();
        
        Matrix4x4 viewL = cam.worldToCameraMatrix;
        Matrix4x4 viewR = cam.worldToCameraMatrix;

        viewL = ChangeMatrix(viewL);
        viewR = ChangeMatrix(viewR);

        cam.SetStereoViewMatrix(Camera.StereoscopicEye.Left, viewL);
        cam.SetStereoViewMatrix(Camera.StereoscopicEye.Right, viewR);
        
        cam.fieldOfView = 8;
        

        //move camera the stereo convergence distance
        Camera cam = GetComponent<Camera>();
        //Transform weapon = cam.transform.parent;

        cam.transform.rotation = reference.rotation;
        cam.transform.position =reference.position-cam.transform.right*cam.stereoSeparation*3;

        
        scopeMaterial.mainTextureScale = new Vector2(coef,coef);
        scopeMaterial.mainTextureOffset = new Vector2((1-coef)/2- offsetX, (1-coef)/2-offsetY);
        */


    }

    /*
    Matrix4x4 ChangeMatrix( Matrix4x4 m )
    {
              
        m[0, 0] *= 5;
        //m[0, 1] = 0;
        //m[0, 2] = ;
        //m[0, 3] = 0;
        //m[1, 0] = 0;
        m[1, 1] *= 5;
        //m[1, 2] = ;
        //m[1, 3] = 0;
        //m[2, 0] = 0;
        //m[2, 1] = 0;
        m[2, 2] *= 5;
        //m[2, 3] = ;
        //m[3, 0] = 0;
        //m[3, 1] = 0;
        //m[3, 2] = ;
        m[3, 3] *= 0;

        return m;
    }
    */
}
