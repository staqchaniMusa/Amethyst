using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// used to create and display the dynamic grid (vertical)
/// </summary>
public class VerticalAxis : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("prefab")]
    public GameObject line;
    [Header("param")]
    public float div = 10;
    public float separationRate;
    public GameObject[] go;
    public float th = 5;
    RectTransform thisRect;
    [Header("Reference")]
    public Transform reference;

    void Awake()
    {
        //size of the grid
        go = new GameObject[(int)(90 / div) *2+ 1];

        // instances
        for (int ii = 0; ii < 90 / div; ii++)
        {
            go[ii] = GameObject.Instantiate(line, transform);

            go[ii].transform.GetChild(0).GetComponent<Text>().text = "" + (90/div-ii) * div;
        }

        for (int ii=0;ii<90/div +1 ;ii++)
        {
            go[(int)(90 / div)+ii]=GameObject.Instantiate(line,transform);

            go[(int)(90 / div)+ii].transform.GetChild(0).GetComponent<Text>().text=""+-ii* div;
        }

        GridLayoutGroup gd =GetComponent<GridLayoutGroup>();
        separationRate = gd.cellSize.y + gd.spacing.y;

        thisRect = GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        // compare the angle of the reference
        float angle = reference.transform.eulerAngles.z;

        if (angle < -180)
        {
            angle += 360;
        }
        else if(angle > 180)
        {
            angle -= 360;
        }
              

        thisRect.localPosition = new Vector3(angle / div*separationRate,0,0);
    }
}
