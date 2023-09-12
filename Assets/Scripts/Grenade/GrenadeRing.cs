using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeRing : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Reference to the grenade script")]
    public Grenade grenadeScp;
    [Header("Button to release ring")]
    bool released = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag=="handLeft"|| other.tag == "handRight")
        {

            // used to activate the grenade when the user triggers on the ring
            bool ringCondition = false;

            if(other.tag == "handLeft")
            {
                ringCondition = InputManager.instance.T_L_DW;
            }
            else if(other.tag == "handRight")
            {
                ringCondition = InputManager.instance.T_R_DW;
            }


            // active
            if (ringCondition&&released==false)
            {
                Release();
                grenadeScp.Activate();

                released = true;


            }
        }
                    
    }

    /// <summary>
    /// release it
    /// </summary>
    public void Release()
    {
        Rigidbody rb=gameObject.AddComponent<Rigidbody>();

        rb.velocity = transform.right * 1.5f;

    }
}
