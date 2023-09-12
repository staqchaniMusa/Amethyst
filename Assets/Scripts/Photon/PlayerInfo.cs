using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// the player info script contains the selection of the player
/// </summary>
 
public class PlayerInfo : MonoBehaviour
{
   
    //singleton
    public static PlayerInfo PI;
    public TypeMode mode;

    //holds the information selected by the user
    [Header("Selected parameters for the player", order = 0)]
    public string NickName;
    public float myHeight=1.80f;
    public int myTeam=0;
    public int mySkin;
    public int myMesh=0;
    public int myWeapon = 0;
    public int myMap;
    public int myscope;
    public int myHability;

    [Header("Soldier Meshes", order = 1)]
    public int nbMeshes = 2;

    [Header("Team colors", order = 1)]
    public Color colAlpha;
    public Color colBravo;
    
    [Header("Selected parameters for the weapons", order = 1)]
    public int nbWEapons=2;
    public int nbScopes=1;
    GameObject selectedWeapon;
    public GameObject weapons;

    // ui elements
    [Header("UI elements", order = 2)]
    public Text textNickname;
    public Text textHeigth;
    public Slider sliderHeight;

    //public Dropdown dropMode;
    //public Dropdown dropTeam;
    public Text textMode;
    public Text textTeam;
    public Text textMap;
    public Sprite[] sprites;
    public Image iconImg;
    public Image mapIcon;
    public Sprite[] mapsIcons;
    int selectedIcon;
    public Text avatarTxt;

    [Header("Habilities")]
    public int nbHabilities = 2;


