using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// Shows the results of the game as a summary when the game ends
/// </summary>
public class EndingMenu : MonoBehaviour
{
    // Start is called before the first frame update


    [Header("UI ELEMENTS")]
    public Text bestPlayer;
    public ScoreBoard scoreBscript;
    public Image imgPlayer;
    public Text waiting;
    public Text summary;
    public Image winnerImage;
    public Button restartBut;


    public void FixedUpdate()
    {
        // say which is the masterclient
        waiting.text = "Waiting:" + PhotonNetwork.MasterClient.NickName;

        if(PhotonNetwork.IsMasterClient)
        {
            restartBut.interactable = true;
        }
        else
        {
            restartBut.interactable = false;
        }

        //display the line render for selection
        if (TimeManager.TM.totalSeconds<=0 
            || transform.root.GetComponent<PlayerHealth>().health<=0
            || GameSetUp.GS.ended)
        {
            VRInputModule.instance.showRenders = true;
        }


    }

    /// <summary>
    /// called when the game ends
    /// </summary>
    public void UpdateValues()
    {
        //mode and turn
        string mode= (string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"];
        int playerTurn = int.Parse((string)PhotonNetwork.CurrentRoom.CustomProperties["TeamTurn"]);

        summary.text = "MODE:" + mode;

        //total team score calculation in function of mode
        if (mode==TypeMode.team.ToString()
            || mode == TypeMode.drone.ToString()
            )
        {
            // compare team scores
            if (scoreBscript.scoreAlpha > scoreBscript.scoreBravo)
            {
                summary.text += " BLUE TEAM WINS";
                winnerImage.color = Color.blue;
            }
            else if (scoreBscript.scoreAlpha < scoreBscript.scoreBravo)
            {
                summary.text += " RED TEAM WINS";
                winnerImage.color = Color.red;
            }
            else
            {
                summary.text += " TIE";
                winnerImage.color = Color.black;
            }
        }
        else if(mode == TypeMode.royale.ToString())
        {
            summary.text +=" "+ scoreBscript.winner.name+ " WINS";
            winnerImage.color = Color.black;

        }
        else if (mode == TypeMode.bomb.ToString())
        {
            //only if the bomb has eploded
            Bomb bombScp = GameObject.FindGameObjectWithTag("bomb").GetComponent<Bomb>();
            if (bombScp.explode)
            {
                if (playerTurn==0)
                {
                    summary.text += " BLUE TEAM WINS";
                    winnerImage.color = Color.blue;
                }
                else if (playerTurn==1)
                {
                    summary.text += " RED TEAM WINS";
                    winnerImage.color = Color.red;
                }
            }

        }
        else if (mode == TypeMode.flag.ToString())
        {
            // compare flag points
            if (FlagManager.instance.flagScore[0] > FlagManager.instance.flagScore[1])
            {
                summary.text += " BLUE TEAM WINS";
                winnerImage.color = Color.blue;
            }
            else if (FlagManager.instance.flagScore[0] < FlagManager.instance.flagScore[1])
            {
                summary.text += " RED TEAM WINS";
                winnerImage.color = Color.red;
            }
            else
            {
                summary.text += " TIE";
                winnerImage.color = Color.black;
            }

        }

        scoreBscript.gameObject.SetActive(true);
        scoreBscript.GetScoring();

        bestPlayer.text =
        scoreBscript.winner.name
        + "  K:" + scoreBscript.winner.kills
        + "  D:" + scoreBscript.winner.deaths
        + "  SC:" + scoreBscript.winner.score;

        imgPlayer.sprite = PlayerInfo.PI.sprites[scoreBscript.CheckRank(scoreBscript.winner.score)];



    }
    /// <summary>
    /// used to restart he game when called (button press)
    /// </summary>
    public void RestartGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
