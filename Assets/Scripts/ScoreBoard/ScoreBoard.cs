using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// scoring class
/// </summary>
public class ScoreBoard : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("external ref")]
    public LeftHandMenu handMenuScp;

    [Header("Ranking")]
    public int[] scoreLimitsRank;
    public Image myRankImage;
    
    [Header("Used to determine the player position in the scoreboard")]
    public GameObject[] playerLines;

    //evolving parameter
    float elapsed = 0;

    //"Where scores are set"
    List<Text> scoresTxt;
    List<Text> namesTxt;
    List<Text> killsTxt;
    List<Text> deathsTxt;
    List<Image> images;
    List<Text> scoresTxt2;
    List<Text> namesTxt2;
    List<Text> killsTxt2;
    List<Text> deathsTxt2;
    List<Image> images2;

    //singleton
    public static ScoreBoard SB;

    [Header("Time to check the scores (refresh)")]
    public float refreshTime = 3;

    [Header("The title of this scoreboard")]
    public Text title;

    [Header("The position of the first element and the prefab")]
    public Transform REF;
    public Transform REF2;
    public GameObject prefabLine;
    public float dist=100;

    [Header("Disable these objects for battle royale")]
    public GameObject[] disableIfBattle;


    [Header("This player's display")]
    public Text myScore;
    public Text myKills;
    public Text myDeaths;

    public int scoreAlpha;
    public int scoreBravo;

    [Header("Game Time")]
    public Text gameTime;

    public Text[] totalScoreTxt;
    

    // IMPORTANT VARIABLE: where the scores are stored
    public List<ScoresPY> scoreOfPlayers;
    public List<ScoresPY> scoreOfPlayers2;
    public Player[] playerArray;
    public ScoresPY winner;


    #region UNITY FUNCTIONS
    public void OnEnable()
    {
        
        
        SB = this;
        //initialize variables for the lists, team 0
        scoresTxt= new List<Text>(); 
        namesTxt= new List<Text>(); 
        killsTxt= new List<Text>(); 
        deathsTxt= new List<Text>(); 
        images= new List<Image>();

        //same for team 1
        scoresTxt2 = new List<Text>();
        namesTxt2 = new List<Text>();
        killsTxt2 = new List<Text>();
        deathsTxt2 = new List<Text>();
        images2= new List<Image>();



        string gMode =(string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"];

        //put the text in the scoreboard
        if (gMode == TypeMode.drone.ToString())
        {
            title.text = "Drone GAME";
        }
        else if (gMode == TypeMode.team.ToString())
        {
            title.text = "TEAM GAME";
        }
        else if (gMode == TypeMode.royale.ToString())
        {
            title.text = "BATTLE ROYALE GAME";
            for (int ii = 0; ii < disableIfBattle.Length; ii++)
            {
                disableIfBattle[ii].SetActive(false);
            }
        }
        else if (gMode == TypeMode.bomb.ToString())
        {
            title.text = "BOMB GAME";
            
        }
        else if (gMode == TypeMode.flag.ToString())
        {
            title.text = "BFLAG GAME";            
        }

        elapsed = 100;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;

       
        //when a "x" seconds have passed, update the scoreboard
        if (elapsed> refreshTime)
        {
            GetScoring();
            elapsed = 0;
        }

      

        //team total score
        int scoreAlpha = 0;
        int scoreBravo = 0;

        for (int kk = 0; kk < scoreOfPlayers.Count; kk++)
        {
            scoreAlpha += scoreOfPlayers[kk].score;

        }

        for (int kk = 0; kk < scoreOfPlayers2.Count; kk++)
        {
            scoreBravo += scoreOfPlayers2[kk].score;
        }

        totalScoreTxt[0].text = "" + scoreAlpha;
        totalScoreTxt[1].text = "" + scoreBravo;



    }
    #endregion

    #region CLASS FOR SCORING
    // important class used for shorting the scores
    public class ScoresPY
    {
        public string name;
        public int kills;
        public int deaths;
        public int score;
        public int skin;

        public ScoresPY(string n, int k, int d, int sc, int sk)
        {
            name = n;
            kills = k;
            deaths = d;
            score = sc;
            skin = sk;
        }



    }
    #endregion



    public void GetScoring()
    {
        //get existing lines on the score board
        GameObject[] roomInstances = GameObject.FindGameObjectsWithTag("scoreBoardLine");

        //destroy all of them
        for (int ii = 0; ii < roomInstances.Length; ii++)
        {
            Destroy(roomInstances[ii]);
        }

        //reset the list to zero elements
        images.Clear();
        namesTxt.Clear();
        scoresTxt.Clear();
        killsTxt.Clear();
        deathsTxt.Clear();

        images2.Clear();
        namesTxt2.Clear();
        scoresTxt2.Clear();
        killsTxt2.Clear();
        deathsTxt2.Clear();



        //set this player's values
        int mySc = (int)PhotonNetwork.LocalPlayer.CustomProperties["score"];
        myScore.text = "score: " + mySc;
        myKills.text = "kills: " + (int)PhotonNetwork.LocalPlayer.CustomProperties["kills"];
        myDeaths.text = "deaths: " + (int)PhotonNetwork.LocalPlayer.CustomProperties["deaths"];
        myRankImage.sprite = PlayerInfo.PI.sprites[CheckRank(mySc)];

        //check the time
        gameTime.text = TimeManager.TM.minStr + ":" + TimeManager.TM.secStr;




        //get the player's list
        playerArray = PhotonNetwork.PlayerList;

        ////////////////////////////////////////////
        //re-create the lines of the scoreboard
        /////////////////////////////////////////////

        //create and dump values for score 
        scoreOfPlayers = new List<ScoresPY>();
        scoreOfPlayers2 = new List<ScoresPY>();

        //COUNTERS FOR POSITIONING
        int countA = 0;
        int countB = 0;


        //START PLAYER LOOP
        for (int kk = 0; kk < playerArray.Length; kk++)
        {
            //create line for team 0
            if ((int)playerArray[kk].CustomProperties["team"] == 0 || (string)playerArray[kk].CustomProperties["Gmode"] == TypeMode.royale.ToString())
            {
                GameObject lineInstance = GameObject.Instantiate(prefabLine, transform);
                lineInstance.transform.localPosition = REF.localPosition - countA * dist * new Vector3(0, 1, 0);

                //add to the list the variables
                images.Add(lineInstance.transform.GetChild(0).GetComponentInChildren<Image>());
                namesTxt.Add(lineInstance.transform.GetChild(2).GetComponentInChildren<Text>());
                scoresTxt.Add(lineInstance.transform.GetChild(3).GetComponentInChildren<Text>());
                killsTxt.Add(lineInstance.transform.GetChild(4).GetComponentInChildren<Text>());
                deathsTxt.Add(lineInstance.transform.GetChild(5).GetComponentInChildren<Text>());


                //create score class team0
                scoreOfPlayers.Add(new ScoresPY(playerArray[kk].NickName,
                (int)playerArray[kk].CustomProperties["kills"],
                (int)playerArray[kk].CustomProperties["deaths"],
                (int)playerArray[kk].CustomProperties["score"],
                (int)playerArray[kk].CustomProperties["skin"]));

                countA++;
            }
            else
            {
                GameObject lineInstance = GameObject.Instantiate(prefabLine, transform);
                lineInstance.transform.localPosition = REF2.localPosition - countB * dist * new Vector3(0, 1, 0);

                //add to the list the variables
                images2.Add(lineInstance.transform.GetChild(0).GetComponentInChildren<Image>());
                namesTxt2.Add(lineInstance.transform.GetChild(2).GetComponentInChildren<Text>());
                scoresTxt2.Add(lineInstance.transform.GetChild(3).GetComponentInChildren<Text>());
                killsTxt2.Add(lineInstance.transform.GetChild(4).GetComponentInChildren<Text>());
                deathsTxt2.Add(lineInstance.transform.GetChild(5).GetComponentInChildren<Text>());

                bool isVR = (bool)playerArray[kk].CustomProperties["isVR"];
                //create score class team1
                scoreOfPlayers2.Add(new ScoresPY(playerArray[kk].NickName + "(<b>" + (isVR ? " Giant" : "Hero") + "</b>)",
                (int)playerArray[kk].CustomProperties["kills"],
                (int)playerArray[kk].CustomProperties["deaths"],
                (int)playerArray[kk].CustomProperties["score"],
                (int)playerArray[kk].CustomProperties["skin"]));

                countB++;
            }
        }



        //order the array by score
        scoreOfPlayers.Sort((p1, p2) => p2.score.CompareTo(p1.score));
        scoreOfPlayers2.Sort((p1, p2) => p2.score.CompareTo(p1.score));

        if (scoreOfPlayers.Count > 0 && scoreOfPlayers2.Count > 0)
        {

            if (scoreOfPlayers[0].score > scoreOfPlayers2[0].score)
            {
                winner = scoreOfPlayers[0];
            }
            else
            {
                winner = scoreOfPlayers2[0];
            }
        }
        else
        {
            if (scoreOfPlayers.Count > 0)
            {
                winner = scoreOfPlayers[0];
            }
            else if (scoreOfPlayers2.Count > 0)
            {
                winner = scoreOfPlayers2[0];
            }
        }

        //set elements of the array in team 0
        int i = 0;
        foreach (ScoresPY scPY in scoreOfPlayers) //loop throught players
        {
            scoresTxt[i].enabled = true;
            namesTxt[i].enabled = true;
            killsTxt[i].enabled = true;
            deathsTxt[i].enabled = true;

            images[i].sprite = PlayerInfo.PI.sprites[CheckRank(scPY.score)];
            scoresTxt[i].text = "" + scPY.score;
            namesTxt[i].text = "" + scPY.name;
            killsTxt[i].text = "" + scPY.kills;
            deathsTxt[i].text = "" + scPY.deaths;

            i++;
        }

        //set elements of the array in team 0
        i = 0;
        foreach (ScoresPY scPY in scoreOfPlayers2) //loop throught players
        {
            scoresTxt2[i].enabled = true;
            namesTxt2[i].enabled = true;
            killsTxt2[i].enabled = true;
            deathsTxt2[i].enabled = true;

            images2[i].sprite = PlayerInfo.PI.sprites[CheckRank(scPY.score)];
            
            scoresTxt2[i].text = "" + scPY.score;
            namesTxt2[i].text = "" + scPY.name;
            killsTxt2[i].text = "" + scPY.kills;
            deathsTxt2[i].text = "" + scPY.deaths;

            i++;
        }

        elapsed = 0;
    }
       
    public void SelectTeam(int a)
    {
        PlayerInfo.PI.myTeam = a;

        Player PY = PhotonNetwork.LocalPlayer;
        PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                                (int)PY.CustomProperties["deaths"],
                                (int)PY.CustomProperties["score"],
                                (int)-1,
                                (float)PY.CustomProperties["height"],
                                (int)PY.CustomProperties["skin"],
                                (int)a,
                                (string)PY.CustomProperties["Gmode"],
                                (int)PY.CustomProperties["mesh"]);

        if (handMenuScp.audioGroup != 0)
        {
            handMenuScp.ChangeAudioGroup(a+1);
        }
    }


    public int CheckRank(int score)
    {
        int indx = 0;

        for(int ii=0; ii< scoreLimitsRank.Length;ii++)
        {
            if(score>=scoreLimitsRank[ii])
            {
                indx = ii;
            }
        }
        
        return indx;
    }
}
