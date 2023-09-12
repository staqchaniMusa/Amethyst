using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// used to manage the scoring in the flag gamemode
/// </summary>
public class FlagManager : MonoBehaviour
{
    public static FlagManager instance;
    bool ended = false;
    // Start is called before the first frame update

    [Header("Team flag socres")]
    public int[] flagScore;
    [Header("Where the scores are shown")]
    public Text[] flagScorTxt;

    [Header("Maximum score to finisht he game")]
    public int maxScore;

    void Awake()
    {
        instance = this;
        // activate all elemetns if flag mode is selected    
        if( (string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"] == TypeMode.flag.ToString())
        {
            for (int ii = 0; ii < transform.childCount; ii++)
            {
                transform.GetChild(ii).gameObject.SetActive(true);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {        
        if (ended==false)
        {
            //of any of the teams gets to the maximum, end game
            if (flagScore[0]>=maxScore || flagScore[0]>=maxScore)
            {
                ended = true;

                GameSetUp.GS.EndGame();
            }
        }
    }


    public void UpdateScore(int team, int value)
    {
        flagScore[team]+=value;
        flagScorTxt[team].text=""+value+"/"+maxScore;


    }



   
}
