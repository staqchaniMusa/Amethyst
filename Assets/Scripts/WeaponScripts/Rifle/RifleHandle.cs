using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// performs the movement of the hande of the wepon
/// </summary>
public class RifleHandle : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("References")]
    public DynamicRifle rifleScript;

    [Header("Positions and limits")]
    public Transform openSlider, closeSlider;

    [Header("state")]
    public bool moving;
    public bool opened=false;
    public bool closed = true;

    GameObject handRef;
    //public Transform bone;
    public Transform pivot;
    //Vector3 pivotOffset;
    AudioSource audioS;

    [Header("Renders")]
    public HandGrabbing handScript;
    public GameObject rendR, rendL;

    [Header("Audio")]
    public AudioClip soundOpen, soundClose;

    public float offsetHandle;
    Vector3 refDirection;

    float sliderTime = 0.05f;
    Vector3 newClosedPos, newOpenPos;

    public float projection;

    void Start()
    {
        offsetHandle = (transform.position - closeSlider.position).magnitude;
        UpdateRelPos();        
        audioS = GetComponent<AudioSource>();
        rendR.SetActive(false);
        rendL.SetActive(false);

        //stated closed
        StartCoroutine(CloseSlider());
    }

    public void UpdateRelPos()
    {
        //the direction considered for the dot product
        refDirection = closeSlider.position - openSlider.position;
        
        newOpenPos = openSlider.position - refDirection.normalized * offsetHandle;
        newClosedPos = closeSlider.position - refDirection.normalized * offsetHandle;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRelPos();

        // if the user is triggering inside the collider
        if (InputManager.instance.T_L_UP || InputManager.instance.T_R_UP 
            || InputManager.instance.G_L_UP || InputManager.instance.G_R_UP)
        {
            moving = false;

            rendR.SetActive(false);
            rendL.SetActive(false);

            if(handScript)
            {
                handScript.rend.enabled = true;
                if (handScript.watch)
                {
                    handScript.watch.SetActive(true);
                }
                handScript.isGrabbingSecondary = false;
                handScript = null;
            }

            if(opened)
            {
                opened = false;
                StartCoroutine(CloseSlider());
                closed = true;
            }

        }

        if(moving)
        {
            MoveSlider(handRef.gameObject);
        }

    }

    private void OnTriggerStay(Collider other)
    {
       
        // set the handscrtip if the trigger is down
        if (rifleScript.objectGrabbingScript != null)
        {

            if (rifleScript.objectGrabbingScript.handGrabScp != null)
            {

                if (other.CompareTag("handRight") && rifleScript.objectGrabbingScript.handGrabScp.CompareTag("handLeft"))
                {
                    if (InputManager.instance.T_R_DW)
                    {
                        rendR.SetActive(true);
                        handRef = other.gameObject;
                        moving = true;

                        handScript = other.GetComponent<HandGrabbing>();

                        handScript.rend.enabled = false;
                        handScript.isGrabbingSecondary = true;

                        if (handScript.watch)
                        {
                            handScript.watch.SetActive(false);
                        }
                    }

                 

                }

                if (other.CompareTag("handLeft") && rifleScript.objectGrabbingScript.handGrabScp.CompareTag("handRight"))
                {
                    if (InputManager.instance.T_L_DW )
                    {
                        rendL.SetActive(true);
                        handRef = other.gameObject;
                        moving = true;

                        handScript = other.GetComponent<HandGrabbing>();
                        handScript.rend.enabled = false;
                        handScript.isGrabbingSecondary = true;

                        if (handScript.watch)
                        {
                            handScript.watch.SetActive(false);
                        }
                    }
                    
                }

            }

        }


    }


    public void MoveSlider(GameObject other)
    {

        //UpdateRelPos();

        //the direction relative to the hand and the initial open slider
        Vector3 relative = other.transform.position - openSlider.position;

        projection = Vector3.Dot(relative, refDirection.normalized);

        float pos = projection / refDirection.magnitude;

        transform.position = Vector3.Lerp(newOpenPos, newClosedPos, pos);

                

        if (pos > 1)
        {            
            opened = false;
            transform.position = newClosedPos;
                        
            if (closed==false)
            {
                closed = true;
                rifleScript.FeedChamberInitial();
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
                rifleScript.FeedChamberNormal();
                audioS.clip = soundOpen;
                audioS.Play();
            }
        }
    }


    public IEnumerator CloseSlider()
    {
        UpdateRelPos();

        for (float ii = 0; ii < sliderTime / 2; ii += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(newOpenPos
                ,newClosedPos, 2 * ii / sliderTime);

            yield return new WaitForFixedUpdate();
        }

        audioS.clip = soundClose;
        audioS.Play();

        rifleScript.FeedChamberInitial();
        transform.position =newClosedPos;
        
    }


    public void ShotSlider()
    {
        StartCoroutine(ShotSlider_co());
    }

    public IEnumerator ShotSlider_co()
    {
        

        rifleScript.sliderOnCorrutine = true;

        for (float ii = 0; ii < sliderTime / 2; ii += Time.deltaTime)
        {
            UpdateRelPos();
            transform.position = Vector3.Lerp(newClosedPos, newOpenPos, 2 * ii / sliderTime);

            yield return new WaitForFixedUpdate();
        }
        UpdateRelPos();
        transform.position = newOpenPos;

        //create bullet
        GameObject bulletInstance = Instantiate(rifleScript.bulletCasePrefab);
        bulletInstance.transform.position = rifleScript.expelTf.position;
        bulletInstance.transform.rotation *= Quaternion.Euler(Random.Range(-rifleScript.randomRotation, rifleScript.randomRotation),
                                                              Random.Range(-rifleScript.randomRotation, rifleScript.randomRotation), 
                                                              Random.Range(-rifleScript.randomRotation, rifleScript.randomRotation));
        bulletInstance.transform.forward = rifleScript.expelTf.forward;
        bulletInstance.GetComponent<Rigidbody>().velocity = (bulletInstance.transform.right
            + new Vector3(Random.Range(-rifleScript.randomDirection, rifleScript.randomDirection), 
                         Random.Range(-rifleScript.randomDirection, rifleScript.randomDirection), 
                         Random.Range(-rifleScript.randomDirection, rifleScript.randomDirection)))
            * rifleScript.expelSpeed;


        for (float ii = 0; ii < sliderTime / 2; ii += Time.deltaTime)
        {
            UpdateRelPos();
            transform.position = Vector3.Lerp(newOpenPos, newClosedPos, 2 * ii / sliderTime);

            yield return new WaitForFixedUpdate();
        }

        UpdateRelPos();
        transform.position = newClosedPos;
        rifleScript.sliderOnCorrutine = false;

    }



}