    //create singleton on enable
    private void OnEnable()
    {
       

        if (PlayerInfo.PI==null)
        {
            PI = this;
        }
        else
        {
            if(PlayerInfo.PI!=this)
            {
                Destroy(PlayerInfo.PI.gameObject);
                PlayerInfo.PI = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);

        ChangeMesh(0);
        ChangeWeapon(0);
        changeIcon(0);

    }

    // Start is called before the first frame update
    void Start()
    {
        //if the player has started the game
        if (PhotonNetwork.NickName == "")
        {
            //generate a random name for the player
            NickName = Random.Range(0, 100) + "Player" + Random.Range(0, 100);
            textNickname.text = NickName;

            //set name of player
            PhotonNetwork.NickName = NickName;
        }
        //if the properties are already set on the game
        else
        {
            //get-->set values for the nickname
            NickName = PhotonNetwork.NickName;
            textNickname.text = NickName;

            //get-->set values for the gameMode
            string textValue =(string)PhotonNetwork.LocalPlayer.CustomProperties["Gmode"];

            //IF YOU WANT TO USE DROP DOWN INSTEAD OF BUTTONS
            /*int numValue = 0;
            if (textValue == "Drone")
            {
                numValue = 0;
            }
            else if (textValue == "team")
            {
                numValue = 1;
            }
            else if (textValue == "royale")
            {
                numValue = 2;
            }
            dropMode.value = numValue;*/

            textMode.text = textValue;
      
            //get-->set values for the team
            int teamValue = (int)PhotonNetwork.LocalPlayer.CustomProperties["team"];

            if (teamValue == 0)
            {
                textValue = "alpha";
            }
            else if (teamValue == 1)
            {
                textValue = "bravo";
            }

            textTeam.text = textValue;
            myTeam = teamValue;

        }
    }

    public void OnNickNameChange(string text)
    {
        NickName = text;
        PhotonNetwork.NickName = NickName;
    }
    //called from UI --> changes the nickname
    public void changeNickName(Text field)
    {
        NickName = field.text;
        PhotonNetwork.NickName = NickName;
    }

    public void changeHeight(float a)
    {
        float h = float.Parse(textHeigth.text, CultureInfoCustom.instance.ci);
        myHeight = h + a;
        textHeigth.text =myHeight.ToString(CultureInfoCustom.instance.ci);

        myHeight = Mathf.Clamp(myHeight, sliderHeight.minValue, sliderHeight.maxValue);

        sliderHeight.value =myHeight;
    }

    public void SliderChangeHeight(GameObject pointer)
    {
    
        Transform posMin = sliderHeight.transform.GetChild(0);
        Transform posMax = sliderHeight.transform.GetChild(1);
        Vector3 dir = posMax.position - posMin.position;

         float interpolation = Vector3.Dot(pointer.transform.position - posMin.position, dir.normalized);
      
        myHeight =sliderHeight.minValue+interpolation*(sliderHeight.maxValue- sliderHeight.minValue)/dir.magnitude;

        sliderHeight.value = myHeight;

        float rounded = Mathf.Round(myHeight*100)/100;
        textHeigth.text =rounded.ToString(CultureInfoCustom.instance.ci);
    }

    public void changeIcon(int a)
    {
        selectedIcon += a;
        if(selectedIcon>sprites.Length-1)
        {
            selectedIcon = 0;
        }
        if(selectedIcon<0)
        {
            selectedIcon = sprites.Length-1;
        }

        iconImg.sprite = sprites[selectedIcon];
        mySkin = selectedIcon;

    }

    // allows to change the selected mesh of the player
    public void ChangeMesh(int a)
    {
        myMesh += a;
        if (myMesh > nbMeshes - 1)
        {
            myMesh = 0;
        }

        if (myMesh < 0)
        {
            myMesh = nbMeshes - 1;
        }

        if(myMesh==0)
        {
            avatarTxt.text = "MAN";
        }
        else
        {
            avatarTxt.text = "WOMAN";
        }
    }


    // allows to change the selected mesh of the player
    public void ChangeWeapon(int a)
    {


        myWeapon += a;
        if (myWeapon > nbWEapons - 1)
        {
            myWeapon = 0;
        }

        if (myWeapon < 0)
        {
            myWeapon = nbWEapons - 1;
        }
        myscope = 0;

    }


    // allows to change the selected mesh of the player
    public void ChangeHability(int a)
    {


        myHability += a;
        if (myHability > nbHabilities - 1)
        {
            myHability = 0;
        }

        if (myHability < 0)
        {
            myHability = nbHabilities - 1;
        }
        

    }

    public void ChangeScope(int a)
    {
        
        nbScopes = selectedWeapon.transform.GetChild(0).Find("Scopes").childCount;

        myscope += a;

        if (myscope > nbScopes - 1)
        {
            myscope = 0;
        }

        if (myscope < 0)
        {
            myscope = nbScopes - 1;
        }

        for(int ii=0; ii< nbScopes; ii++)
        {
            selectedWeapon.transform.GetChild(0).Find("Scopes").GetChild(ii).gameObject.SetActive(false);
        }

        selectedWeapon.transform.GetChild(0).Find("Scopes").GetChild(myscope).gameObject.SetActive(true);

        
    }

    
    //called from UI --> changes the nickname
    public void changeGameMode()
    {
        //find values
        
        if(mode==TypeMode.team)
        {
            mode =TypeMode.royale;
        }
        else if (mode == TypeMode.royale)
        {
            mode = TypeMode.drone;
        }
        else if (mode == TypeMode.drone)
        {
            mode = TypeMode.bomb;
        }
        else if (mode == TypeMode.bomb)
        {
            mode = TypeMode.flag;
        }
        else if (mode == TypeMode.flag)
        {
            mode = TypeMode.team;
        }

        textMode.text = mode.ToString();

        DebugOnCanvas.DC.Debug("Changed gameMode: " + textMode.text);

    }





    //called from UI --> changes the nickname
    public void ChangeMap()
    {
        //find values
        string value = textMap.text;
        string textValue = "";
        int index=0;

        if (value == "spaceship")
        {
            textValue = "exterior";
            index = 1;
        }
        else if (value == "exterior")
        {
            textValue = "spaceship";
            index = 0;
        }
       

        textMap.text = textValue;
        
        myMap = index;

        mapIcon.sprite = mapsIcons[myMap];

        DebugOnCanvas.DC.Debug("Changed gameMode: " + textValue);
    }


    public void changeTeam()
    {
        string value = textTeam.text;

        string textValue = "";

        if (value == "bravo")
        {
            textValue = "alpha";
            myTeam = 0;
        }
        else if (value == "alpha")
        {
            textValue = "bravo";
            myTeam = 1;
        }

        textTeam.text = textValue;

        DebugOnCanvas.DC.Debug("Changed team: " + textValue);
    }

    private void Update()
    {
        if (PhotonNetwork.InLobby)
        {
            selectedWeapon = weapons.transform.GetChild(PlayerInfo.PI.myWeapon).gameObject;
        }
    }
}
