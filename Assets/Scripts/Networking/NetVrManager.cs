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

        if (!myVrRig && GameObject.Find("[CameraRig]"))
            myVrRig = GameObject.Find("[CameraRig]").gameObject;

        if (!myHead && GameObject.Find("Camera (eye)"))
            myHead = myVrRig.transform.Find("Camera (eye)").gameObject;

        if (!myRightHand && GameObject.Find("Controller (right)"))
            myRightHand = myVrRig.transform.Find("Controller (right)").gameObject;

        if (!myLeftHand && GameObject.Find("Controller (left)"))
            myLeftHand = myVrRig.transform.Find("Controller (left)").gameObject;

        if (myNetRig)
        {
            myNetRig.GetComponent<NetVrRig>().isStanding = isStanding;
        }
    }
}
