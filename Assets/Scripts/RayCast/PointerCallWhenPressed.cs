using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointerCallWhenPressed : MonoBehaviour
{
    // Start is called before the first frame update
    CustomRayCaster raycaster;
    public GameObject selected;

    void Start()
    {
        raycaster = GameObject.FindObjectOfType<CustomRayCaster>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (selected!=null && (InputManager.instance.T_R_DW || Input.GetMouseButtonDown(0)))
        {
            if (selected.activeInHierarchy)
            {
                selected.GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Button>() != null )
        {
            selected = other.gameObject;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject==selected)
        {
            selected = null;
        }
    }


}
