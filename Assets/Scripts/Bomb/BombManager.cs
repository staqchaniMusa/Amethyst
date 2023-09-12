using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// CONTROLS THE bomb scene and changes the turn  when game ends
/// </summary>
public class BombManager : MonoBehaviour
{
    [Header("Tansition time")]
    public float timeToChangeTeam=5;
    int teamTurn;
    // Start is called before the first frame update

    #region UNITY_FUNCTIONS 
    void Start()
    {
        if((string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"]== TypeMode.bomb.ToString())
        {
            teamTurn = int.Parse((string)PhotonNetwork.CurrentRoom.CustomProperties["TeamTurn"]);

            transform.GetChild(teamTurn).gameObject.SetActive(true);
        }
    }
    #endregion

    /// <summary>
    /// it changes the team turn
    /// </summary>
    public void ChangeTeamTurn()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            ChangeTurn();
            GameSetUp.GS.EndGame();
        }
    }

    /// <summary>
    /// actual change of turn using the custom properties of the room
    /// </summary>
    public void ChangeTurn()
    {                  

        if(teamTurn==0)
        {
            teamTurn = 1;
        }
        else
        {
            teamTurn = 0;
        }

        ExitGames.Client.Photon.Hashtable CustomRoomProperties = new ExitGames.Client.Photon.Hashtable(1)
                { { "Gmode",  (string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"]  },
                  { "Map",  (string)PhotonNetwork.CurrentRoom.CustomProperties["Map"]  },
                  { "TeamTurn", ""+teamTurn}
                };


        PhotonNetwork.CurrentRoom.SetCustomProperties(CustomRoomProperties);    

       
    }
}
