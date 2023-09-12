using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// used to load pistol
/// </summary>
public class Magazine : MonoBehaviour
{
    public enum TypeOfMagazine { pistol, rifle};

    public TypeOfMagazine typeMag;

    [Header("Networking")]
    public bool isNetworkVisible = true;

    [Header("Bullets")]
    public Text[] bulletTexts;
    public int initialBullets=31;
    public int actualBullets=31;
    public GameObject[] bulletMesh;
       
    [Header("Expel")]
    public float expelSpeed=0.25f;
    public AudioClip expelClip;
    public float impactSpeed = 1.5f;

    [Header("Collision")]
    public AudioClip colisionClip;
    public string insertTag = "pistolInsertClip";


    //privtae
    private AudioSource _audioSrc;
    // Start is called before the first frame update

    private Collider _col;
    private Rigidbody _rb;
    //grabbing
    private ObjectGrabbing grabbingScp;
    public bool isInHand;
    public bool attached = false;
    public bool isLose = false;

    private float elapsed=100;

    DynamicPistol pistolScp = null;
    DynamicRifle rifleScp = null;
    ShotGun shotgunScp=null;

    PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    void Start()
    {
        attached = false;

        actualBullets = initialBullets;

        _rb = GetComponent<Rigidbody>();

        _audioSrc = GetComponent<AudioSource>();

        _col = GetComponent<Collider>();

        grabbingScp = GetComponent<ObjectGrabbing>();
    }

