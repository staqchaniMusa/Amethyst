using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWeaponMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        for (int ii = 0; ii < transform.childCount; ii++)
        {
            if (ii != PlayerInfo.PI.myWeapon)
            {
                transform.GetChild(ii).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(ii).gameObject.SetActive(true);
            }
        }


    }
}
