using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TimeDisplay {realTime, gameTime };

public class PDA : MonoBehaviour
{
    // Start is called before the first frame update
    public TimeDisplay timeDisp;

    //the external UI elements
    public Image heart;
    public Image circle;
    public Text healthTxt;
    public Text hour;

    // movement of the UI elements
    public float healthSpeed=0.01f;
    public float circleSpeed=0.05f;
    float elapsed;

    public Image loadingHability;
    public Image[] habilityIcons;

    public HabilityManager habScript;

    void Start()
    {
        

        if (habilityIcons != null)
        {
            for (int ii = 0; ii < habilityIcons.Length; ii++)
            {
                habilityIcons[ii].enabled = false;
            }

            if (habilityIcons.Length>0)
            {
                habilityIcons[PlayerInfo.PI.myHability].enabled = true;
            }
        }
            

        elapsed = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //dispay hability loading bar
        if (loadingHability != null && habScript!=null)
        {
            loadingHability.fillAmount = habScript.fillAmount;
        }

        //set elapsed
        elapsed += Time.fixedDeltaTime*healthSpeed;


        if (TimeDisplay.realTime==timeDisp)
        {
            //get time data
            DateTime currentTime = System.DateTime.Now;
            int hourR = currentTime.Hour;
            int minuteR = currentTime.Minute;

            //strings holding the hour
            string hourSt, minuteSt;

            //set the text 0+value  or value
            if (minuteR < 10)
            {
                minuteSt = "0" + minuteR;
            }
            else
            {
                minuteSt = "" + minuteR;
            }

            if (hourR < 10)
            {
                hourSt = "0" + hourR;
            }
            else
            {
                hourSt = "" + hourR;
            }


            //save the text and display it
            hour.text = hourSt + ":" + minuteSt;
        }
        else if (TimeDisplay.gameTime == timeDisp)
        {

            //save the text and display it
            hour.text = TimeManager.TM.minStr + ":" + TimeManager.TM.secStr;
        }



        //rotate circle
        circle.transform.localRotation = Quaternion.Euler(0, 0, circleSpeed * Time.fixedTime);

        //simulate heartbeat
        heart.fillAmount = elapsed;


        if (elapsed > 1)
        {
            elapsed = 0;
        }


        if (PhotonNetwork.InRoom)
        {
            try
            {
                int health = transform.root.GetComponent<PlayerHealth>().health;
                //color in function of health

                heart.color = new Color(1 - (float)health / 100, (float)health / 100, 0, 1);
                healthTxt.text = "HEALTH: " + health;
            }
            catch
            {

            }

        }

        


    }
}
