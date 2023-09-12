using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// regenerates the life of the players inside it
/// </summary>
public class HealthRegen : MonoBehaviour
{
    

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag=="regen")
        {
            transform.root.GetComponent<PlayerHealth>().canRegenerate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "regen")
        {
            transform.root.GetComponent<PlayerHealth>().canRegenerate = false;
        }

    }
}
