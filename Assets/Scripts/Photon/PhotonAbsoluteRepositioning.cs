using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonAbsoluteRepositioning : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform tf;

    void Start()
    {
        //tf = transform.parent;

    }

    // Update is called once per frame
    void Update()
    {
        //this process is needed in the new version fo PHOTON
        if (tf != null)
        {
            //transform.SetParent(tf);
            transform.SetParent(null);

            transform.rotation = tf.rotation;
            transform.position = tf.position;
        }
        
    }
}
