using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// shot gun two-handed grabbing
/// </summary>
public class SliderSecondGrab : MonoBehaviour
{

    private ShotGun shotGunSc;

    [Header("Renders")]
    public GameObject rendHand_L;
    public GameObject rendHand_R;
    PhotonView PV;
    float projection;

    [Header("State")]
    public bool opened, closed;
    Vector3 newClosedPos, newOpenPos;
    Vector3 refDirection;
    AudioSource audioS;

    [Header("Sounds")]
    public AudioClip soundOpen, soundClose;

    [Header("Slider positions and limits")]
    public Transform openSlider, closeSlider;
    public float sliderTime = 0.15f;
    public float offsetHandle;
    bool moving = false;
    GameObject handRef;


    // Start is called before the first frame update
    void Start()
    {
        PV = transform.root.GetComponent<PhotonView>();
        rendHand_L.SetActive(false);
        rendHand_R.SetActive(false);

        audioS = GetComponent<AudioSource>();
        shotGunSc = transform.root.GetComponent<ShotGun>();

        //start closed
        StartCoroutine(CloseSlider());
        
    }

    // Update is called once per frame
    void Update()
    {
        //this updates the relative position regarding the hands
        UpdateRelPos();

        if (InputManager.instance.G_R_UP || InputManager.instance.G_L_UP)
        {
            
            if (shotGunSc)
            {
                //moving
                if (shotGunSc.objectGrabbingScript.handGrabScp)
                {
                    shotGunSc.objectGrabbingScript.handGrabScp.otherHand.rend.enabled = true;
                    if (shotGunSc.objectGrabbingScript.handGrabScp.otherHand.watch != null)
                    {
                        shotGunSc.objectGrabbingScript.handGrabScp.otherHand.watch.SetActive(true);
                        shotGunSc.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = false;
                    }
                }
                moving = false;
                shotGunSc.sliderPart = null;
                handRef = null;

                if (opened)
                {
                    opened = false;
                    StartCoroutine(CloseSlider());
                    closed = true;
                }
            }
          

            rendHand_L.SetActive(false);
            rendHand_R.SetActive(false);



        }

    }

    private void LateUpdate()
    {
        if (!PV.IsMine)
        {
            rendHand_L.SetActive(false);
            rendHand_R.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // if the the user is holding thr grabbing button in the collider
        if (shotGunSc)
        {
            if ((other.gameObject.CompareTag("handRight") && shotGunSc.objectGrabbingScript.handGrabScp
                && (InputManager.instance.G_R_DW))
                || (other.gameObject.CompareTag("handLeft")
                && (InputManager.instance.G_L_DW)))

            {
                if ((shotGunSc.objectGrabbingScript.handGrabScp.CompareTag("handLeft")
                && (InputManager.instance.G_R_DW))
                || (other.gameObject.CompareTag("handLeft")
                && shotGunSc.objectGrabbingScript.handGrabScp.CompareTag("handRight")
                && (InputManager.instance.G_L_DW)))
                {
                    if (shotGunSc)
                    {
                        shotGunSc.sliderPart = this;
                    }


                    if (PV.IsMine)
                    {
                        SetRendHandEnabled(true, other.gameObject.tag);
                    }
                    else
                    {
                        SetRendHandEnabled(false, other.gameObject.tag);
                    }


                    shotGunSc.objectGrabbingScript.handGrabScp.otherHand.rend.enabled = false;


                    shotGunSc.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = true;
               
                    

                    if (shotGunSc.objectGrabbingScript.handGrabScp.otherHand.watch != null)
                    {
                        shotGunSc.objectGrabbingScript.handGrabScp.otherHand.watch.SetActive(false);
                    }

                    moving = true;
                    handRef = shotGunSc.objectGrabbingScript.handGrabScp.otherHand.gameObject;
                }
            }
        }

        if (moving && handRef!=null)
        {
            shotGunSc.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = true;
            MoveSlider(handRef.gameObject);
        }
       

    }

    /// <summary>
    /// recalculate direction
    /// </summary>
    public void UpdateRelPos()
    {
        //the direction considered for the dot product
        refDirection = closeSlider.position - openSlider.position;

        newOpenPos = openSlider.position - refDirection.normalized * offsetHandle;
        newClosedPos = closeSlider.position - refDirection.normalized * offsetHandle;
    }

    /// <summary>
    /// closes the moving part of the shotgun
    /// </summary>
    /// <returns></returns>
    public IEnumerator CloseSlider()
    {
        UpdateRelPos();

        for (float ii = 0; ii < sliderTime / 2; ii += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(newOpenPos
                , newClosedPos, 2 * ii / sliderTime);

            yield return new WaitForFixedUpdate();
        }

        audioS.clip = soundClose;
        audioS.Play();
        transform.position = newClosedPos;

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("handRight")
           || other.gameObject.CompareTag("handLeft"))
        {
            if (shotGunSc)
            {
                if (shotGunSc.objectGrabbingScript.handGrabScp)
                {
                    shotGunSc.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = false;
                    //shotGunSc.sliderPart = null;
                }

                
            }
          
            SetRendHandEnabled(false, other.gameObject.tag);



        }
    }

    /// <summary>
    /// display or not the hands
    /// </summary>
    /// <param name="b"></param>
    /// <param name="name"></param>
    public void SetRendHandEnabled(bool b, string name)
    {
        if (name == "handLeft")
        {
            rendHand_L.SetActive(b);
        }
        else if (name == "handRight")
        {
            rendHand_R.SetActive(b);
        }
    }

    /// <summary>
    /// moves the slider between limits
    /// </summary>
    /// <param name="other"></param>
    public void MoveSlider(GameObject other)
    {

        //UpdateRelPos();

        //the direction relative to the hand and the initial open slider
        Vector3 relative = other.transform.position - openSlider.position;

        projection = Vector3.Dot(relative, refDirection.normalized);

        float pos = projection / refDirection.magnitude;

        transform.position = Vector3.Lerp(newOpenPos, newClosedPos, pos);


        // limit position
        if (pos > 1)
        {
            opened = false;
            transform.position = newClosedPos;

            if (closed == false)
            {
                closed = true;
                
                audioS.clip = soundClose;
                audioS.Play();
            }
        }
        else if (pos < 0)
        {

            closed = false;
            transform.position = newOpenPos;

            if (opened == false)
            {
                opened = true;
                shotGunSc.FeedChamberNormal();

                audioS.clip = soundOpen;
                audioS.Play();
            }
        }
    }


    private void OnDestroy()
    {
        if (shotGunSc)
        {
            if (shotGunSc.objectGrabbingScript.handGrabScp)
            {
                shotGunSc.objectGrabbingScript.handGrabScp.otherHand.isGrabbingSecondary = false;
            }

            shotGunSc.sliderPart = null;
        }
        
    }
}
