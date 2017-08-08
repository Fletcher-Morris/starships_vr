using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetVrRig : NetworkBehaviour {

    public GameObject headObj;
    public GameObject rightHandObj;
    public GameObject leftHandObj;

    public NetVrHandController rightHandController;
    public NetVrHandController leftHandController;

    public NetVrManager myMan;
    
    public bool showHands = false;
    private bool showingHands = true;

    [SyncVar]
    public bool isStanding;

    private void Start()
    {
        if (!isLocalPlayer)
            return;

        myMan = GameObject.Find("NM").GetComponent<NetVrManager>();
        myMan.myNetRig = gameObject;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (myMan)
        {
            if (myMan.isActiveAndEnabled)
            {
                headObj.transform.position = myMan.myHead.transform.position;
                headObj.transform.rotation = myMan.myHead.transform.rotation;

                rightHandObj.transform.position = myMan.myRightHand.transform.position;
                rightHandObj.transform.rotation = myMan.myRightHand.transform.rotation;
                rightHandController.triggerAxis = myMan.myRightHand.GetComponent<VrHandController>().triggerAxis;
                rightHandController.gripDown = myMan.myRightHand.GetComponent<VrHandController>().gripDown;
                rightHandController.thumbDown = myMan.myRightHand.GetComponent<VrHandController>().thumbDown;
                rightHandController.fingerDown = myMan.myRightHand.GetComponent<VrHandController>().fingerDown;

                leftHandObj.transform.rotation = myMan.myLeftHand.transform.rotation;
                leftHandObj.transform.position = myMan.myLeftHand.transform.position;
                leftHandController.triggerAxis = myMan.myLeftHand.GetComponent<VrHandController>().triggerAxis;
                leftHandController.gripDown = myMan.myLeftHand.GetComponent<VrHandController>().gripDown;
                leftHandController.thumbDown = myMan.myLeftHand.GetComponent<VrHandController>().thumbDown;
                leftHandController.fingerDown = myMan.myLeftHand.GetComponent<VrHandController>().fingerDown;
            }
        }
        else
        {
            GameObject.Destroy(gameObject);
        }

        if(showHands != showingHands)
        {
            SetObjectsVisible(showHands);
        }
    }

    public void SetObjectsVisible(bool value)
    {
        if (!value)
        {
            foreach (GameObject obj in RosmarusExtensions.GetAllChildren(headObj))
            {
                obj.layer = LayerMask.NameToLayer("Invisible");
            }
            foreach (GameObject obj in RosmarusExtensions.GetAllChildren(rightHandObj))
            {
                obj.layer = LayerMask.NameToLayer("Invisible");
            }
            foreach (GameObject obj in RosmarusExtensions.GetAllChildren(leftHandObj))
            {
                obj.layer = LayerMask.NameToLayer("Invisible");
            }
            headObj.layer = LayerMask.NameToLayer("Invisible");
            rightHandObj.layer = LayerMask.NameToLayer("Invisible");
            leftHandObj.layer = LayerMask.NameToLayer("Invisible");
        }
        else
        {
            foreach (GameObject obj in RosmarusExtensions.GetAllChildren(rightHandObj))
            {
                obj.layer = LayerMask.NameToLayer("Default");
            }
            foreach (GameObject obj in RosmarusExtensions.GetAllChildren(leftHandObj))
            {
                obj.layer = LayerMask.NameToLayer("Default");
            }
            headObj.layer = LayerMask.NameToLayer("Default");
            rightHandObj.layer = LayerMask.NameToLayer("Default");
            leftHandObj.layer = LayerMask.NameToLayer("Default");
        }

        showingHands = showHands;
    }
}
