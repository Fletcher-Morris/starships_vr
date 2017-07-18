using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetVrManager : MonoBehaviour {

    public GameObject myVrRig;

    public GameObject myNetRig;

    public GameObject myHead;
    public GameObject myRightHand;
    public GameObject myLeftHand;

    private void Update()
    {
        if (!myVrRig)
            myVrRig = GameObject.Find("[CameraRig]").gameObject;

        if (!myHead)
            myHead = myVrRig.transform.Find("Camera (eye)").gameObject;

        if (!myRightHand)
            myRightHand = myVrRig.transform.Find("Controller (right)").gameObject;

        if (!myLeftHand)
            myLeftHand = myVrRig.transform.Find("Controller (left)").gameObject;
    }
}
