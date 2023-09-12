using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// helper to debug ray
/// </summary>
public class LookAtRay : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform origin;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if( Physics.Raycast(origin.position, origin.forward,out hit,1000000))
        {
            transform.forward = hit.point - transform.position;

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        }

        
    }
}
