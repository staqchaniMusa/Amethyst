using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

/// <summary>
/// maanages the abilities in-game
/// </summary>
public class HabilityManager : MonoBehaviour
{
    [Header("Container with the transparent renders")]
    public GameObject objectContainer;

    [Header("Hand to create the linerend")]
    public Transform hand;
    Vector3 dirHand;
    Vector3 dirProyection;
    LineRenderer lineR;

    [Header("Line rend param")]
    public float lineRendIncrement = 0.2f;
    public float vmax = 2;
    public float maxTime = 10;
    public float acceleration = 5;
    public Vector3 target;
    public string[] photonPrefabs;

    [Header("Ability param [!]usingTime<habilityTime[!]")]
    public float habilityTime = 25;
    public float usingTime =10;
    float elapsed;
    public float fillAmount;
    public GameObject habilityObject;
    public GameObject childObject;

    PlayerHealth playerHealth;

    float vx, vy;

    
    public bool active = false;


    // Start is called before the first frame update
    void Start()
    {
        playerHealth = transform.root.GetComponent<PlayerHealth>();
        elapsed = 1000;
        lineR = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Activate or de-activate to show it in game
    /// </summary>
    public void EnaleShowSpanwHability()
    {
        if (elapsed > habilityTime)
        {
            active = !active;

            for (int ii = 0; ii < transform.childCount; ii++)
            {
                transform.GetChild(ii).gameObject.SetActive(false);
            }
            transform.GetChild(0).GetChild(PlayerInfo.PI.myHability).gameObject.SetActive(true);
        }

    }
    private void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;

        // Destroys the ability after a time
        if(habilityObject!=null && elapsed> usingTime )
        {
            if(childObject)
                PhotonNetwork.Destroy(childObject);
            if(habilityObject)
                PhotonNetwork.Destroy(habilityObject);
        }

        fillAmount = (float)elapsed / (float)habilityTime;


    }

    // Update is called once per frame
    void Update()
    {
        
    

        if (active && playerHealth.health>0)
        {
            lineR.enabled = true;
            //get vector of hand
            dirHand = hand.forward;

            //it spoyection
            dirProyection = dirHand;
            dirProyection.y = 0;

            //angle between both
            float angle = -Vector3.SignedAngle(dirProyection, dirHand, hand.right) * Mathf.PI / 180.0f;

            vx = vmax * Mathf.Cos(angle);
            vy = vmax * Mathf.Sin(angle);

            //create line render positions
            List<Vector3> pos = new List<Vector3>();

            objectContainer.SetActive(false);

            for (float t = 0; t < maxTime; t += lineRendIncrement)
            {
                //actual position of the linerenderer point
                pos.Add(hand.position + vx * t * hand.forward + (vy * t - 0.5f * acceleration * Mathf.Pow(t, 2)) * hand.up);

                //ray direction
                Vector3 rayDir = vx * hand.forward + (vy - acceleration * t) * hand.up;

                //perform raycasting          
                Ray ry = new Ray(pos[pos.Count - 1], rayDir);
                RaycastHit hit;
                if (Physics.Raycast(ry, out hit, 0.25f))
                {
                    //get hit point
                    if (hit.collider.gameObject.tag=="ground"
                        || hit.collider.gameObject.tag == "environmnet"
                        && Vector3.Angle(hit.normal,Vector3.up)<20)
                    {
                        target = hit.point;
                        
                        objectContainer.SetActive(true);
                        objectContainer.transform.position = target;

                        objectContainer.transform.rotation= Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x,0, Camera.main.transform.forward.z),hit.normal);

                        t = maxTime;

                        //check if pressing trigger
                        if (InputManager.instance.T_R_DW && elapsed > habilityTime)
                        {
                            habilityObject=PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", photonPrefabs[PlayerInfo.PI.myHability]), objectContainer.transform.position, objectContainer.transform.rotation);
                            if (habilityObject.transform.childCount>0 )
                            {
                                if (habilityObject.transform.GetChild(0).GetComponent<PhotonView>())
                                {
                                    childObject = habilityObject.transform.GetChild(0).gameObject;
                                }
                            }
                            elapsed = 0;
                            active = false;
                        }


                    }
                }
                


            }

            //set line renderer points
            lineR.positionCount = pos.Count;

            for (int ii = 0; ii < pos.Count; ii++)
            {
                lineR.SetPosition(ii, pos[ii]);
            }
        }
        else
        {
            objectContainer.SetActive(false);
            lineR.enabled = false;
        }
    }

    /// <summary>
    /// show the linerend
    /// </summary>
    /// <param name="tg"></param>
    public void SetActiveTeleport(Toggle tg)
    {
        if (playerHealth)
        {
            active = tg.isOn;
        }
    }
}

