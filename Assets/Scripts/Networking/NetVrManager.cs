using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetVrManager : MonoBehaviour {

    public GameObject myVrRig;

    public GameObject myNetRig;

    public GameObject myHead;
    public GameObject myRightHand;
    public GameObject myLeftHand;

    public Toggle myStandingToggle;
    public bool isStanding;

    private void Start()
    {
        
    }

    private void Update()
    {
        isStanding = myStandingToggle.isOn;

        if (!myVrRig)
            myVrRig = GameObject.Find("[CameraRig]").gameObject;

        if (!myHead)
            myHead = myVrRig.transform.Find("Camera (eye)").gameObject;

        if (!myRightHand)
            myRightHand = myVrRig.transform.Find("Controller (right)").gameObject;

        if (!myLeftHand)
            myLeftHand = myVrRig.transform.Find("Controller (left)").gameObject;

        if (myNetRig)
        {
            myNetRig.GetComponent<NetVrRig>().isStanding = isStanding;
        }
    }
}
