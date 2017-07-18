using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrHandController : MonoBehaviour {

    public GameObject normalHand;
    public GameObject pointingHand;
    public GameObject fistHand;
    public GameObject thumbsHand;
    public GameObject okHand;

    public GameObject myToucher;

    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;

    public float triggerAxis;

    public bool gripDown;
    public bool thumbDown;
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
        trackedObject = GetComponent<SteamVR_TrackedObject>();

        normalHand = transform.Find("Normal Hand").gameObject;
        pointingHand = transform.Find("Pointing Hand").gameObject;
        fistHand = transform.Find("Fist Hand").gameObject;
        thumbsHand = transform.Find("Thumbs Hand").gameObject;
        okHand = transform.Find("OK Hand").gameObject;
        myToucher = transform.Find("UiToucher").gameObject;


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
        device = SteamVR_Controller.Input((int)trackedObject.index);

        triggerAxis = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            StartCoroutine(VibrateController(.1f, 10));
        }

        if(normalHand != null && myToucher != null)
        {
            HandSwapper();
        }
    }

    private void HandSwapper()
    {
        fingerDown = device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
        thumbDown = device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad);
        gripDown = device.GetPress(SteamVR_Controller.ButtonMask.Grip);

        if(fingerDown && thumbDown && gripDown)
        {
            normalHand.SetActive(false);
            thumbsHand.SetActive(false);
            pointingHand.SetActive(false);
            fistHand.SetActive(true);
            okHand.SetActive(false);

            myToucher.SetActive(false);
        }
        else if (thumbDown && fingerDown)
        {
            normalHand.SetActive(false);
            thumbsHand.SetActive(false);
            pointingHand.SetActive(false);
            fistHand.SetActive(false);
            okHand.SetActive(true);

            myToucher.SetActive(false);
        }
        else if (fingerDown && gripDown)
        {
            normalHand.SetActive(false);
            thumbsHand.SetActive(true);
            pointingHand.SetActive(false);
            fistHand.SetActive(false);
            okHand.SetActive(false);

            myToucher.SetActive(false);
        }
        else if (thumbDown && gripDown)
        {
            normalHand.SetActive(false);
            thumbsHand.SetActive(false);
            pointingHand.SetActive(true);
            fistHand.SetActive(false);
            okHand.SetActive(false);

            myToucher.SetActive(true);
            myToucher.transform.localPosition = new Vector3(0.027f, -0.055f, -0.012f);

            FingerPose(triggerAxis);
        }
        else
        {
            normalHand.SetActive(true);
            thumbsHand.SetActive(false);
            pointingHand.SetActive(false);
            fistHand.SetActive(false);
            okHand.SetActive(false);

            myToucher.SetActive(true);
            myToucher.transform.localPosition = new Vector3(-0.023f, -0.03f, -0.042f);
        }
    }

    public void FingerPose(float pose)
    {
        joint1.transform.localRotation = Quaternion.Lerp(joint1Max, joint1Min, pose);
        joint2.transform.localRotation = Quaternion.Lerp(joint2Max, joint2Min, pose);
        joint3.transform.localRotation = Quaternion.Lerp(joint3Max, joint3Min, pose);
    }

    public void Vibrate(float length, float strength)
    {
        StartCoroutine(VibrateController(length, strength));
    }

    private IEnumerator VibrateController(float length, float strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            device.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
            yield return null;
        }
    }
}
