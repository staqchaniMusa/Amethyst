using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// displays the different UI elements on the canvas to know the available rooms
/// </summary>
public class PlayerDisplay : MonoBehaviourPun
{
    // Start is called before the first frame update
    PhotonView PV;

    //where gameobjects will be created
    public Transform[] container;
    public Color[] cols;
    //singleton
    public static PlayerDisplay PD;
    //the prefab used to generate the room views
    public GameObject prefabPlayerView;

    public float refreshTime = 1.5f;
    float elapsed;

    public Text roomTxt;

    public float totalTime = 60;
    public float initialTime = 20;

    public bool loaded;

    public Text timerTxt;
    public Text butTextStart;


    void OnEnable()
    {
        elapsed = 1000;
        loaded = false;

        if (PhotonNetwork.IsMasterClient)
        {
            totalTime = initialTime;
        }

        PV = GetComponent<PhotonView>();

        PD = this;

       
    }



    private void Start()
    {
     
    }

    public void FixedUpdate()
    {


        elapsed += Time.fixedDeltaTime;

        if (elapsed > refreshTime)
        {
            elapsed = 0;
            UpdatePlayers();
        }

      
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                butTextStart.text = "you are the HOST: START";
                butTextStart.transform.parent.GetComponent<Button>().interactable = true;

              
            }
            else
            {
                butTextStart.text = "WAITING FOR " + PhotonNetwork.MasterClient.NickName;
                butTextStart.transform.parent.GetComponent<Button>().interactable = false;


            }
        }

        timerTxt.text = "Wait: " + Mathf.Round(totalTime) + " s";


        if (PhotonNetwork.IsMasterClient)
        {
            if (loaded==false)
            {
                totalTime -= Time.fixedDeltaTime;
            }


            if (totalTime <= 0 && loaded == false)
            {

                CallStartingGame();
                
            }
        }

    }


    public void CallStartingGame()
    {
        totalTime = 0;
        loaded = true;           
        
        //suffle colors
        if (PhotonNetwork.IsMasterClient)
        {           
            Invoke("StartGM", 2.5f);
        }
    }

    public void StartGM()
    {
        
        PhotonLobby.lobby.StartGame();
        
    }

    //create the room views in the canvas
    public void UpdatePlayers()
    {
        if (!PhotonNetwork.InRoom)
        {
            return;

        }
        roomTxt.text = PhotonNetwork.CurrentRoom.Name;

        //get existing room instances
        GameObject[] playerInstances = GameObject.FindGameObjectsWithTag("playerInstance");

        //destroy all of them
        for (int ii = 0; ii < playerInstances.Length; ii++)
        {
            Destroy(playerInstances[ii]);
        }

        Player[] players = PhotonNetwork.PlayerList;



        //create new room view
        //int jj = 0;
        for (int ii = 0; ii < players.Length; ii++)
        {
            string tempName = "" + players[ii].NickName;


            //add join action, listener to the button
            // LEAVE THIS LINE IF FIND GAME WITH SAME GMODE
            //if (PlayerInfo.PI.myGameMode== (string)roomsInfo[ii].CustomProperties["Gmode"])
            //{

            int team = (int)players[ii].CustomProperties["team"];
            GameObject goInst = GameObject.Instantiate(prefabPlayerView, container[team]);

            //set position
            //goInst.transform.localPosition = new Vector3(0, -jj * dist);

            //set texts and button actions  0--> name   1--> players  2--> join button
            goInst.transform.GetComponent<Image>().color = cols[team];
            goInst.transform.GetChild(0).GetComponent<Text>().text = tempName;

            //}
        }


        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_TimeManagement", RpcTarget.OthersBuffered, totalTime);
        }

    }

    [PunRPC]
    public void RPC_TimeManagement(float tm)
    {
        totalTime = tm;
    }


    public void Shuffle(List<int> list, Player[] pys)
    {
        string display = "";

        //for color
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, list.Count);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }


        //for position
        List<int> list2 = new List<int>();
        for (int ii = 0; ii < 16; ii++)
        {
            list2.Add(ii);
        }
        n = list2.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, list2.Count);
            int value = list2[k];
            list2[k] = list2[n];
            list2[n] = value;
        }


        //players 
        for (int ii = 0; ii < pys.Length; ii++)
        {
            PhotonLobby.lobby.SetCustomPlayerProp(pys[ii], 0, 0, 0, 100, 0, list2[ii], 0, "royale", list[ii]);
            display += "Player [" + list[ii] + "]->(pos: " + list2[ii] + ")\n";
        }

              


    }

    public void ShowMenu()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void DisconnectFromRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void SelectTeam(int a)
    {
        
        PlayerInfo.PI.myTeam = a;
        
    }

}
