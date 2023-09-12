using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(PlayerHealth))]
public class PlayerHealthEditor : Editor
{

    void OnEnable()
    {
        // Setup the SerializedProperties.


    }


    public override void OnInspectorGUI()
    {
       
        DrawDefaultInspector();

        PlayerHealth myScript = (PlayerHealth)target;

        if (GUILayout.Button("Take Hit"))
        {
            myScript.GetHit(50);
                   
        }

        if (GUILayout.Button("Revive"))
        {
            myScript.Respawn();

        }


    }

}
