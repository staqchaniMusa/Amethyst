using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// to know where the player is in each connecting stage
/// </summary>
public enum connectionState
{
    notconnected,
    inLobby,
    inRoom

}

public enum TypeMode { team, royale, drone, bomb, flag };

/// <summary>
/// to know the game Mode
/// </summary>
/*public enum gameMode
{
    Cooperative,
    Teams,
    battleRoyale,
    flag,
    bomb

}
*/

/// <summary>
/// This is the most important script that allows the room/lobby connections
/// </summary>
public class PhotonLobby : MonoBehaviourPunCallbacks, IInRoomCallbacks, IPunOwnershipCallbacks
{

    //singleton
    public static PhotonLobby lobby;


    // connecting/disconnecting icons
    GameObject conectedIcon;
    GameObject disconnectedIcon;

    //list of rooms used to join an specific room or create one
    public List<RoomInfo> roomsInfo;
    
    [Header("The time of game", order = 0)]
    public int Time_minutes = 1;
    public int Time_seconds = 30;

    //the number of created rooms
    [Header("The type of game selected", order = 0)]
    //public gameMode gameMode;
    //public TypeMode gameMode;


    //the number of created rooms
    [Header("Number of rooms in the game", order = 0)]
    public int numberOfRooms;

    //to know whether the player is in lobby

    //to set the number of maximum players of a room
    [Header("Maximum number of players per room", order = 0)]
    public int MaxPlayersRoom = 8;

    //used to know the connection state
    [Header("Shows the connection state of the player", order = 0)]
    public connectionState conState;

    //photon view attached to this gameobject
    private PhotonView PV;

    // player variables
    Player[] photonPlayers;

    [Header("Shows the players in room", order = 0)]
    public int playersInRoom;

    [Header("The player prefab used to get the initial life", order = 0)]
    public GameObject playerPrefab;

    //buld index of current scene
    int currentScene;
    internal int myTeam;
    public bool isVR;

    public RoomInfo CurrentRoom { get; internal set; }

    #region UNITY Functions
    private void Awake()
    {
        conState = connectionState.notconnected;
        //initialization
        PV = GetComponent<PhotonView>();
    }
        
    public void FixedUpdate()
    {
        //find the connection buttons
        conectedIcon = GameObject.FindGameObjectWithTag("connectedIcon");
        disconnectedIcon = GameObject.FindGameObjectWithTag("disconnectedIcon");


        // set the true or false in function of connection
        if (PhotonNetwork.IsConnected)
        {
            if (conectedIcon!=null&&disconnectedIcon!=null)
            {
                conectedIcon.SetActive(true);
                disconnectedIcon.SetActive(false);
            }
        }
        else
        {
            if (conectedIcon != null && disconnectedIcon != null)
            {
                conectedIcon.SetActive(false);
                disconnectedIcon.SetActive(true);
            }
        }


    }

    public override void OnEnable()
    {
        base.OnEnable();

        //set up singleton
        if (PhotonLobby.lobby == null)
        {
            PhotonLobby.lobby = this;
        }
        else
        {
            if (PhotonLobby.lobby != this)
            {
                Destroy(PhotonLobby.lobby.gameObject);
                PhotonLobby.lobby = this;
            }

        }
        DontDestroyOnLoad(this.gameObject);


        //// [!]  [!]   TO MAKE THIS LINE WORK
        //CHANGE THE PhotonserverSettings
        if (PhotonNetwork.IsConnected==false)
        {
            PhotonNetwork.ConnectUsingSettings(); //connects to the master photon server
        }

        //needed to perfom well
        PhotonNetwork.AddCallbackTarget(this);

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        //needed to perform well
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;


    }

