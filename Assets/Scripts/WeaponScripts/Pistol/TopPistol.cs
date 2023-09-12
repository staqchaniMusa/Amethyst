using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopPistol : MonoBehaviour
{
    // Start is called before the first frame update
    public DynamicPistol pistolScript;
    public Transform openSlider, closeSlider;
    public bool moving;
    AudioSource audioS;
    GameObject handRef;

    //public Transform bone;
    public Transform pivot;
    //Vector3 pivotOffset;

    public HandGrabbing handScript;
    public GameObject rendR, rendL;

    public bool opened = false;
    public bool closed = true;

    public AudioClip soundOpen, soundClose;

    public float offsetHandle;
    Vector3 refDirection;

    float sliderTime = 0.05f;
    public Vector3 newClosedPos, newOpenPos;
    float projection;

    public void UpdateRelPos()
    {
        //the direction considered for the dot product
        refDirection = closeSlider.position - openSlider.position;

        newOpenPos = openSlider.position - refDirection.normalized * offsetHandle;
        newClosedPos = closeSlider.position - refDirection.normalized * offsetHandle;
    }

    void Start()
    {
        offsetHandle = (transform.position - closeSlider.position).magnitude;
        UpdateRelPos();
        audioS = GetComponent<AudioSource>();
        rendR.SetActive(false);
        rendL.SetActive(false);

        StartCoroutine(CloseSlider());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRelPos();

        if (InputManager.instance.T_L_UP || InputManager.instance.T_R_UP)
        {
            moving = false;

            rendR.SetActive(false);
            rendL.SetActive(false);

            if (handScript)
            {
                handScript.rend.enabled = true;

                if (handScript.watch)
                {
                    handScript.watch.SetActive(true);
                }

                handScript.isGrabbingSecondary = false;

                handScript = null;

                
            }

            if (opened)
            {
                opened = false;
                StartCoroutine(CloseSlider());
                closed = true;
            }

        }

        if (moving)
        {
            MoveSlider(handRef.gameObject);
        }

    }

    private void OnTriggerStay(Collider other)
    {

        if (pistolScript.objectGrabbingScript != null)
        {

            if (pistolScript.objectGrabbingScript.handGrabScp != null)
            {

                if (other.CompareTag("handRight") && pistolScript.objectGrabbingScript.handGrabScp.CompareTag("handLeft"))
                {
                    if (InputManager.instance.T_R_DW)
                    {
                        rendR.SetActive(true);
                        handRef = other.gameObject;
                        moving = true;

                        handScript = other.GetComponent<HandGrabbing>();
                        handScript.isGrabbingSecondary = true;
                        handScript.rend.enabled = false;
                        handScript.watch.SetActive(false);
                    }



                }

                if (other.CompareTag("handLeft") && pistolScript.objectGrabbingScript.handGrabScp.CompareTag("handRight"))
                {
                    if (InputManager.instance.T_L_DW)
                    {
                        rendL.SetActive(true);
                        handRef = other.gameObject;
                        moving = true;

                        handScript = other.GetComponent<HandGrabbing>();
                        handScript.rend.enabled = false;
                        handScript.watch.SetActive(false);
                        handScript.isGrabbingSecondary = true;
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

            if (closed == false)
            {
                closed = true;
                pistolScript.FeedChamberInitial();
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
                pistolScript.FeedChamberNormal();
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
                , newClosedPos, 2 * ii / sliderTime);

            yield return new WaitForFixedUpdate();
        }

        audioS.clip = soundClose;
        audioS.Play();

        pistolScript.FeedChamberInitial();
        transform.position = newClosedPos;

    }


    public void ShotSlider()
    {
        StartCoroutine(ShotSlider_co());
    }

    public IEnumerator ShotSlider_co()
    {


        pistolScript.sliderOnCorrutine = true;

        for (float ii = 0; ii < sliderTime / 2; ii += Time.deltaTime)
        {
            UpdateRelPos();
            transform.position = Vector3.Lerp(newClosedPos, newOpenPos, 2 * ii / sliderTime);

            yield return new WaitForFixedUpdate();
        }
        UpdateRelPos();
        transform.position = newOpenPos;

        //bullet
        GameObject bulletInstance = Instantiate(pistolScript.bulletCasePrefab);
        bulletInstance.transform.position = pistolScript.expelTf.position;
        bulletInstance.transform.rotation *= Quaternion.Euler(Random.Range(-pistolScript.randomRotation, pistolScript.randomRotation),
                                                              Random.Range(-pistolScript.randomRotation, pistolScript.randomRotation),
                                                              Random.Range(-pistolScript.randomRotation, pistolScript.randomRotation));
        bulletInstance.transform.forward = pistolScript.expelTf.forward;
        bulletInstance.GetComponent<Rigidbody>().velocity = (bulletInstance.transform.right
            + new Vector3(Random.Range(-pistolScript.randomDirection, pistolScript.randomDirection),
                         Random.Range(-pistolScript.randomDirection, pistolScript.randomDirection),
                         Random.Range(-pistolScript.randomDirection, pistolScript.randomDirection)))
            * pistolScript.expelSpeed;


        for (float ii = 0; ii < sliderTime / 2; ii += Time.deltaTime)
        {
            UpdateRelPos();
            transform.position = Vector3.Lerp(newOpenPos, newClosedPos, 2 * ii / sliderTime);

            yield return new WaitForFixedUpdate();
        }

        UpdateRelPos();
        transform.position = newClosedPos;
        pistolScript.sliderOnCorrutine = false;

    }

}
