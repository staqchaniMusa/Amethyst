using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// depreciated
/// </summary>
public class ZoomCalibration : MonoBehaviour
{
    // Start is called before the first frame update
    public CameraRendererScope rendScope;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(InputManager.instance.T_R_DW)
        {
            rendScope.offsetY += 0.001f;
        }

        if (InputManager.instance.T_L_DW)
        {
            rendScope.offsetY -= 0.001f;
        }

        if (InputManager.instance.One_R_DW)
        {
            rendScope.offsetX += 0.001f;
        }

        if (InputManager.instance.One_L_DW)
        {
            rendScope.offsetX -= 0.001f;
        }
        */

        //GetComponent<Text>().text = "X:"+rendScope.offsetX+" Y:"+ rendScope.offsetY;
    }
}
