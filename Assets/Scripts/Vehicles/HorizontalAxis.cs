using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// used to create and display the dynamic grid (hrizontal)
/// </summary>
public class HorizontalAxis : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("prefab")]
    public GameObject line;
    [Header("param")]
    public float div = 10;
    public float separationRate;
    public GameObject[] go;
    [Header("reference")]
    public Transform reference;
    RectTransform thisRect;

    void Awake()
    {

        //size of the grid
        go = new GameObject[(int)(360 / div) *2+ 1];


        //instances
        for (int ii = 0; ii < 360 / div; ii++)
        {
            go[ii] = GameObject.Instantiate(line, transform);

            go[ii].transform.GetChild(0).GetComponent<Text>().text = "" + (ii) * div;
        }

        for (int ii=0;ii<360/ div+1 ;ii++)
        {
            go[(int)(360 / div)+ii]=GameObject.Instantiate(line,transform);

            go[(int)(360 / div)+ii].transform.GetChild(0).GetComponent<Text>().text=""+ii* div;
        }

        GridLayoutGroup gd =GetComponent<GridLayoutGroup>();
        separationRate = gd.cellSize.x + gd.spacing.x;

        thisRect = GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        // compare the angle of the reference
        float angle = reference.transform.eulerAngles.y;

        if(angle>180)
        {
            angle -= 360;
        }

        if (angle < -180)
        {
            angle += 360;
        }

        thisRect.localPosition = new Vector3(angle/ div*separationRate,thisRect.localPosition.y,0);
    }
}
