using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public GameObject[] VRObjects;
    public GameObject[] PCObjects;

    private void Start()
    {
        if (PhotonLobby.lobby.isVR)
        {
            EnableVR(true);
        }
        else EnablePC(true);
    }

    void EnableVR(bool activate)
    {
        foreach (var item in VRObjects)
        {
            item.SetActive(activate);
        }

        if (activate)
            EnablePC(false);
    }

    void EnablePC(bool activate)
    {
        foreach (var item in PCObjects)
        {
            item.SetActive(activate);
        }

        if (activate) EnableVR(false);

    }
}
