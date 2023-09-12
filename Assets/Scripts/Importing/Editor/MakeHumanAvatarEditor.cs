using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(MakeHumanAvatar))]
public class MakeHumanAvatarEditor : Editor
{
    MakeHumanAvatar myScript;

    void OnEnable()
    {
        // Setup the SerializedProperties.


    }


    public override void OnInspectorGUI()
    {
       
        DrawDefaultInspector();

        myScript = (MakeHumanAvatar)target;

        if (GUILayout.Button("Re-import Model Rigged"))
        {
            myScript.ReimportHuman(); ;
            
        }

       

    }

    

}
