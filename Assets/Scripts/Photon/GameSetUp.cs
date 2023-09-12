using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;

/// <summary>
/// manages the events inside the multiplayer scene
/// </summary>
public class GameSetUp : MonoBehaviourPun
{
    public bool autoRestart;
    public float timeToFinish=10;
    // Start is called before the first frame update
    public static GameSetUp GS;

    // the points used to spawn different players traged as spawnPoints
    public Transform[] spawnPointsAlpha;
    public Transform[] spawnPointsBeta;
    public Transform[] spawnPointsRandom;

    public bool ended;
    PhotonView PV;

    #region UNITY FUNCTIONS
    private void Start()
    {
        PV = GetComponent<PhotonView>();

        //check if the static value is null
        if (GameSetUp.GS==null)
        {
            GameSetUp.GS = this;
        }
    }

    private void Update()
    {
        //checK for back button pressed to go to lobby
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //DisconectPlayer(0);
        }


    }
    #endregion

    public void DisconectPlayer(int seconds)
    {
        StartCoroutine(DisconnectAndLoad(seconds));
    }

    IEnumerator DisconnectAndLoad(int second)
    {
        yield return new WaitForSeconds(second);

        PhotonNetwork.Disconnect();
  
        //WAIT FOR READY STATEMENT
        while(PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        Debug.Log("Leaving the room");

        Destroy(PhotonLobby.lobby.gameObject);
        Destroy(ShootingManager.SM.gameObject);

        //go to the lobby
        SceneManager.LoadScene((int)sceneIndex.lobby);


    }

    public void EndGame()
    {
        PV.RPC("RPC_EndGame",RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_EndGame()
    {
        StartCoroutine(EndGame_Co());
    }

    public IEnumerator EndGame_Co()
    {
        yield return new WaitForSeconds(1.5f);

        ended = true;

        Debug.Log("End game");
        //show the winner in the scoreboard
        GameObject endCanvas = GameObject.FindGameObjectWithTag("ending");
        endCanvas.transform.GetChild(0).gameObject.SetActive(true);
        endCanvas.GetComponent<EndingMenu>().UpdateValues();

        
        if (autoRestart)
        {
            yield return new WaitForSeconds(timeToFinish);

            DisconectPlayer(0);
        }
    }



}
