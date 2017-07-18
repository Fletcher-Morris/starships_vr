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

    public bool gripDown;
    public bool thumbDown;
    public bool fingerDown;

    private void Start()
    {
        trackedObject = GetComponent<SteamVR_TrackedObject>();

        normalHand = transform.Find("Normal Hand").gameObject;
        pointingHand = transform.Find("Pointing Hand").gameObject;
        fistHand = transform.Find("Fist Hand").gameObject;
        thumbsHand = transform.Find("Thumbs Hand").gameObject;
        okHand = transform.Find("OK Hand").gameObject;
        myToucher = transform.Find("UiToucher").gameObject;
    }

    private void Update()
    {
        device = SteamVR_Controller.Input((int)trackedObject.index);

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

    public IEnumerator VibrateController(float length, float strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            device.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
            yield return null;
        }
    }
}
