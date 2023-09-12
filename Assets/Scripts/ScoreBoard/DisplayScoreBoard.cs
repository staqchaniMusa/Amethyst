using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayScoreBoard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(InputManager.instance.One_L_DW)
        {
            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);
        }
    }
}
