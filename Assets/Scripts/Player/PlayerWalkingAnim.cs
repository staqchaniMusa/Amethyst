using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingAnim : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 pos_i_1, pos_i;

    [Header("The speed of the head")]
    Vector3 speed;
    public Vector3 filteredSpeed;
    [Header("Filter value to obtain the filtered speed")]
    public float filterValue = 0.15f;

    void Start()
    {
        pos_i_1 = transform.position;


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //latest position
        pos_i = transform.position;

        //increment=speed
        speed = (pos_i - pos_i_1) / Time.fixedDeltaTime;

        filteredSpeed = Vector3.Lerp(filteredSpeed, speed, filterValue);

        filteredSpeed = new Vector3(filteredSpeed.x,0,filteredSpeed.z);


        //last position
        pos_i_1 = pos_i;
    }
}
