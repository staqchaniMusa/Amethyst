using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
using System;

[Serializable]
public class MyEvent : UnityEvent { }

public class DynamicButton : MonoBehaviour
{
    [Header("The callback of the button")]
    public MyEvent eventCall;

    // Start is called before the first frame update
    [Header("Springs")]
    public SpringJoint[] springs;
    public float d, k, minDistance;
    private Rigidbody _rb;
    AudioSource audioS;
    public float min=0.75f, max=1.25f;


    float elapsed;
    [Header("Interactuable time")]
    public float timeToMove = 2.5f;




    void Start()
    {
        audioS = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody>();
        foreach (SpringJoint js in springs)
        {
            js.damper = d;
            js.spring = k;

            js.minDistance = minDistance;
        }
    }

    private void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);
        localVelocity.x = 0;
        localVelocity.z = 0;

        _rb.velocity = transform.TransformDirection(localVelocity);

        //clamp  position
        float clampedY = Mathf.Clamp(transform.localPosition.y, min, max);

        transform.localPosition = new Vector3(0,clampedY,0);

    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("button") && elapsed>0.5f)
        {
            audioS.Play();

            //Debug.Log(transform.parent.name);

            //call actions
            eventCall.Invoke();
        }
    }


    
}
