using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetVrHandController : NetworkBehaviour {

    public GameObject myHandGroup;
    public GameObject normalHand;
    public GameObject pointingHand;
    public GameObject fistHand;
    public GameObject thumbsHand;
    public GameObject okHand;

    [SyncVar]
    public float triggerAxis;
    [SyncVar]
    public bool gripDown;
    [SyncVar]
    public bool thumbDown;
    [SyncVar]
    public bool fingerDown;

    private GameObject joint1;
    private GameObject joint2;
    private GameObject joint3;

    private Quaternion joint1Min;
    private Quaternion joint2Min;
    private Quaternion joint3Min;

    private Quaternion joint1Max;
    private Quaternion joint2Max;
    private Quaternion joint3Max;

    private void Start()
    {

        normalHand = myHandGroup.transform.Find("Normal Hand").gameObject;
        pointingHand = myHandGroup.transform.Find("Pointing Hand").gameObject;
        fistHand = myHandGroup.transform.Find("Fist Hand").gameObject;
        thumbsHand = myHandGroup.transform.Find("Thumbs Hand").gameObject;
        okHand = myHandGroup.transform.Find("OK Hand").gameObject;

        joint1 = pointingHand.transform.GetChild(0).GetChild(4).gameObject;
        joint2 = joint1.transform.GetChild(0).gameObject;
        joint3 = joint2.transform.GetChild(0).gameObject;

        joint1Min = fistHand.transform.GetChild(0).GetChild(4).localRotation;
        joint2Min = fistHand.transform.GetChild(0).GetChild(4).GetChild(0).localRotation;
        joint3Min = fistHand.transform.GetChild(0).GetChild(4).GetChild(0).GetChild(0).localRotation;

        joint1Max = pointingHand.transform.GetChild(0).GetChild(4).localRotation;
        joint2Max = pointingHand.transform.GetChild(0).GetChild(4).GetChild(0).localRotation;
        joint3Max = pointingHand.transform.GetChild(0).GetChild(4).GetChild(0).GetChild(0).localRotation;
    }

    private void Update()
    {
        if (normalHand != null)
        {
            HandSwapper();
        }
    }

    private void HandSwapper()
    {
        if (fingerDown && thumbDown && gripDown)
        {
            normalHand.SetActive(false);
            thumbsHand.SetActive(false);
            pointingHand.SetActive(false);
            fistHand.SetActive(true);
            okHand.SetActive(false);
        }
        else if (thumbDown && fingerDown)
        {
            normalHand.SetActive(false);
            thumbsHand.SetActive(false);
            pointingHand.SetActive(false);
            fistHand.SetActive(false);
            okHand.SetActive(true);
        }
        else if (fingerDown && gripDown)
        {
            normalHand.SetActive(false);
            thumbsHand.SetActive(true);
            pointingHand.SetActive(false);
            fistHand.SetActive(false);
            okHand.SetActive(false);
        }
        else if (thumbDown && gripDown)
        {
            normalHand.SetActive(false);
            thumbsHand.SetActive(false);
            pointingHand.SetActive(true);
            fistHand.SetActive(false);
            okHand.SetActive(false);

            FingerPose(triggerAxis);
        }
        else
        {
            normalHand.SetActive(true);
            thumbsHand.SetActive(false);
            pointingHand.SetActive(false);
            fistHand.SetActive(false);
            okHand.SetActive(false);
        }
    }

    public void FingerPose(float pose)
    {
        joint1.transform.localRotation = Quaternion.Lerp(joint1Max, joint1Min, pose);
        joint2.transform.localRotation = Quaternion.Lerp(joint2Max, joint2Min, pose);
        joint3.transform.localRotation = Quaternion.Lerp(joint3Max, joint3Min, pose);
    }
}
