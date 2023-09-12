using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// displays the different UI elements on the canvas to know the available rooms
/// </summary>
public class RoomDisplay : MonoBehaviourPun
{
    // Start is called before the first frame update

    //where gameobjects will be created
    public Transform container;
    public GameObject loginGo;
    //singleton
    public static RoomDisplay RD;
    //the prefab used to generate the room views
    public GameObject prefabRoomView;

    public float refreshTime = 1.5f;
    float elapsed;

    void Start()
    {
        elapsed = 1000;

        RD = this;
    }

    public void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;

        if (elapsed > refreshTime)
        {
            elapsed = 0;
            if (PhotonLobby.lobby.roomsInfo != null)
            {
                UpdateRooms(PhotonLobby.lobby.roomsInfo);
            }
        }
    }

    public void ReloadRooms()
    {
        if (PhotonLobby.lobby.roomsInfo != null)
        {
            UpdateRooms(PhotonLobby.lobby.roomsInfo);
        }
    }
    //create the room views in the canvas
    public void UpdateRooms(List<RoomInfo> roomsInfo)
    {
        //get existing room instances
        GameObject[] roomInstances = GameObject.FindGameObjectsWithTag("roomInstance");

        //destroy all of them
        for(int ii=0;ii<roomInstances.Length;ii++)
        {
            Destroy(roomInstances[ii]);
        }

        //create new room view
        int jj = 0;
        for (int ii = 0; ii < roomsInfo.Count; ii++)
        {
            string tempName = roomsInfo[ii].Name;


            //add join action, listener to the button
            // LEAVE THIS LINE IF FIND GAME WITH SAME GMODE
            //if (PlayerInfo.PI.myGameMode== (string)roomsInfo[ii].CustomProperties["Gmode"])
            //{
            if (roomsInfo[ii].PlayerCount>0)
            {

                GameObject goInst = GameObject.Instantiate(prefabRoomView, container);

                //set texts and button actions  0--> name   1--> players  2--> join button



                goInst.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = PlayerInfo.PI.mapsIcons[int.Parse((string)roomsInfo[ii].CustomProperties["Map"])];
                goInst.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = roomsInfo[ii].Name;
                goInst.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "[" + roomsInfo[ii].PlayerCount + "/" + roomsInfo[ii].MaxPlayers + "]";
                goInst.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = "" + (string)roomsInfo[ii].CustomProperties["Gmode"];

                RoomInfo room = roomsInfo[ii];
                goInst.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
                {
                    PhotonLobby.lobby.CurrentRoom = room;
                    PhotonNetwork.JoinRoom(tempName);
                    //Debug.Log("Joined Room: " + tempName);
                    loginGo.SetActive(false);
                    PlayerDisplay.PD.ShowMenu();
                });
                jj++;
                //}
            }
        }
       

    }
}
