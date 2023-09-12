using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// manages the health of the player
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("This is the character controller of the XRrig")]
    public CharacterController charCOntroller;
    public GameObject XR_go;

    [Header("Used for life management")]
    public float reSpawnTime = 10;
    // this is the initial health of the player
    public int intitalhealth = 100;
    public int score = 100;
    //chek if its dead
    public bool dead = false;

    //these variables manage the re-generating process
    public float regenerationTime = 1;
    public int regenerationValue = 10;
    //this is used to know when to generate the life of the player
    public bool canRegenerate = false;

    public float waitTimeDeath = 1.6f;

    //time evolving parameter
    float elapsed;



    //this is the current health of the player
    public int health;
    public int lastHealth;
    public Player lastPlayerHit;

    PhotonView PV;

    [Header("Used for death material")]
    public Material dieMaterial;


    [Header("UI elements")]
    //the image and text that stores the life of the player
    public Text healthTxt;
    public Image healthImg;
    public Image healthImg2;

    [Header("When player dies")]
    public GameObject endCanvas;
    public Image loadingBar;
    //this variable is used to change the message recieved when a player is killed by someone
    public Text killText;
    public GameObject respawnButton;
    public Animator[] anim; 


    [Header("Used for getting hit")]
    public Animator animHit;
    public AudioClip hitClip;

    AvatarSetUp avatarSetup;

    Material[][] previousMaterials;

    VRInputModule rayCast;

    //greanade manager
    [Header("Managers")]
    GrenadeManager grenadeMag;
    public PistolManager pistolMang;
    public RIfleManager rifleMang;
    public MagazineManager pistolMag;
    public MagazineManager rifleMag;
    public KnifeManager knifeMag;
    public GrenadeManager grenadeMang;

    #region UNITY FUNTIONS
    void Start()
    {
        //initialization
        dead = false;

        rayCast = GameObject.FindObjectOfType<VRInputModule>();

        PV = GetComponent<PhotonView>();

        avatarSetup = GetComponent<AvatarSetUp>();

        health = intitalhealth;
        lastHealth = health;

        elapsed = 0;


        grenadeMag = GetComponent<GrenadeManager>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;

        //get health of the player if it is set in the properties
        if (PhotonNetwork.LocalPlayer.CustomProperties != null)
        {
            health = (int)PhotonNetwork.LocalPlayer.CustomProperties["health"];
        }


        //regenerate life if condition is achieved
        if (elapsed >= regenerationTime && canRegenerate)
        {
            health += regenerationValue;

            health = Mathf.Clamp(health, 0, 100);

            

            Player PY = PhotonNetwork.LocalPlayer;
            PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                    (int)PY.CustomProperties["deaths"],
                    (int)PY.CustomProperties["score"],
                    (int)health,
                    (float)PY.CustomProperties["height"],
                    (int)PY.CustomProperties["skin"],
                    (int)PY.CustomProperties["team"],
                    (string)PY.CustomProperties["Gmode"],
                    (int)PY.CustomProperties["mesh"]);

            elapsed = 0;
            canRegenerate = true;
        }

       

        // if there is a variation in the health, get hit animation
        if (lastHealth > health)
        {
            lastHealth = health;
            animHit.SetTrigger("getHit");
        }

        //update health values
        healthTxt.text = "" + health;
        healthImg.fillAmount = (float)health / (float)intitalhealth;



        if (PV.IsMine)
        {
            PV.RPC("RPC_UpdateLifeBar", RpcTarget.All, healthImg.fillAmount);
        }


        //change color of the health bar
        if ((int)PV.Owner.CustomProperties["team"]==0)
        {
            healthImg2.transform.parent.GetComponent<Image>().color = Color.blue;
            healthImg2.color = Color.blue;
        }
        else
        {
            healthImg2.transform.parent.GetComponent<Image>().color = Color.red;
            healthImg2.color = Color.red;
        }



        ///////////////////
        //DYING
        ////////////////////
        if (health <= 0 && dead == false)
        {
            dead = true;
            if (PV.IsMine)
            {
                Player PY = PV.Owner;
                //increase deaths of the player that receives the SHOT
                PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                     (int)PY.CustomProperties["deaths"] + 1,
                     (int)PY.CustomProperties["score"],
                     0,
                     (float)PY.CustomProperties["height"],
                     (int)PY.CustomProperties["skin"],
                     (int)PY.CustomProperties["team"],
                     (string)PY.CustomProperties["Gmode"],
                     (int)PY.CustomProperties["mesh"]);


                if (lastPlayerHit != null && PhotonNetwork.LocalPlayer != lastPlayerHit)
                {
                    PY = lastPlayerHit;

                    //increase score and kills of the player that SEND THE BULLET
                    PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"] + 1,
                        (int)PY.CustomProperties["deaths"],
                        (int)PY.CustomProperties["score"] + score,
                        (int)PY.CustomProperties["health"],
                        (float)PY.CustomProperties["height"],
                        (int)PY.CustomProperties["skin"],
                        (int)PY.CustomProperties["team"],
                        (string)PY.CustomProperties["Gmode"],
                        (int)PY.CustomProperties["mesh"]);
                }

                
                //calls the rpc function for dying
                PV.RPC("RPC_Die", RpcTarget.All, PlayerInfo.PI.myMesh);
            }

        }

    }

    #endregion


    #region HIT
    public void getHit()
    {

        //set animation and vibrate
        animHit.SetTrigger("getHit");

        //VibrationManager.VM.TriggerVibration(hitClip);
        /*OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.LTouch);
        */

        animHit.transform.rotation = Quaternion.Euler(0, 0, 90);

    }


    public void GetHit(int a)
    {
    
        Player PY = PhotonNetwork.LocalPlayer;
        a = Mathf.Clamp((int)PY.CustomProperties["health"] - a, -10, intitalhealth+25);

        PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"] ,
                        (int)PY.CustomProperties["deaths"],
                        (int)PY.CustomProperties["score"] ,
                        (int)a,
                        (float)PY.CustomProperties["height"],
                        (int)PY.CustomProperties["skin"],
                        (int)PY.CustomProperties["team"],
                        (string)PY.CustomProperties["Gmode"],
                        (int)PY.CustomProperties["mesh"]);
    }

    #endregion


    #region DIE RPC
    [PunRPC]
    public void RPC_Die(int mesh)
    {
        StartCoroutine(playerDies(mesh));
    }

    [PunRPC]
    public void RPC_UpdateLifeBar(float h)
    {
        healthImg2.fillAmount = h;
    }

    public IEnumerator playerDies(int msh)
    {
        //can't regenerate
        canRegenerate = false;

        //set animation to die
        anim[msh].SetBool("die", true);
        anim[msh].gameObject.GetComponent<XR_follower>().enabled = false;


        //destroy all the weapons and magazines
        pistolMang.DestroyPistol();
        rifleMang.DestroyRifle();
        pistolMag.DestroyMagazines();
        rifleMag.DestroyMagazines();
        knifeMag.DestroyKnife();



        if (PV.IsMine)
        {
            grenadeMag.DestroyGrenade();

            //set the button to false and enable raycasting from hand
            respawnButton.SetActive(false);
            rayCast.showRenders = true;
            //rayCast.enabled = true;
        }


        //¡disable player movement and colliders!
        //Vector3 pos = XR_go.transform.position;


        //use this if you want player to fall off the gorund
        //Rigidbody rb = OVR_go.AddComponent<Rigidbody>();
        /*prevent movement and rotation
        rb.constraints = RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ
            | RigidbodyConstraints.FreezePositionX
            | RigidbodyConstraints.FreezePositionZ;

        */
        charCOntroller.enabled = false;

        yield return new WaitForSeconds(waitTimeDeath);


        //re-store animation
        anim[msh].SetBool("die", false);

        /*set the materials for the skinned mesh rendered
        previousMaterials = new Material[avatarSetup.soldierMeshes.Length][];

        for (int ii = 0; ii < avatarSetup.soldierMeshes.Length; ii++)
        {
            previousMaterials[ii] = avatarSetup.soldierMeshes[ii].GetComponent<SkinnedMeshRenderer>().materials;
            Material[] diyingMats = avatarSetup.soldierMeshes[ii].GetComponent<SkinnedMeshRenderer>().materials;

            for (int jj = 0; jj < avatarSetup.soldierMeshes[ii].GetComponent<SkinnedMeshRenderer>().materials.Length; jj++)
            {
                diyingMats[jj] = dieMaterial;
            }

            avatarSetup.soldierMeshes[ii].GetComponent<SkinnedMeshRenderer>().materials = diyingMats;

            Debug.Log("changing material");
        }
        */

        //dissable colliders
        avatarSetup.DisbleMeshes(false);


        //show Dead Canvas
        endCanvas.SetActive(true);

        //update kill text
        if (lastPlayerHit == null)
        {
            killText.text = "KILLED BY ROBOT";
        }
        else if(PhotonNetwork.LocalPlayer==lastPlayerHit)
        {
            killText.text = "SUICIDE";
        }
        else
        {
            killText.text = "KILED BY: " + lastPlayerHit.NickName;
        }


      


        loadingBar.fillAmount = 0;

        float elapsed = 0;

        while (elapsed < reSpawnTime)
        {
            elapsed += Time.fixedDeltaTime;

            loadingBar.fillAmount = (float)elapsed / (float)reSpawnTime;

            yield return new WaitForFixedUpdate();

        }

        loadingBar.fillAmount = 0;

        respawnButton.SetActive(true);
        anim[msh].gameObject.GetComponent<XR_follower>().enabled = true;



    }



    ///////////////////
    // Respawn
    ////////////////////
    public void Respawn()
    {
        //RESTORE HEALTH ONLY 
        Player PY = PhotonNetwork.LocalPlayer;
        PhotonLobby.lobby.SetCustomPlayerProp(PY, (int)PY.CustomProperties["kills"],
                (int)PY.CustomProperties["deaths"],
                (int)PY.CustomProperties["score"],
                intitalhealth, //health changed here
                (float)PY.CustomProperties["height"],
                (int)PY.CustomProperties["skin"],
                (int)PY.CustomProperties["team"],
                (string)PY.CustomProperties["Gmode"],
            (int)PY.CustomProperties["mesh"]);
        Debug.Log("Reset life and position " + intitalhealth);

        PV.RPC("RPC_Respawn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_Respawn()
    {
        //re-enable colliders

        avatarSetup.ResetTeam();
        avatarSetup.DisbleMeshes(true);

        /*use this if the rb is created
         * Destroy(rb);*/


        //ree-store player movement, canvas  and collider
        charCOntroller.enabled = true;


        endCanvas.SetActive(false);
                    
        //restore health
        health = intitalhealth;
        lastHealth = intitalhealth;



        //re-estore materials
        /*for (int ii = 0; ii < avatarSetup.soldierMeshes.Length; ii++)
        {
            avatarSetup.soldierMeshes[ii].GetComponent<SkinnedMeshRenderer>().materials = previousMaterials[ii];

        }
        */

        if ((int)PV.Owner.CustomProperties["team"] == 0)
        {
            int spawnPicker = Random.Range(0, GameSetUp.GS.spawnPointsAlpha.Length);

            charCOntroller.transform.position = GameSetUp.GS.spawnPointsAlpha[spawnPicker].position;
        }
        else
        {
            int spawnPicker = Random.Range(0, GameSetUp.GS.spawnPointsBeta.Length);

            charCOntroller.transform.position = GameSetUp.GS.spawnPointsBeta[spawnPicker].position;
        }


        //transform.rotation = GameSetUp.GS.spawnPoints[spawnPicker].rotation;
        //PV.RPC("RPC_Respawn", RpcTarget.AllBuffered);

        if (PV.IsMine)
        {
          //  grenadeMag.AddGrenade();
            
            //RE-DEPLOY WEAPONS
           // pistolMang.DeployPistol();
            rifleMang.DeployRifle();
          //  pistolMag.DeployMagazines();
            rifleMag.DeployMagazines();
            knifeMag.DeployKnife();
        }

        // it is not dead now
        Invoke("NotDead", 0.5f);
    }

    /*[PunRPC]
    public void RPC_Respawn()
    {

        
    }
    */

    public void NotDead()
    {
        dead = false;
        rayCast.showRenders = false;
    }

    #endregion

    public void SetLasPlayerHit(Player py)
    {
        PV.RPC("RPC_SetLastPlayerHit", RpcTarget.AllBuffered,py);
    }

    [PunRPC]
    public void RPC_SetLastPlayerHit(Player py)
    {
        lastPlayerHit = py;
    }

}


