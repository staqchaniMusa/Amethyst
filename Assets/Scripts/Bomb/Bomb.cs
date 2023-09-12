using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class Bomb : MonoBehaviour
{
    [Header("Password to activate bomb")]
    public int passLength=8;
    public string password;

    [Header("This is the code the users selects")]
    public string enterCode;

    PhotonView PV;

    [Header("UI elements")]
    public Text displayPass;
    public Text displayCode;
    public Text counter;

    [Header("Readable variables")]
    public bool active;
    public bool explode;
    public bool snapped = false;
    float elapsed;

    [Header("Time to explode (s)")]
    public int explodeTimeSeconds = 45;

    public ParticleSystem partc;

    Collider _col;
    Rigidbody _rb;


    


    // Start is called before the first frame update
    void Start()
    {
        elapsed = 0;
        PV = GetComponent<PhotonView>();

        _col = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        password = "";

        //generate random password and syncro
        if (PhotonNetwork.IsMasterClient)
        {
            for (int ii = 0; ii < passLength; ii++)
            {
                password += Random.Range(0, 10);
            }
            PV.RPC("RPC_ChangePassword", RpcTarget.AllBuffered, password);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(active)
        {
            
            elapsed += Time.fixedDeltaTime;

            int totalSeconds = explodeTimeSeconds - (int)elapsed;
            int min = totalSeconds / 60;
            int sec = totalSeconds % 60;

            string minStr = "";
            string secStr = "";

            // generate string with time
            if(min<10)
            {
                minStr = "0" + min;
            }
            else
            {
                minStr = ""+min;
            }

            if (sec < 10)
            {
                secStr = "0" + sec;
            }
            else
            {
                secStr = "" + sec;
            }


            counter.text = minStr + " : " + secStr;
      
            //explode if time is finished
            if (elapsed> explodeTimeSeconds && explode==false)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PV.RPC("RPC_Explode", RpcTarget.AllBuffered);
                    
                }

                GameObject.FindGameObjectWithTag("bombManager").GetComponent<BombManager>().ChangeTeamTurn();
            }
        }
        
    }


    /// <summary>
    /// syncro the code
    /// </summary>
    /// <param name="pass"></param>
    [PunRPC]
    public void RPC_ChangePassword(string pass)
    {
        password = pass;
        displayPass.text = password;
        displayCode.text = enterCode;

    }

    /// <summary>
    /// Called when a button of the bomb is pressed
    /// </summary>
    /// <param name="pass"></param>
    public void AddNumber(int nb)
    {
        PV.RPC("RPC_AddNumber", RpcTarget.AllBuffered, nb);
    }

    /// <summary>
    /// called from the previous function in poton server
    /// </summary>
    /// <param name="nb"></param>
    [PunRPC]
    public void RPC_AddNumber(int nb)
    {
        enterCode +=nb;
        displayCode.text = enterCode;

        if(enterCode.Length>passLength)
        {
            PV.RPC("RPC_ClearBomb", RpcTarget.AllBuffered);
        }

    }

    /// <summary>
    /// clears the code 
    /// </summary>
    public void ClearBomb()
    {
        PV.RPC("RPC_ClearBomb", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_ClearBomb()
    {
        enterCode = "";
        if (!active)
        {
            displayCode.text = "ERROR";
        }
        else
        {
            displayCode.text = "ACTIVE";
        }

    }

    /// <summary>
    /// activates the bomb
    /// </summary>
    public void ActivateBomb()
    {

        if (enterCode == password && snapped)
        {
            active = !active;

            PV.RPC("RPC_ActivateBomb",RpcTarget.AllBuffered, active);
        }
        else
        {
            ClearBomb();
        }

       
    }

    [PunRPC]
    public void RPC_ActivateBomb(bool act)
    {
        active = act;

    }

    /// <summary>
    /// explodes the bomb using a photon call
    /// </summary>
    [PunRPC]
    public void RPC_Explode()
    {
        partc.Play();
        explode = true;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        //when the bomb can be snnaped in a specific box collider.
        if(other.tag=="snapBomb")
        {
            ObjectGrabbing grabbingScp = GetComponent<ObjectGrabbing>();

            if (grabbingScp.handGrabScp)
            {
                grabbingScp.handGrabScp.objectInHand = null;
                grabbingScp.handGrabScp.potentialOnjectInHand.Clear();
            }

            // RELEASING PROCESS

            //_col.enabled = false;
            _col.isTrigger = false;
            _col.enabled = false;
            _rb.useGravity = false;
            _rb.isKinematic = true;

            other.GetComponent<Renderer>().enabled = false;

            transform.position = other.transform.position;
            transform.forward = -other.transform.forward;

            grabbingScp.enabled = false;
            grabbingScp.SetRendHandEnabled(false, "handLeft");
            grabbingScp.SetRendHandEnabled(false, "handRight");

            snapped = true;

        }
    }



}
