using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplicateText : MonoBehaviour
{
    // Start is called before the first frame update
    public Text original;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = original.text;
    }
}
