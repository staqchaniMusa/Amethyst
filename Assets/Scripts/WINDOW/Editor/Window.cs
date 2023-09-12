using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Window : EditorWindow
{
    //texture
    [SerializeField]
    public Texture logo;
    
    //player health
    [SerializeField]
    public GameObject playerPrefab;
    int pHealth;
    PlayerHealth playerHealthScript;
    int playerScore;
    float reSpawnTime;


    //shooting
    [SerializeField]
    public GameObject shootingManagerPrefab;
    [SerializeField]
    public GameObject bulletPrefab;

    
    ShootingManager shootMangScript;
   
    float bulletspeed;


    [SerializeField]
    public AudioClip audioShooting;
    
    

    //Drones
    [SerializeField]
    public GameObject DroneGO;
    DroneHealth DroneHealthScript;
    DroneMovement zmbieMovementScript;
    float attackDist;
    float timeBetweenAttacks;
    float zSpeed;
    float zRotSpeed;
    float zAcceleration;
    float zStopDistance;
    int zScore;
    int zHeatlh;
    public AudioClip zSound;
    [SerializeField]
    public GameObject DroneSpawnGo;
    DroneManager DroneMangScript;
    float timeToSpawn;
    bool DroneOn;

    //room
    [SerializeField]
    public GameObject roomGO;
    int maxNumberOfPlayers;
    PhotonLobby lobbyScript;
    int minutes;
    int seconds;

    Vector2 pos;

   /* // Add menu item named "My Window" to the Window menu
    [MenuItem("MFPS PRO/Settings")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        //EditorWindow.GetWindow(typeof(Window));
        GetWindow<Window>("MFPS Quest + Photon PUN2 PRO");
       
    }

    private void OnEnable()
    {
        Initialize();
    }*/

    void Initialize()
    {

        //LOAD DEFAUTS
        if(playerPrefab == null)
        {
            playerPrefab = Resources.Load("PhotonPrefabs/PlayerAvatarNew") as GameObject;
        }
        if (shootingManagerPrefab == null)
        {
            shootingManagerPrefab = Resources.Load("ShootingManager") as GameObject;
        }
        if (DroneGO == null)
        {
            DroneGO = Resources.Load("PhotonPrefabs/Drone") as GameObject;
        }
        if (DroneSpawnGo == null)
        {
            DroneSpawnGo = Resources.Load("DRONE_MANAGER") as GameObject;
        }
        if (roomGO == null)
        {
            roomGO = Resources.Load("LOBBY") as GameObject;

        }

        try
        {
            //get health script
            playerHealthScript = playerPrefab.GetComponent<PlayerHealth>();
            pHealth = playerHealthScript.intitalhealth;
            playerScore = playerHealthScript.score;
            reSpawnTime = playerHealthScript.reSpawnTime;

            //shooting
            shootMangScript = shootingManagerPrefab.GetComponent<ShootingManager>();

            bulletspeed = shootMangScript.bulletSpeed;

            audioShooting = bulletPrefab.transform.GetChild(0).GetComponent<AudioSource>().clip;


            //get Drone scripts
            DroneHealthScript = DroneGO.GetComponent<DroneHealth>();
            zmbieMovementScript = DroneGO.GetComponent<DroneMovement>();
            attackDist = zmbieMovementScript.attackDistance;
            timeBetweenAttacks = zmbieMovementScript.timeBetweenAttaks;
            zSpeed = zmbieMovementScript.speed;
            zRotSpeed = zmbieMovementScript.rotSpeed;
            zAcceleration = zmbieMovementScript.acceleraton;
            zStopDistance = zmbieMovementScript.stopDistance;
            zScore = DroneHealthScript.scoreDrone;
            zHeatlh = DroneHealthScript.health;
            zSound = DroneGO.GetComponent<AudioSource>().clip;
            DroneMangScript = DroneSpawnGo.GetComponent<DroneManager>();
            timeToSpawn = DroneMangScript.timeToSpawn;
            DroneOn = DroneMangScript.enabled;

            //lobby script
            lobbyScript = roomGO.GetComponent<PhotonLobby>();
            maxNumberOfPlayers = lobbyScript.MaxPlayersRoom;
            seconds = lobbyScript.Time_seconds;
            minutes = lobbyScript.Time_minutes;

        }
        catch
        {

        }
       
    }

    [DidReloadScripts]
    public static void onScriptsUpdated()
    {
      //  ShowWindow();
    }
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        pos=EditorGUILayout.BeginScrollView(pos, GUILayout.Width(350), GUILayout.Height(350));

        GUI.DrawTexture(new Rect(100, 0, 60, 60), logo, ScaleMode.ScaleToFit, true, 1F);

        GUILayout.Space(70);

        if (GUILayout.Button("IMPORT FREE PACKAGES"))
        {
            //only PUN AND VOICE NEEDED
            Application.OpenURL("https://assetstore.unity.com/packages/tools/network/pun-2-free-119922");
            Application.OpenURL("https://assetstore.unity.com/packages/tools/audio/photon-voice-2-130518");

        }
        GUILayout.Label("Player settings", EditorStyles.boldLabel);
        playerPrefab = EditorGUILayout.ObjectField("Player prefab", playerPrefab, typeof(GameObject), true) as GameObject;
        pHealth = EditorGUILayout.IntField("Health", pHealth);
        playerScore = EditorGUILayout.IntField("Score when killed", playerScore);
        reSpawnTime = EditorGUILayout.FloatField("Respawn Time of player", reSpawnTime);
        

        GUILayout.Label("Shooting settings", EditorStyles.boldLabel);
        shootingManagerPrefab = EditorGUILayout.ObjectField("Shoot manager", shootingManagerPrefab, typeof(GameObject), true) as GameObject;

        bulletspeed = EditorGUILayout.FloatField("Bullet speed", bulletspeed);

        audioShooting = EditorGUILayout.ObjectField("Shooting Sound", audioShooting, typeof(AudioClip), false) as AudioClip;


        GUILayout.Label("Drone settings", EditorStyles.boldLabel);
        DroneGO = EditorGUILayout.ObjectField("This is the Drone gameObject", DroneGO, typeof(GameObject), false) as GameObject;
        attackDist = EditorGUILayout.FloatField("Attack Distance", attackDist);
        timeBetweenAttacks = EditorGUILayout.FloatField("Time between attacks", timeBetweenAttacks);
        zSpeed = EditorGUILayout.FloatField("Drone speed", zSpeed);
        zRotSpeed = EditorGUILayout.FloatField("Drone Rotation speed", zRotSpeed);
        zAcceleration = EditorGUILayout.FloatField("Acceleration", zAcceleration);
        zStopDistance = EditorGUILayout.FloatField("Stopping distance", zStopDistance);
        zHeatlh = EditorGUILayout.IntField("Drone health", zHeatlh);
        zScore = EditorGUILayout.IntField("Drone score (when dies)", zScore);
        zSound = EditorGUILayout.ObjectField("Drone Sound", zSound, typeof(AudioClip), false) as AudioClip;
        DroneSpawnGo = EditorGUILayout.ObjectField("Drone Manager", DroneSpawnGo, typeof(GameObject), false) as GameObject;
        timeToSpawn = EditorGUILayout.FloatField("Drone Spawn time", timeToSpawn);
        DroneOn = EditorGUILayout.Toggle("Spawn ON", DroneOn);


        GUILayout.Label("Room settings", EditorStyles.boldLabel);
        roomGO = EditorGUILayout.ObjectField("This is the room gameObject", roomGO, typeof(GameObject), false) as GameObject;
        maxNumberOfPlayers = EditorGUILayout.IntField("Max number of players", maxNumberOfPlayers);
        minutes = EditorGUILayout.IntField("Game minutes", minutes);
        seconds = EditorGUILayout.IntField("Game seconds", seconds);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
     
        GUILayout.Space(25);
        if (GUILayout.Button("APPLY CHANGES"))
        {
            
            //set player's health
            playerHealthScript.intitalhealth = pHealth;
            playerHealthScript.score=playerScore;
            playerHealthScript.reSpawnTime=reSpawnTime;


            PrefabUtility.RecordPrefabInstancePropertyModifications(playerPrefab);
            EditorUtility.SetDirty(playerPrefab);

            //shooting
            shootMangScript.bulletSpeed= bulletspeed;
            bulletPrefab.transform.GetChild(0).GetComponent<AudioSource>().clip= audioShooting;

            PrefabUtility.RecordPrefabInstancePropertyModifications(shootingManagerPrefab);
            EditorUtility.SetDirty(shootingManagerPrefab);
            PrefabUtility.RecordPrefabInstancePropertyModifications(bulletPrefab);
            EditorUtility.SetDirty(bulletPrefab);

            //Drone scripts
            zmbieMovementScript.attackDistance = attackDist;
            zmbieMovementScript.timeBetweenAttaks = timeBetweenAttacks;
            zmbieMovementScript.speed = zSpeed;
            zmbieMovementScript.rotSpeed = zRotSpeed;
            zmbieMovementScript.acceleraton = zAcceleration;
            zmbieMovementScript.stopDistance = zStopDistance;
            DroneHealthScript.scoreDrone = zScore;
            DroneHealthScript.health = zHeatlh;
            DroneGO.GetComponent<AudioSource>().clip= zSound;
            DroneMangScript.timeToSpawn= timeToSpawn;
            DroneMangScript.enabled = DroneOn;

            PrefabUtility.RecordPrefabInstancePropertyModifications(DroneGO);
            EditorUtility.SetDirty(DroneGO);
            PrefabUtility.RecordPrefabInstancePropertyModifications(DroneSpawnGo);
            EditorUtility.SetDirty(DroneSpawnGo);


            //room scripts
            lobbyScript.MaxPlayersRoom= maxNumberOfPlayers;
            lobbyScript.Time_seconds= seconds;
            lobbyScript.Time_minutes=minutes;

            PrefabUtility.RecordPrefabInstancePropertyModifications(roomGO);
            EditorUtility.SetDirty(roomGO);

            Debug.Log("Changes were applied");
        };

    }

}
