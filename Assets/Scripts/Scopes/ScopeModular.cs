using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeModular : MonoBehaviour
{
    [Header("Collision")]
    public AudioClip colisionClip;
    public string insertTag = "scopePositionRifle";
    float elapsed = 0;
    DynamicRifle rifleScp;
    ObjectGrabbing grabbingScp;

    [Header("Scope config")]
    public int weaponIndex;
    public int scopeIndex;


    // Start is called before the first frame update
    void Start()
    {        
        grabbingScp = GetComponent<ObjectGrabbing>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;

        if( grabbingScp.handGrabScp!=null || rifleScp!=null )
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(insertTag) && elapsed > 0.5f)
        {
            elapsed = 0;
            

            //attach the scope to the gun 
            GameObject gun = other.gameObject.transform.parent.gameObject;
            rifleScp = gun.GetComponent<DynamicRifle>();

            if (rifleScp.selectedScope==null && grabbingScp.handGrabScp!=null)
            {
                if (rifleScp.weaponIndex == weaponIndex)
                {
                    PlayerInfo.PI.myscope = scopeIndex;

                    rifleScp.selectedScope = gameObject;
                    transform.position = rifleScp.scopeTf.position;
                    transform.rotation = rifleScp.scopeTf.rotation;
                    transform.SetParent(rifleScp.transform);

                    //release scope
                    if (grabbingScp != null)
                    {
                        GetComponent<Collider>().isTrigger = true;
                        grabbingScp.handGrabScp.objectInHand = null;

                        grabbingScp.rendHand_L.SetActive(false);
                        grabbingScp.rendHand_R.SetActive(false);

                        grabbingScp.handGrabScp.rend.enabled = true;

                        if (grabbingScp.handGrabScp.watch)
                        {
                            grabbingScp.handGrabScp.watch.SetActive(true);
                        }
                    }
                }

            }



        }


    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(insertTag) && elapsed > 0.5f)
        {
            elapsed = 0;
            GetComponent<Collider>().isTrigger = false;

            //dettach the scope to the gun 
            GameObject gun = other.gameObject.transform.parent.gameObject;
            rifleScp = gun.GetComponent<DynamicRifle>();

            if (rifleScp.selectedScope == gameObject)
            {
                rifleScp.selectedScope = null;
                //PlayerInfo.PI.myscope = -1;
            }

         


        }


    }
}
