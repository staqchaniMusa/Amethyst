using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine.UI;
public class Talking : MonoBehaviour
{
    // Start is called before the first frame update
    public Speaker spk;
    Image im;

    void Start()
    {
        im = GetComponent<Image>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if(spk.IsPlaying)
        {
            im.enabled = true;
        }
        else
        {
            im.enabled = false;
        }

        
    }
}
