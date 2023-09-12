using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SpatialTracking;

/// <summary>
/// called once the player is initialized--> this is attached to each player
/// </summary>
public class AvatarSetUp : MonoBehaviour
{


    [Header("Used to add new ammo packs")]
    public MagazineManager pistolAmmoMg;
    public MagazineManager rifleAmmoMg;

    [Header("Hand scripts")]
    public HandGrabbing handL;
    public HandGrabbing handR;
    

    //the photon view attached to the player
    private PhotonView PV;


    [Header("Display UI")]
    //variables to display name and obtain the canvas of the player
    public Text nameTxt;
    public GameObject healthImg;
    public Text canvasPlayerName;


    [Header("XR objects")]
    // used for OCULUS MFPS and must be destroyed for all the other players excep for THE LOCAL PLAYER
    public GameObject[] XR_objects;
    
    //oculus follower script that must be destroyed if not LOCAL
    public CharacterController controller;
    public Transform xrRig;
    //the meshes of the different soldiers
    [Header("Meshes/soldiers/hands")]
    public GameObject[] soldierMeshes;
    public GameObject body;
    public Renderer[] photohands;
    public Renderer[] hudRends;
    public Material[] hudMats;
    //the colliders of the player (USE TO DISABLE SELF-SHOOTING)
    //public Collider[] cols;

    [Header("Tracking")]
    public TrackedPoseDriver posedriver;

    [Header("Vehicle")]
    public bool inVehicle;
    public GameObject HudVehicle;

    PlayerHealth playerHp;

    [Header("Size Correction")]
    float coef;
    float refHeight =8f;
    public Transform[] armature;
    

