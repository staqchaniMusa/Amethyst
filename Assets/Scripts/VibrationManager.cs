using System.Collections;
using System.Collections.Generic;
using UnityEngine;

# if UNITY_STANDALONE_WIN
//using Valve.VR;
#endif

public class VibrationManager : MonoBehaviour
{
    //For OCULUS
    //public AudioClip vibClip;

    public static VibrationManager VM;
    // Start is called before the first frame update


    //STEAM VR vibration
    public float duration = 0.5f;
    public float frequency = 150;
    public float amplitude = 75;

#if UNITY_STANDALONE_WIN
   // public SteamVR_Action_Vibration vibrationAction;
#endif

    void Start()
    {
        if (VM && VM != this)
        {
            Destroy(this);
        }
        else
        {
            VM = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerVibration(string hand)
    {
        // FOR OCULUS
        /*OVRHapticsClip clip =new OVRHapticsClip(vibClip);

        OVRHaptics.LeftChannel.Preempt(clip);
        OVRHaptics.RightChannel.Preempt(clip);
        */
#if UNITY_STANDALONE_WIN
        if (hand == "handLeft")
        {

      //      vibrationAction.Execute(0, duration, frequency, amplitude, SteamVR_Input_Sources.LeftHand);

        }
        if (hand == "handRight")
        {

       //     vibrationAction.Execute(0, duration, frequency, amplitude, SteamVR_Input_Sources.RightHand);

    }
        #endif
    }
}