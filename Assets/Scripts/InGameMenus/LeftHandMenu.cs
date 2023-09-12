using Photon.Pun;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum TypeOfMenu { lobby, inGame };

public class LeftHandMenu : MonoBehaviour
{
    [Header("Type of menu")]
    public TypeOfMenu type;

    [Header("The custom raycaster")]
    public VRInputModule custRay;

    [Header("The menus available")]
    public GameObject[] menus;

    [Header("Hability")]
    public Image[] habIcons;
    


    [Header("Weapon properties")]
    public Image rangeImg;
    public Image cadenceImg;
    public Image damageImg;


    [Header("Map properties")]
    public Image mapImg;
    public Sprite[] mapsSprites;

    [Header("Team properties")]
    public Image teamImg;
    public Color colAlpha, colBravo;


    public bool startDisabled = false;

    //used to prevent raycasting bloc
    //BoxCollider bxCol;
    [Header("Audio")]
    public int audioGroup = 0;
    public PunVoiceClient pV_Network;
    public Recorder pv_Recorder;


    //the canvas of the Go
    GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        //bxCol.GetComponent<BoxCollider>();
        canvas = transform.GetChild(0).gameObject;

        if (startDisabled)
        {
            canvas.SetActive(false);
            custRay.showRenders = false;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        //set the canvas and custom Ray Caster
        if(InputManager.instance.Two_L_DW )
        {
            bool state= !canvas.activeInHierarchy;

            canvas.SetActive(state);
            custRay.showRenders = state;
            //bxCol.enabled = state;
        }


        //in function of type of menu do this or that
        if (type == TypeOfMenu.lobby)
        {

            //hability icons
            for (int ii = 0; ii < habIcons.Length; ii++)
            {
                habIcons[ii].enabled = false;
            }

            habIcons[PlayerInfo.PI.myHability].enabled = true;


            //set fill amount of weapon images
          /*  rangeImg.fillAmount = ShootingManager.SM.rifleRange[PlayerInfo.PI.myWeapon] / ShootingManager.SM.maxRange;
            cadenceImg.fillAmount = (ShootingManager.SM.maxCadence - ShootingManager.SM.rifleCadence[PlayerInfo.PI.myWeapon]) / ShootingManager.SM.maxCadence;
            damageImg.fillAmount = (ShootingManager.SM.rifleDamageBody[PlayerInfo.PI.myWeapon]
                                  + ShootingManager.SM.rifleDamageHead[PlayerInfo.PI.myWeapon])
                                  / (2 * ShootingManager.SM.maxDamage);*/


            //set map properties
            /*
            string value = PlayerInfo.PI.myGameMode;
            int indx = 0;

            if (value == "team")
            {
                indx = 0;
            }
            else if (value == "royale")
            {
                indx = 1;
            }
            else if (value == "Drone")
            {
                indx = 2;
            }
            mapImg.sprite = mapsSprites[indx];
            */

            //team properties
            if (PlayerInfo.PI.myTeam == 0)
            {
                teamImg.color = colAlpha;
            }
            else
            {
                teamImg.color = colBravo;
            }
        }
        else if(type == TypeOfMenu.inGame)
        {

        }
    }

    public void Quit() { GameSetUp.GS.DisconectPlayer(0); }


    /// <summary>
    /// shows the selected menu
    /// </summary>
    /// <param name="a"></param>
    public void ShowMenu(int a)
    {
        for (int ii = 0; ii < menus.Length; ii++)
        {
            menus[ii].SetActive(false);
        }


        menus[a].SetActive(true);
    }

    public void ChangeAudioGroup(int a)
    {
        if(a==1)
        {
            a = (int)PhotonNetwork.LocalPlayer.CustomProperties["team"] + 1;
        }
        
        PhotonView PV = transform.root.GetComponent<PhotonView>();
        
        //phV_Network.Client.ChangeAudioGroups(groupsToRemove, groupsToAdd);
        if (PV != null)
        {
            if (PV.IsMine)
            {
                pV_Network.Client.ChangeAudioGroups(null, new byte[1] { (byte)a });
            }
        }
        //        phRecorder.InterestGroup = targetGroup;
        pv_Recorder.InterestGroup = (byte)a;
        audioGroup = a;
    }

}
