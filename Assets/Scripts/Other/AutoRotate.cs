using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// simple script to rotate the gameobject wiht no physics
/// </summary>
public class AutoRotate : MonoBehaviour
{
    public SnapAxis axis;

    // Start is called before the first frame update
    public float rotationSpeed =0.5f;
    Quaternion initialAngle;
    public bool useInitialRotation;



    void Start()
    {
        initialAngle = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (useInitialRotation)
        {
            if (axis == SnapAxis.X)
            {
                transform.rotation = initialAngle * Quaternion.Euler(180 + rotationSpeed * Time.fixedTime,0, 0);
            }

            if (axis == SnapAxis.Y)
            {
                transform.rotation = initialAngle * Quaternion.Euler(0, 180 + rotationSpeed * Time.fixedTime, 0);
            }

            if (axis == SnapAxis.Z)
            {
                transform.rotation = initialAngle * Quaternion.Euler(0,0,180 + rotationSpeed * Time.fixedTime);
            }
        }
        else
        {
            if (axis == SnapAxis.X)
            {
                transform.rotation =  Quaternion.Euler(180 + rotationSpeed * Time.fixedTime, 0, 0);
            }

            if (axis == SnapAxis.Y)
            {
                transform.rotation =  Quaternion.Euler(0, 180 + rotationSpeed * Time.fixedTime, 0);
            }

            if (axis == SnapAxis.Z)
            {
                transform.rotation =  Quaternion.Euler(0, 0, 180 + rotationSpeed * Time.fixedTime);
            }
        }
        
    }
}
