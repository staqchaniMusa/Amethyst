using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

/// <summary>
/// Used to impose game time to manage game
/// </summary>
public class TimeManager : MonoBehaviour
{
    //generate time string
    [Header("Read only--> minutes and seconds")]
    public string minStr = "";
    public string secStr = "";

    [Header("Restarts the game auto")]
    public bool autoRestart;
    [Header("UI")]
    public Text timeDisplay;

    float elapsed;
    [Header("Param")]
    public float syncroTime;
    public float totalSeconds;
    PhotonView PV;
    public bool finishGame=false;
    public float timeToFinish = 8;
    public static TimeManager TM;
    public float startingTime=8;
    public bool started = false;
    public int initTime;


    // Start is called before the first frame update
    void Start()
    {
        if(TM==null)
        {
            TM = this;
        }
        started = false;

        PV = GetComponent<PhotonView>();

        //start seconds
        totalSeconds = PhotonLobby.lobby.Time_minutes * 60 + PhotonLobby.lobby.Time_seconds;
        initTime = (int)totalSeconds;

        //format 
        if((initTime % 60)<10)
        {
            timeDisplay.text = "" + (initTime / 60) + ":" +"0"+ (initTime % 60);
        }
        else
        {
            timeDisplay.text = "" + (initTime / 60) + ":" +  (initTime % 60);
        }

        elapsed = 0;

        SceneManager.sceneLoaded += Initialize;
    }

    void Initialize(Scene scene, LoadSceneMode mode)
    {
        totalSeconds = initTime;
        
        elapsed = 0;
        started = false;

    }


    // Update is called once per frame
    void FixedUpdate()
    {
                
        //if on the multiplayer game
        if (totalSeconds<=0 && finishGame==false 
            && (SceneManager.GetActiveScene().buildIndex == (int)sceneIndex.multiplayer_01
            ||SceneManager.GetActiveScene().buildIndex == (int)sceneIndex.multiplayer_02))
        {
            finishGame = true;

            GameSetUp.GS.EndGame();

            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC("RPC_syncroTime", RpcTarget.OthersBuffered, totalSeconds);
            }          
            
        }
        //ending the game
        else if(finishGame==false)
        {
            elapsed += Time.fixedDeltaTime;
            totalSeconds -= Time.fixedDeltaTime;


            if ( initTime-totalSeconds>= startingTime && started == false)
            {
                started = true;
                DisableObjects();
            }

            if (started==false)
            {                
                UpdateTimerCount((int)( startingTime-( initTime-totalSeconds)));
            }

        }
           
        //format
        int min = (int) (Mathf.Round(totalSeconds) / 60);
        int sec = (int) (Mathf.Round(totalSeconds) % 60);

        //introduce 0 if lower than 10
        if (min<10)
        {
            minStr = "0" + min;
        }
        else
        {
            minStr = ""+min;
        }

        if (sec < 10)
        {
            secStr = "0" + sec;
        }
        else
        {
            secStr = ""+ sec;
        }

        //syncro time along the server
        if (elapsed>= syncroTime && finishGame==false)
        {
            //if it is master client, update the room time (syncronize time)
            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC("RPC_syncroTime",RpcTarget.OthersBuffered,totalSeconds);

            }

            elapsed = 0;
        }

  


    }

    /// <summary>
    /// Disablse the obejects with an specific tag
    /// </summary>
    public void DisableObjects()
    {
        GameObject[] goes = GameObject.FindGameObjectsWithTag("disableOnStart");

        foreach (GameObject go in goes)
        {
            go.SetActive(false);
        }
    }

    /// <summary>
    /// updates the time counter
    /// </summary>
    /// <param name="time"></param>
    public void UpdateTimerCount(int time)
    {
        GameObject[] texts = GameObject.FindGameObjectsWithTag("startTimerText");

        foreach (GameObject go in texts)
        {
            if (time >= 1)
            {
                go.GetComponent<Text>().text = "" + time;
            }
            else
            {
                go.GetComponent<Text>().text = "START";
            }
        }

    }

    /// <summary>
    /// called formt he UI
    /// </summary>
    /// <param name="newTime"></param>
    public void ChangeTimer(int newTime)
    {
        initTime += newTime;


        initTime = Mathf.Clamp(initTime,(1*60), (20*60));

        if ((initTime % 60) < 10)
        {
            timeDisplay.text = "" + (initTime / 60) + ":" + "0" + (initTime % 60);
        }
        else
        {
            timeDisplay.text = "" + (initTime / 60) + ":" + (initTime % 60);
        }

    }

    [PunRPC]
    public void RPC_syncroTime(float seconds)
    {
        totalSeconds = seconds;

    }

          

}
