using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugCanvas : MonoBehaviour
{
    public static DebugCanvas DC;
    public Text debugText;

    public Text objectHandRightTxt;
    public Text objectHandLeftTxt;


    HandGrabbing leftGrabbingScp;
    HandGrabbing rightGrabbingScp;


    // Start is called before the first frame update
    void Start()
    {
        DC = this;

        //find hands
        GameObject leftHandTf = GameObject.FindGameObjectWithTag("handLeft");
        GameObject rightHandTf = GameObject.FindGameObjectWithTag("handRight");

        leftGrabbingScp = leftHandTf.GetComponent<HandGrabbing>();
        rightGrabbingScp = rightHandTf.GetComponent<HandGrabbing>();

    }

    // Update is called once per frame
    void Update()
    {
        if (rightGrabbingScp.objectInHand == null)
        {
            objectHandRightTxt.text = "null";

        }
        else
        {
            objectHandRightTxt.text = rightGrabbingScp.objectInHand.name;
        }


        if (leftGrabbingScp.objectInHand == null)
        {
            objectHandLeftTxt.text = "null";

        }
        else
        {
            objectHandLeftTxt.text = leftGrabbingScp.objectInHand.name;
        }
    }

    public void Log(string str)
    {
        debugText.text = str+"\n" + debugText.text;
    }
}
