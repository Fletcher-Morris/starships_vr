using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetVrRig : NetworkBehaviour {

    public GameObject headObj;
    public GameObject rightHandObj;
    public GameObject leftHandObj;

    public NetVrManager myMan;
    
    public bool showHands = false;

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
            headObj.transform.position = myMan.myHead.transform.position;
            rightHandObj.transform.position = myMan.myRightHand.transform.position;
            leftHandObj.transform.position = myMan.myLeftHand.transform.position;

            headObj.transform.rotation = myMan.myHead.transform.rotation;
            rightHandObj.transform.rotation = myMan.myRightHand.transform.rotation;
            leftHandObj.transform.rotation = myMan.myLeftHand.transform.rotation;
        }
        else
        {
            GameObject.Destroy(gameObject);
        }

        if (!showHands)
        {
            headObj.layer = LayerMask.NameToLayer("Invisible");
            rightHandObj.layer = LayerMask.NameToLayer("Invisible");
            leftHandObj.layer = LayerMask.NameToLayer("Invisible");
        }
        else
        {
            headObj.layer = LayerMask.NameToLayer("Default");
            rightHandObj.layer = LayerMask.NameToLayer("Default");
            leftHandObj.layer = LayerMask.NameToLayer("Default");
        }
    }
}
