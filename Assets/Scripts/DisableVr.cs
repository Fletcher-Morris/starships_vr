using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class DisableVr : MonoBehaviour
{
    public bool disableOnStart = false;
    public bool enableVr = true;
    private bool vrIsEnabled = true;

    public GameObject nonVrCam;
    public GameObject cameraRig;
    public NetVrManager netMan;

    private void Start()
    {
        if (disableOnStart)
            enableVr = false;
    }

    private void Update()
    {
        if(enableVr != vrIsEnabled)
        {
            if (enableVr == true)
            {
                VrEnable();
            }
            else
            {
                VrDisable();
            }
        }
    }

    public void VrEnable()
    {
        nonVrCam.SetActive(false);
        cameraRig.SetActive(true);
        netMan.enabled = true;
        VRSettings.enabled = true;
        vrIsEnabled = true;
    }

    public void VrDisable()
    {
        nonVrCam.SetActive(true);
        cameraRig.SetActive(false);
        netMan.enabled = false;
        VRSettings.enabled = false;
        vrIsEnabled = false;
    }
}