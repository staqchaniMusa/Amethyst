using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// USED TO REPLICATE ANIMATIONS ON THE PHOTON SERVER
/// </summary>
public class ReplicateAnimations : MonoBehaviour
{
    [Header("Animator to replicate and animation objective")]
    public Animator animR;
    public Animator animO;
    public SkinnedMeshRenderer meshRenderer;
   
    // Start is called before the first frame update
    void Start()
    {
        // do not show the photon hands if it is mine
        if(GetComponent<PhotonView>().IsMine)
        {
           meshRenderer.enabled = false;
        }

       
    }

    // Update is called once per frame
    void Update()
    {
        //the animation parameters are: 
        if (GetComponent<PhotonView>().IsMine)
        {
            animO.SetFloat("grab", animR.GetFloat("grab"));
            animO.SetFloat("pick", animR.GetFloat("pick"));
        }

       
    }


}
