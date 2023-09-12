using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class VoiceManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Recorder recorder;
    float elapsed1, elapsed2;
    public PunVoiceClient voiceNet;

    PhotonView PV;
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            voiceNet.enabled = true;
            recorder.TransmitEnabled = true;
            recorder.RecordingEnabled = true;
            recorder.RestartRecording();
            elapsed1 = 10000;
            elapsed2 = 10000;
        }
        else
        {
            voiceNet.enabled = false;
            recorder.TransmitEnabled = false;
            recorder.RecordingEnabled = false;
            recorder.RecordingEnabled = false;
        }
      

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (PV.IsMine)
        {
            elapsed1 += Time.fixedDeltaTime;
            elapsed2 += Time.fixedDeltaTime;

            if (elapsed1 > 20)
            //if (recorder != null && recorder.enabled && !recorder.TransmitEnabled)
            {
                //Debug.Log("Turn on Transmit");
                recorder.TransmitEnabled = false;
                recorder.RecordingEnabled = false;
                recorder.RecordingEnabled = false;
                elapsed1 = 0;
                //recorder.VoiceDetection = true;
            }
            if (elapsed2 > 20f)
            {
                //Debug.Log("Turn on Transmit");
                recorder.TransmitEnabled = true;
                recorder.RecordingEnabled = true;
                recorder.RecordingEnabled = true;
                elapsed2 = 0;
            }
        }
    }

    private void OnDestroy()
    {
        if (voiceNet.Client.IsConnected)
        {
            voiceNet.Disconnect();
        }
    }
}
