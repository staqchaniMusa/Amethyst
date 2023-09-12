using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiantAvatar : MonoBehaviour
{
    private PhotonView PV;
    public Renderer[] meshesh;
    [Header("Display UI")]
    public Text canvasPlayerName;
    public Text nameTxt;
    public GameObject healthImg;

    public GameObject CanvasUI;
    PlayerHealth playerHp;

    [Header("Size Correction")]
    float coef;
    float refHeight = 1f;
    public Transform[] armature;

    public GameObject XR;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        playerHp = GetComponent<PlayerHealth>();
        if (PV.IsMine)
        { // send a buffered call to instanciate the players if someone joins the game
          //PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.NickName);
          //display the nicknames of players

            PlayerInfo.PI.myHeight = 1f;
            coef = PlayerInfo.PI.myHeight / refHeight;
            PV.RPC("RPC_ShowMesh", RpcTarget.AllBuffered, PlayerInfo.PI.myTeam);

            // send a buffered call to instanciate the players if someone joins the game
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.NickName, 1f);
            XR.SetActive(true);
        }
    }

    [PunRPC]
    // send a buffered call to instanciate the players if someone joins the game
    void RPC_ShowMesh(int team)
    {

        foreach (var rend in meshesh)
        {
            rend.enabled = !PV.IsMine;
            rend.material.color = team == 0 ? PlayerInfo.PI.colAlpha : PlayerInfo.PI.colBravo;
        }
    }

    //addd character is used to set the value of the nickname to other players
    [PunRPC]
    void RPC_AddCharacter(string nickName, float cf)
    {
        foreach (Transform tf in armature)
        {
            tf.localScale = Vector3.one * cf;
        }
        PV = GetComponent<PhotonView>();



        StartCoroutine(LateAddCharacter(nickName));
    }

    IEnumerator LateAddCharacter(string nickName)
    {
        yield return new WaitForSeconds(0.1f);
        string teamStr = "";

        //set the nickname
        nameTxt.text = nickName;

        //this is needed because buffered messages do not execute start()
        PV = GetComponent<PhotonView>();

        //Debug.Log(PV);
        //change color of the text in function of team
        if (PV != null)
        {
            if ((int)PV.Owner.CustomProperties["team"] == 0)
            {
                nameTxt.color = Color.blue;
                teamStr = "ALPHA";
                foreach (var rend in meshesh)
                {

                    rend.material.color = PlayerInfo.PI.colAlpha;
                }
            }
            else
            {
                nameTxt.color = Color.red;
                teamStr = "BRAVO";
                foreach (var rend in meshesh)
                {

                    rend.material.color = PlayerInfo.PI.colBravo;
                }
            }
        }


        Debug.Log("Intilialize player " + nickName + " " + teamStr);


        string gMode = (string)PhotonNetwork.CurrentRoom.CustomProperties["Gmode"];


        //set the spawn point of the player if not null
        if (GameSetUp.GS != null)
        {
            if ((int)PV.Owner.CustomProperties["team"] == 0)
            {
                int spawnPicker = 0;

                Vector3 fow = new Vector3();
                Vector3 pos = new Vector3();

                if (gMode == TypeMode.team.ToString()
                  || gMode == TypeMode.bomb.ToString()
                  || gMode == TypeMode.flag.ToString())
                {
                    spawnPicker = Random.Range(1, GameSetUp.GS.spawnPointsAlpha.Length);
                    pos = GameSetUp.GS.spawnPointsAlpha[0].position;
                    //fow = GameSetUp.GS.spawnPointsAlpha[spawnPicker].parent.position - GameSetUp.GS.spawnPointsAlpha[spawnPicker].position;
                }
                else if (gMode == TypeMode.royale.ToString()
                     || gMode == TypeMode.drone.ToString())
                {
                    spawnPicker = Random.Range(1, GameSetUp.GS.spawnPointsRandom.Length);
                    pos = GameSetUp.GS.spawnPointsRandom[0].position;
                    //fow = GameSetUp.GS.spawnPointsRandom[spawnPicker].forward;
                }

                //controller.transform.LookAt(fow, Vector3.up);
                transform.position = pos;
                Debug.Log(pos);
                
            }
            else if ((int)PV.Owner.CustomProperties["team"] == 1)
            {
                int spawnPicker = 0;

                Vector3 fow = new Vector3();
                Vector3 pos = new Vector3();

                if (gMode == TypeMode.team.ToString()
                  || gMode == TypeMode.bomb.ToString()
                  || gMode == TypeMode.flag.ToString())
                {
                    spawnPicker = Random.Range(1, GameSetUp.GS.spawnPointsAlpha.Length);
                    pos = GameSetUp.GS.spawnPointsBeta[0].position;
                    //fow = GameSetUp.GS.spawnPointsBeta[spawnPicker].parent.position - GameSetUp.GS.spawnPointsBeta[spawnPicker].position;
                }
                else if (gMode == TypeMode.royale.ToString()
                     || gMode == TypeMode.drone.ToString())
                {
                    spawnPicker = Random.Range(1, GameSetUp.GS.spawnPointsRandom.Length);
                    pos = GameSetUp.GS.spawnPointsRandom[0].position;
                    //fow = GameSetUp.GS.spawnPointsRandom[spawnPicker].forward;
                }

                //controller.transform.LookAt(fow, Vector3.up);
                transform.position = pos;


            }

        }

        
    }
}