    // Update is called once per frame
    void Update()
    {
        if(attached==false)
        {
            _col.enabled = true;
            grabbingScp.enabled = true;
        }
        else
        {
            _col.enabled = false;
            grabbingScp.enabled = false;
        }

        //NEtworking visible
        if (!PV.IsMine && PhotonNetwork.InRoom)
        {
            if (!isNetworkVisible)
            {
                for (int ii = 0; ii < transform.childCount; ii++)
                {
                    transform.GetChild(ii).gameObject.SetActive(false);
                }
                _col.enabled = false;
            }
            else
            {
                for (int ii = 0; ii < transform.childCount; ii++)
                {
                    transform.GetChild(ii).gameObject.SetActive(true);
                }
                _col.enabled = true;
            }
        }

        //check if it is in hand
        if (grabbingScp.handGrabScp!=null)
        {
            isInHand = grabbingScp.handGrabScp.objectInHand == gameObject;
        }
        else
        {
            isInHand = false;
        }

        elapsed += Time.deltaTime;

        //check if is dettached/attached
        /*bool dettachCondition = isInHand
                                       && (InputManager.instance.G_R_UP&& grabbingScp.handGrabScp.CompareTag("handRight")
                                      || InputManager.instance.G_L_UP && grabbingScp.handGrabScp.CompareTag("handLeft"));
        if (dettachCondition)
        {
            attached = false;
        }*/


        //color of bullet text
        if (actualBullets == 0)
        {
            foreach (Text bt in bulletTexts)
            {
                bt.color = Color.red;
            }
            foreach (GameObject bm in bulletMesh)
            {
                bm.SetActive(false);
            }
        }
        else
        {
            foreach (Text bt in bulletTexts)
            {
                bt.color = Color.white;
            }

            foreach (GameObject bm in bulletMesh)
            {
                bm.SetActive(true);
            }

        }
        foreach (Text bt in bulletTexts)
        {
            bt.text = "" + actualBullets;
        }


        if(pistolScp!=null)
        {
            transform.position = pistolScp.insertClipTf.position;
            transform.rotation = pistolScp.insertClipTf.rotation;
        }

        if (rifleScp != null)
        {
            transform.position = rifleScp.insertClipTf.position;
            transform.rotation = rifleScp.insertClipTf.rotation;
            transform.SetParent(rifleScp.insertClipTf);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(insertTag) && elapsed>0.5f)
        {
            elapsed = 0;


            if (typeMag == TypeOfMagazine.pistol)
            {
                //attach the clip to the gun 
                GameObject gun = other.gameObject.transform.parent.parent.gameObject;

                pistolScp = gun.GetComponent<DynamicPistol>();

                if (pistolScp!=null)
                {
                    if (pistolScp.pistolClipScp == null)
                    {
                        attached = true;
                        //not working in new photon version
                        //transform.SetParent(gun.transform);
                        //transform.GetComponent<PhotonAbsoluteRepositioning>().tf = pistolScp.insertClipTf;

                        grabbingScp.handGrabScp.objectInHand = null;

                        pistolScp.pistolClipScp = this;
                        //_col.enabled = false;
                        _col.isTrigger = true;
                        _rb.useGravity = false;
                        _rb.isKinematic = true;

                        transform.position = pistolScp.insertClipTf.position;
                        transform.rotation = pistolScp.insertClipTf.rotation;

                        pistolScp.insertClipTf.GetComponent<Collider>().enabled = false;

                        pistolScp.Reload();

                        if (PV.IsMine)
                        {
                            grabbingScp.rendHand_L.SetActive(false);
                            grabbingScp.rendHand_R.SetActive(false);

                            grabbingScp.handGrabScp.rend.enabled = true;
                            grabbingScp.handGrabScp.watch.SetActive(true);

                            grabbingScp.handGrabScp = null;
                        }

                        if (!PV.IsMine)
                        {
                            PV.RequestOwnership();
                        }

                    }
                }
            }
            else if(typeMag == TypeOfMagazine.rifle)
            {

                //attach the clip to the gun 
                GameObject gun = other.gameObject.transform.parent.parent.gameObject;


                if(gun.GetComponent<DynamicRifle>())
                {
                    rifleScp = gun.GetComponent<DynamicRifle>();
                }
                else if(gun.GetComponent<ShotGun>())
                {
                    shotgunScp= gun.GetComponent<ShotGun>();

                }

                if (rifleScp)
                {
                    if (rifleScp.rifleMag == null)
                    {
                        attached = true;

                        //not working in new photon
                        //transform.SetParent(gun.transform);
                        //transform.GetComponent<PhotonAbsoluteRepositioning>().tf = (rifleScp.insertClipTf);
                        if (grabbingScp.handGrabScp != null)
                        {
                            grabbingScp.handGrabScp.objectInHand = null;
                        }


                        rifleScp.rifleMag = this;
                        _col.isTrigger = true;
                        //_col.enabled = false;
                        _rb.useGravity = false;
                        _rb.isKinematic = true;

                        transform.position = rifleScp.insertClipTf.position;
                        transform.localRotation = Quaternion.Euler(0, 0, 0);

                        rifleScp.insertClipTf.GetComponent<Collider>().enabled = false;

                        rifleScp.Reload();

                        if (PV.IsMine)
                        {
                            grabbingScp.rendHand_L.SetActive(false);
                            grabbingScp.rendHand_R.SetActive(false);

                            grabbingScp.handGrabScp.rend.enabled = true;
                            if (grabbingScp.handGrabScp.watch)
                            {
                                grabbingScp.handGrabScp.watch.SetActive(true);
                            }

                            grabbingScp.handGrabScp = null;

                        }


                        if (!PV.IsMine)
                        {
                            PV.RequestOwnership();
                        }

                    }
                }
                else if(shotgunScp)
                {
                   
                       
                                        //not working in new photon
                    //transform.SetParent(gun.transform);
                    //transform.GetComponent<PhotonAbsoluteRepositioning>().tf = (rifleScp.insertClipTf);
                    if (grabbingScp.handGrabScp != null)
                    {
                        grabbingScp.handGrabScp.objectInHand = null;
                    }

                    _col.isTrigger = true;
                    //_col.enabled = false;
                    _rb.useGravity = false;
                    _rb.isKinematic = true;

                    transform.position = shotgunScp.insertClipTf.position;
                    transform.localRotation = Quaternion.Euler(0, 0, 0);

                    

                    shotgunScp.Reload();

                    if (PV.IsMine)
                    {
                        grabbingScp.rendHand_L.SetActive(false);
                        grabbingScp.rendHand_R.SetActive(false);

                        grabbingScp.handGrabScp.rend.enabled = true;
                        grabbingScp.handGrabScp.watch.SetActive(true);

                        grabbingScp.handGrabScp = null;

                    }



                    if (!PV.IsMine)
                    {
                        PV.RequestOwnership();
                    }

                    PhotonNetwork.Destroy(gameObject);
                    
                }

            }

                      
                       
            //Debug.Log("Inserted Clip");
        }


        //hits another magazine
    
        /*if (other.gameObject.GetComponent<Magazine>() != null)
        {
            Debug.Log(other.gameObject.name);
            if (grabbingScp.handGrabScp.filteredSpeed.magnitude > impactSpeed)
            {

                if (other.gameObject.GetComponent<Magazine>().rifleScp != null)
                {
                    other.gameObject.GetComponent<Magazine>().rifleScp.rifleMag = null;
                }

                other.gameObject.GetComponent<Magazine>().Dettach();

                other.gameObject.GetComponent<Rigidbody>().velocity = grabbingScp.handGrabScp.filteredSpeed;

                //DynamicRifle gunScript = other.gameObject.transform.parent.parent.gameObject.GetComponent<DynamicRifle>();

            }
        }
        */



    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        _audioSrc = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody>();
        if (collision.gameObject.tag=="Untagged")
        {
            _audioSrc.clip = colisionClip;
            _audioSrc.Play();

        }
        
        if(collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("environment"))
        {
            GetComponent<DestroyAfterTime>().enabled = true;
        }



        _rb.useGravity = true;
                

    }



    public void Dettach()
    {
        isLose = true;
        attached = false;

        transform.SetParent(null);

        _audioSrc.clip = expelClip;
        _audioSrc.Play();

        _rb.velocity = -transform.forward * expelSpeed;
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _col.isTrigger = false;
        //grabbingScp.isGrabbable = true;

        _col.enabled =false;
        gameObject.layer = grabbingScp.maskDefault;

        Invoke("ReactivateCollider", 0.05f);

        //not working in PHOTON
        //transform.SetParent(null);
        //transform.GetComponent<PhotonAbsoluteRepositioning>().tf = null;
        
        if(pistolScp!=null)
        {
            pistolScp.insertClipTf.GetComponent<Collider>().enabled = true;
        }
        
        if(rifleScp!=null)
        {
            rifleScp.insertClipTf.GetComponent<Collider>().enabled = true;
        }

        pistolScp = null;
        rifleScp = null;

        elapsed = 0;
    }

    public void ReactivateCollider()
    {
        _col.enabled = true;
        gameObject.layer = grabbingScp.maskDefault;
    }
    public void SetNetWorkVisible(bool b)
    {
        PV.RPC("RPC_SetNetVis", RpcTarget.AllBuffered, b);
    }

    [PunRPC]
    public void RPC_SetNetVis(bool b)
    {
        isNetworkVisible = b;

    }

}
