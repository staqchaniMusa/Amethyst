using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class Warning : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {


        string errorMessage =
                       "This package works with FOUR FREE packages from the store:\n -Photon PUN2 \n - Photon voice2";

#if UNITY_EDITOR

        //ERROR MESSAGE 1
        if (!Directory.Exists(Application.dataPath + "/Photon") )
        {
            if (UnityEditor.EditorUtility.DisplayDialog("Packages needed to import", errorMessage, "Download", "OK, Play with errors"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/network/pun-2-free-119922");
                Application.OpenURL("https://assetstore.unity.com/packages/tools/audio/photon-voice-2-130518");


            }
        }

        //ERROR MESSAGE 2
        errorMessage = "You must import solider as RIG-HUMANOID--> you can do it manually, by selecting the humanoid in the rig options of the model";
        if (!gameObject.GetComponent<MakeHumanAvatar>().checkModelType())
        {
            if (UnityEditor.EditorUtility.DisplayDialog("Rigged models", errorMessage, "RE-IMPORT", "OK, Play with errors"))
            {
                Selection.activeGameObject = gameObject;
                gameObject.GetComponent<MakeHumanAvatar>().ReimportHuman();

            }
        }

#endif


    }
    }


