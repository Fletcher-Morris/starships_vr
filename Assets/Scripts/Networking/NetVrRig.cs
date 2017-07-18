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
            headObj.transform.position = myMan.myHead.transform.position * 2;
            rightHandObj.transform.position = myMan.myRightHand.transform.position * 2;
            leftHandObj.transform.position = myMan.myLeftHand.transform.position * 2;

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
            headObj.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
            rightHandObj.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
            leftHandObj.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            headObj.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
            rightHandObj.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
            leftHandObj.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
