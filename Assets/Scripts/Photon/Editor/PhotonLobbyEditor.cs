using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(PhotonLobby))]
public class PhotonLobbyEditor : Editor
{

    void OnEnable()
    {
        // Setup the SerializedProperties.


    }


    public override void OnInspectorGUI()
    {
       
        DrawDefaultInspector();

        PhotonLobby myScript = (PhotonLobby)target;

        if (GUILayout.Button("Join Room Manually"))
        {
            myScript.createOrJoinRoom("roomTest");
                   
        }


        if (GUILayout.Button("START GAME"))
        {
            myScript.StartGame();

        }




    }

}
