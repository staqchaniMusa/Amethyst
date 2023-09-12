using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// This is used to show the maps in game
/// </summary>
public class MapMenu : MonoBehaviour
{
    [Header("The menus available")]
    public GameObject[] menus;

    [Header("Map properties")]
    public Image[] player;
    float max_X_real;
    float min_X_real;

    float max_Y_real;
    float min_Y_real;

    [Header("Limits of the maps")]
    public Transform[] max_map;
    public Transform[] min_map;

    [Header("Starting param")]
    public bool startDisabled = true;
    public float offsetAngle=90;

    //used to prevent raycasting bloc
    //BoxCollider bxCol;

    [Header("Interpolate properties")]
    public float interpolateX;
    public float interpolateY;

    //the canvas of the Go
    GameObject canvas;
    //
    int mapIndex =0;

    //from scene
    Transform tfMin;
    Transform tfMax;


    // Start is called before the first frame update
    void Start()
    {

        //bxCol.GetComponent<BoxCollider>();
        mapIndex = SceneManager.GetActiveScene().buildIndex - 2;

        canvas = transform.GetChild(mapIndex).gameObject;

        if (startDisabled)
        {
            canvas.SetActive(false);

        }

        tfMin = GameObject.FindGameObjectWithTag("minLimit").transform;
        tfMax = GameObject.FindGameObjectWithTag("maxLimit").transform;

        max_X_real =tfMax.position.x;
        min_X_real= tfMin.position.x;

        max_Y_real = tfMax.position.z;
        min_Y_real = tfMin.position.z;


    }

    // Update is called once per frame
    void Update()
    {
        //camera position
        Vector3 posCam = new Vector3(Camera.main.transform.position.x,0, Camera.main.transform.position.z) ;

        //interpolation
        interpolateX=(max_map[mapIndex].localPosition.x - min_map[mapIndex].localPosition.x) /(max_X_real - min_X_real+0.01f)*(posCam.x-min_X_real)+ min_map[mapIndex].localPosition.x;
        interpolateY= (max_map[mapIndex].localPosition.y - min_map[mapIndex].localPosition.y) / (max_Y_real - min_Y_real+0.01f) * (posCam.z - min_Y_real) + min_map[mapIndex].localPosition.y;

        //update positions
        player[mapIndex].transform.localPosition = new Vector2(interpolateX, interpolateY);
        player[mapIndex].transform.localRotation = Quaternion.Euler(0,0,-Camera.main.transform.eulerAngles.y+offsetAngle);

        //set the canvas and custom Ray Caster
        if (InputManager.instance.One_L_DW )
        {
            bool state= !canvas.activeInHierarchy;

            canvas.SetActive(state);
            //bxCol.enabled = state;
        }


       
    }


}