    // Start is called before the first frame update
    #region UNITY FUNTION
    void Start()
    {
        playerHp = GetComponent<PlayerHealth>();
        PV = GetComponent<PhotonView>();

        if (PV.IsMine)
        {
            coef = PlayerInfo.PI.myHeight / refHeight;

            // send a buffered call to instanciate the players if someone joins the game
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.NickName, 5f);

            soldierMeshes[PlayerInfo.PI.myMesh].SetActive(true);
            
            foreach (GameObject soldierM in soldierMeshes)
            {
                for (int ii=0; ii<soldierM.transform.childCount;ii++)
                {
                    if (soldierM.transform.GetChild(ii).GetComponent<Renderer>() != null)
                    {
                        soldierM.transform.GetChild(ii).GetComponent<Renderer>().enabled = false;
                    }
                }
               
            }

            // set the active mesh of the player in the other clients
            PV.RPC("RPC_ShowMesh", RpcTarget.OthersBuffered, PlayerInfo.PI.myMesh, PlayerInfo.PI.myTeam);

            //disable colliders FOR DISABLE SELF-SHOOTING
            /*            
            foreach (Collider col in cols)
            {
                col.enabled=(false);
            }
            */

            //display the nicknames of players
            string teamStr = "";
            if((int)PhotonNetwork.LocalPlayer.CustomProperties["team"]==0)
            {
                teamStr = "ALPHA";
                hudRends[0].material = hudMats[0];
                hudRends[1].material = hudMats[0];
            }
            else
            {
                teamStr = "BRAVO";
                hudRends[0].material = hudMats[1];
                hudRends[1].material = hudMats[1];
            }

            canvasPlayerName.text = PhotonNetwork.NickName+" ["+ teamStr+"]";
            nameTxt.enabled = false;
            healthImg.SetActive(false);

            //Set active the OVR for LOCAL PLAYER --> MAIN CAMERA
            for (int ii = 0; ii < XR_objects.Length; ii++)
            {
                XR_objects[ii].SetActive(true);

            }

            
        }

    }
    #endregion

    #region BUFFERED RPC
    //addd character is used to set the value of the nickname to other players
    [PunRPC]
    void RPC_AddCharacter(string nickName, float cf)
    {
        foreach (Transform tf in armature)
        {
           //tf.localScale = cf * tf.localScale;
           tf.localScale = Vector3.one * cf;
        }
        StartCoroutine(LateAddCharacter(nickName));
    }

    IEnumerator  LateAddCharacter(string nickName)
    {
        yield return new WaitForSeconds(0.1f);
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
            }
            else
            {
                nameTxt.color = Color.red;
            }
        }


       
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
                    spawnPicker = Random.Range(0, GameSetUp.GS.spawnPointsAlpha.Length);
                    pos = GameSetUp.GS.spawnPointsAlpha[0].position;
                   // fow = GameSetUp.GS.spawnPointsAlpha[spawnPicker].parent.position - GameSetUp.GS.spawnPointsAlpha[spawnPicker].position;
                }
                else if (gMode == TypeMode.royale.ToString()
                     || gMode == TypeMode.drone.ToString())
                {
                    spawnPicker = Random.Range(0, GameSetUp.GS.spawnPointsRandom.Length);
                    pos = GameSetUp.GS.spawnPointsRandom[spawnPicker].position;
                    //fow = GameSetUp.GS.spawnPointsRandom[spawnPicker].forward;
                }

                //controller.transform.LookAt(fow, Vector3.up);
                transform.position = pos;
                xrRig.position = pos;

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
                    spawnPicker = Random.Range(0, GameSetUp.GS.spawnPointsAlpha.Length);
                    pos = GameSetUp.GS.spawnPointsBeta[0].position;
                    fow = GameSetUp.GS.spawnPointsBeta[spawnPicker].parent.position - GameSetUp.GS.spawnPointsBeta[spawnPicker].position;
                }
                else if (gMode == TypeMode.royale.ToString()
                     || gMode == TypeMode.drone.ToString())
                {
                    spawnPicker = Random.Range(0, GameSetUp.GS.spawnPointsRandom.Length);
                    pos = GameSetUp.GS.spawnPointsRandom[spawnPicker].position;
                    fow = GameSetUp.GS.spawnPointsRandom[spawnPicker].forward;
                }

                //controller.transform.LookAt(fow, Vector3.up);
                transform.position = pos;
                xrRig.position = pos;

            }
        }
    }
    [PunRPC]
    // send a buffered call to instanciate the players if someone joins the game
    void RPC_ShowMesh(int msh, int team)
    {

        soldierMeshes[msh].SetActive(true);

        for (int ii = 0; ii < soldierMeshes[msh].transform.childCount; ii++)
        {
            if (soldierMeshes[msh].transform.GetChild(ii).GetComponent<Renderer>() != null)
            {
                soldierMeshes[msh].transform.GetChild(ii).GetComponent<Renderer>().enabled = true;

                if (team == 0)
                {
                    soldierMeshes[msh].transform.GetChild(ii).GetComponent<Renderer>().material.color = PlayerInfo.PI.colAlpha;
                }
                else
                {
                    soldierMeshes[msh].transform.GetChild(ii).GetComponent<Renderer>().material.color = PlayerInfo.PI.colBravo;
                }
            
            }
            
        }

  
    }


    #endregion


    #region COLLIDERS
    public void DisbleMeshes(bool b)
    {
        PV.RPC("RPC_DisableMeshes", RpcTarget.AllBuffered, b);
    }

    [PunRPC]
    void RPC_DisableMeshes(bool b)
    {
        foreach (GameObject msh in soldierMeshes)
        {
            msh.SetActive(b);
        }
        foreach (Renderer msh in photohands)
        {
            if (!PV.IsMine)
            {
                msh.enabled = (b);
            }
        }
    }

    #endregion


    #region VEHICLE

    public void InsideVehicle()
    {

        //posedriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;

        inVehicle = true;


        PV.RPC("RPC_InsideVehicle", RpcTarget.AllBuffered);
        PV.RPC("RPC_HideMesh", RpcTarget.AllBuffered);

    }

    [PunRPC]
    public void RPC_InsideVehicle()
    {
        handL.potentialOnjectInHand.Clear();
        handR.potentialOnjectInHand.Clear();

        body.SetActive(false);

        //if hud goes with head
        //HudVehicle.SetActive(true);

        playerHp.pistolMang.EnableObj(false);
        playerHp.rifleMang.EnableObj(false);
        playerHp.rifleMag.EnableObj(false);
        playerHp.pistolMag.EnableObj(false);
        playerHp.knifeMag.EnableObj(false);
        playerHp.grenadeMang.EnableObj(false);


    }

    public void OutsideVehicle()
    {
        //posedriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        inVehicle = false;


        PV.RPC("RPC_OutsideVehicle", RpcTarget.AllBuffered);
        PV.RPC("RPC_ShowMesh2", RpcTarget.AllBuffered, (int)PlayerInfo.PI.myMesh);

        
    }

    [PunRPC]
    public void RPC_OutsideVehicle()
    {
        controller.transform.up = new Vector3(0, 1, 0);

        body.SetActive(true);       

        playerHp.pistolMang.EnableObj(true);
        playerHp.rifleMang.EnableObj(true);
        playerHp.rifleMag.EnableObj(true);
        playerHp.pistolMag.EnableObj(true);
        playerHp.knifeMag.EnableObj(true);
        playerHp.grenadeMang.EnableObj(true);


    }

    [PunRPC]
    // send a buffered call to instanciate the players if someone joins the game
    void RPC_HideMesh()
    {
        for (int ii = 0; ii < soldierMeshes.Length; ii++)
        {
            soldierMeshes[ii].SetActive(false);
        }   

        
    }

    [PunRPC]
    // send a buffered call to instanciate the players if someone joins the game
    void RPC_ShowMesh2(int val)
    {
        for (int ii = 0; ii < soldierMeshes.Length; ii++)
        {
            if (ii==val || ii>soldierMeshes.Length-3)
            {
                soldierMeshes[ii].SetActive(true);
            }
        }

    }


    public void ResetTeam()
    {
        if (PV.IsMine)
        {
            coef = PlayerInfo.PI.myHeight / refHeight;

            if ((int)PhotonNetwork.LocalPlayer.CustomProperties["team"] == 0)
            {
                hudRends[0].material = hudMats[0];
                hudRends[1].material = hudMats[0];
            }
            else
            {
                hudRends[0].material = hudMats[1];
                hudRends[1].material = hudMats[1];
            }

            // send a buffered call to instanciate the players if someone joins the game
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.NickName, coef);
            PV.RPC("RPC_ShowMesh", RpcTarget.OthersBuffered, PlayerInfo.PI.myMesh, (int)PV.Owner.CustomProperties["team"]);
        }
    }

        #endregion
    }