    #endregion

    
     #region FAIL EVENTS
    // callbacks for errors in logging
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        DebugOnCanvas.DC.Debug("Tried to create a new room but failed: room with same name exists");
        StartCoroutine(restartConnexion());
    }


    // callbacks for errors in loging
    public override void OnJoinRoomFailed(short returnCode, string message)
    {

        StartCoroutine(restartConnexion());
        DebugOnCanvas.DC.Debug("Tried to join a room but failed: " + message);

    }

    #endregion

        
    #region ON EVENTS
    //get the information of the room
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        roomsInfo = roomList;
               
    }

    // Reset user properties when connected to lobby
    public override void OnJoinedLobby()
    {
        //get name of player
        PhotonNetwork.NickName = PlayerInfo.PI.NickName;

        //set connection state
        conState = connectionState.inLobby;

        base.OnJoinedLobby();

        DebugOnCanvas.DC.Debug("Joined Lobby");


        //set custom properties from player Info and reset the score, kills, etc
        Player PY = PhotonNetwork.LocalPlayer;
        SetCustomPlayerProp(PY, 0,
                0,
                0,
                playerPrefab.GetComponent<PlayerHealth>().intitalhealth,
                PlayerInfo.PI.myHeight,
                PlayerInfo.PI.mySkin,
                PlayerInfo.PI.myTeam,
                PlayerInfo.PI.mode.ToString(),
                PlayerInfo.PI.myMesh);


    }

    
    //CALBACK USED to determine if the player is connected to the server
    public override void OnConnectedToMaster()
    {

        DebugOnCanvas.DC.Debug("Player has connected to the photon server");

        PhotonNetwork.AutomaticallySyncScene=true;
        PhotonNetwork.JoinLobby();

    }

    // always join lobby to get user/player data
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.JoinLobby();
    }

    public void Refresh()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("roomInstance");
        for(int ii=0;ii<gos.Length;ii++)
        {
            Destroy(gos[ii]);
        }
        StartCoroutine(restartConnexion());
    }

    //this function allows the user to re-connect
    IEnumerator restartConnexion()
    {
        DebugOnCanvas.DC.Debug("Refreshing");
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        while (PhotonNetwork.IsConnected == false)
        {
            yield return null;

        }
        yield return new WaitForSeconds(1);
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion

    #region ROOM EVENTS



    // THIS FUNCTION IS USED TO CREATE A ROOM IT WILL BE IN INCREASING ORDER
    public void createRoom()
    {
        //RETURN if not in lobby
        if (PhotonNetwork.InLobby == false)
        {
            return;
        }

        //name of the room
        string roomName = Random.Range(0,100)+"room"+ Random.Range(0, 100);


        /*get the number of rooms
        numberOfRooms = roomsInfo.Count;

        if (numberOfRooms == 0)
        {
            roomName += "1";
        }
        else
        {
            //check for existing rooms
            for (int ii = 0; ii < roomsInfo.Count; ii++)
            {
                roomName += (roomsInfo.Count+1);
            }
        }
        */

        //we need to make public the variable "Gmode" and "map" in the hastable (matchmaking)
        string[] str=new string[3];
        str[0] = "Gmode";
        str[1] = "Map";
        str[2] = "TeamTurn";

        // if the room does not exit, create one
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MaxPlayersRoom,
            CustomRoomProperties = (new ExitGames.Client.Photon.Hashtable(1)
                { { "Gmode", PlayerInfo.PI.mode.ToString()},
                  { "Map", PlayerInfo.PI.myMap.ToString()},
                  { "TeamTurn", "0"}
                }),
            CustomRoomPropertiesForLobby = str
        };


        //line to create the room
        PhotonNetwork.CreateRoom(roomName, roomOps);

        DebugOnCanvas.DC.Debug("Created room=" + roomName);

        numberOfRooms = roomsInfo.Count;
    }


    public void createOrJoinRoom(string roomName)
    {
        //RETURN if not in lobby
        if (PhotonNetwork.InLobby == false)
        {
            return;
        }

        //we need to make public the variable "Gmode" in the hastable (matchmaking)
        string[] str = new string[3];
        str[0] = "Gmode";
        str[1] = "Map";
        str[2] = "TeamTurn";

        // if the room does not exit, create one
        RoomOptions roomOps = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)MaxPlayersRoom,
            CustomRoomProperties = (new ExitGames.Client.Photon.Hashtable(1)
                { { "Gmode", (string)PlayerInfo.PI.mode.ToString()},
                  { "Map", PlayerInfo.PI.myMap.ToString()},
                  { "TeamTurn", "0"}
                }),
            CustomRoomPropertiesForLobby = str
        };


        //line to create the room
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOps,TypedLobby.Default);
    }

    // when a player joins a room
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        conState = connectionState.inRoom;

        DebugOnCanvas.DC.Debug("We are now in a room");

        //obtain the players connected
        photonPlayers = PhotonNetwork.PlayerList;


        //THIS IS THE MAIN CALL
        //DebugOnCanvas.DC.Debug("Starting the game");
        DebugOnCanvas.DC.Debug("Actual players=" + (PhotonNetwork.PlayerList.Length) + " of possible=" + (MaxPlayersRoom));

        //set custom properties from player Info and reset the score, kills, etc
        int PCPlayerCount = 0;
        int VRPlayerCount = 0;
        bool isVRPlayer = PhotonLobby.lobby.isVR;

        Player PY = PhotonNetwork.LocalPlayer;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player == PY) continue;
            if ((int)player.CustomProperties["team"] == PlayerInfo.PI.myTeam && (bool)player.CustomProperties["vrPlayer"]) VRPlayerCount++;
            else if ((int)player.CustomProperties["team"] == PlayerInfo.PI.myTeam) PCPlayerCount++;

            if ((isVRPlayer && VRPlayerCount > 1) || (!isVRPlayer && PCPlayerCount > 2))
            {
                PhotonNetwork.LeaveRoom();
                return;
            }
        }
        SetCustomPlayerProp(PY, 0,
                0,
                0,
                playerPrefab.GetComponent<PlayerHealth>().intitalhealth,
                PlayerInfo.PI.myHeight,
                PlayerInfo.PI.mySkin,
                PlayerInfo.PI.myTeam,
                PlayerInfo.PI.mode.ToString(),
                PlayerInfo.PI.myMesh);
        Debug.Log("Player Joined : " + PlayerInfo.PI.myTeam);
        //StartGame();

    }

    //WHEN A NEW PLAYER IS ENTERING THE ROOM
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);


        //get the player list information again
        DebugOnCanvas.DC.Debug("A new player has joined: " + newPlayer.NickName);

        //obtain the players connected
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
    }


    //decreae the players in room variable when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        DebugOnCanvas.DC.Debug(otherPlayer.NickName + " Has left the game");
        
        //obtain the players connected
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom =photonPlayers.Length;
    }


    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {

        currentScene = scene.buildIndex;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(scene.buildIndex));

        //used tow know if the multiplayer scene has been loaded
        if (currentScene == (int)sceneIndex.multiplayer_01 
            || currentScene== (int)sceneIndex.multiplayer_02)
        {
            //PV.RPC("RPC_CreatePlayer", RpcTarget.All);
            CreatePlayer();
        }

             

    }


    #endregion


    #region CREATE PLAYER PREFAB IN NETWORK


    private void CreatePlayer()
    {
        Vector3 position = Vector3.zero;
        if (GameSetUp.GS)
        {
            if(PlayerInfo.PI.myTeam == 0)
            {
                position = PhotonLobby.lobby.isVR ? GameSetUp.GS.spawnPointsAlpha[0].position : GameSetUp.GS.spawnPointsAlpha[Random.Range(1,GameSetUp.GS.spawnPointsAlpha.Length)].position;
            } else
                position = PhotonLobby.lobby.isVR ? GameSetUp.GS.spawnPointsBeta[0].position : GameSetUp.GS.spawnPointsBeta[Random.Range(1, GameSetUp.GS.spawnPointsBeta.Length)].position;
        }
        if(PhotonLobby.lobby.isVR)
        // [!] this is the directory used to get the most important players' prefab [!] --> playerAvatar
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatarNew"), position, Quaternion.Euler(0, 0, 0));
        else
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PCPlayer"), position, Quaternion.Euler(0, 0, 0));
       
    }

    #endregion

    #region PUBLIC FUNCTIONS CALLED FROM BUTTONS
    //called when editing name
    public void ChangeName(Text nicknameText)
    {
        PhotonNetwork.NickName = nicknameText.text;
    }

    public void StartGame()
    {
      
        /*while(!PhotonNetwork.InRoom
            && SceneManager.GetActiveScene().buildIndex!= int.Parse((string)PhotonNetwork.CurrentRoom.CustomProperties["Map"]))
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        */

        Debug.Log((string)PhotonNetwork.CurrentRoom.CustomProperties["Map"]);
        Player PY = PhotonNetwork.LocalPlayer;

        SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                                (int)PY.CustomProperties["deaths"],
                                (int)PY.CustomProperties["score"],
                                (int)PY.CustomProperties["health"],
                                (float)PY.CustomProperties["height"],
                                (int)PY.CustomProperties["skin"],
                                (int)PlayerInfo.PI.myTeam,
                                (string)PY.CustomProperties["Gmode"],
                                (int)PY.CustomProperties["mesh"]);
        //yield return null;
        SceneManagerAsync.SM.GoToMultiplayerScene(
            int.Parse( (string)PhotonNetwork.CurrentRoom.CustomProperties["Map"])
            );
       
      
    }


    #endregion


    public void SetCustomPlayerProp(Player py, int kills,int deaths,int score, int health, float height, int skin,int team,string Gmode, int mesh)
    {
        bool isVR = PhotonLobby.lobby.isVR;

        py.SetCustomProperties((new ExitGames.Client.Photon.Hashtable(1)
                { { "kills", kills},
                { "deaths", deaths },
                { "score", score },
                {"health", health } ,
                {"height", height },
                {"skin", skin },
                {"team", team },
                {"Gmode", Gmode },
                {"mesh",  mesh},
            {"isVR" , isVR}
                }));

        Debug.Log("My Team " + team);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        targetView.gameObject.GetComponent<IPunClientOwnershipChange>().OnOwnershipRequest(targetView, requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        targetView.gameObject.GetComponent<IPunClientOwnershipChange>().OnOwnershipTransfered(targetView,previousOwner);
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        targetView.gameObject.GetComponent<IPunClientOwnershipChange>().OnOwnershipTransferFailed(targetView, senderOfFailedRequest);
    }
    public void Quit()
    {
        Application.Quit();
    }

}
