using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script allows to debug values out of the console into a gameobject that is tagged "debugText"
/// </summary>
public class DebugOnCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    // used to set to viewing state
    public bool is_On;

    //create singleton
    public static DebugOnCanvas DC;

    //the text for the debug
    string debugText;

    //the place where the text will be shown
    Text objectiveText;


    void Start()
    {
        //set up singleton
        if (DebugOnCanvas.DC == null)
        {
            DebugOnCanvas.DC = this;
        }
        else
        {
            if (DebugOnCanvas.DC != this)
            {
                Destroy(DebugOnCanvas.DC.gameObject);
                DebugOnCanvas.DC = this;
            }

        }
        DontDestroyOnLoad(this.gameObject);

        
    }

    public void Clear()
    {
        debugText = "";
    }

    //when called it will show
    public void Debug(string a)
    {
        //only when is true get the gameobject
        if (is_On == true)
        {
            if (GameObject.FindGameObjectWithTag("debugText") != null)
            {
                objectiveText = GameObject.FindGameObjectWithTag("debugText").GetComponent<Text>();

            }

            // generate the debug, using the latest feedback as the last part of the string
            debugText = a + "\n" + debugText;
            if (objectiveText != null)
            {
                objectiveText.text = debugText;
            }
        }
        //if the we do not want to debug values, set the gameobject to false.
        else
        {
            if (GameObject.FindGameObjectWithTag("debugText") != null)
            {
                GameObject.FindGameObjectWithTag("debugText").SetActive(false);
            }
        }
    
    }
}
