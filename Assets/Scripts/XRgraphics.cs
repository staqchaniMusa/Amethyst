using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class XRgraphics : MonoBehaviour
{
    // Start is called before the first frame update
    public bool XR_improvementOn=false;
    void Start()
    {
        if (XR_improvementOn)
        {
            XRSettings.eyeTextureResolutionScale = 1.5f;
        }

        //Application.targetFrameRate = 60;
    }

  
}
