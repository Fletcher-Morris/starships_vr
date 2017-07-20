using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrUiTouch : MonoBehaviour {


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<VrUiButton>())
        {
            other.GetComponent<VrUiButton>().Touch(gameObject);
            other.GetComponent<VrUiButton>().isPressed = true;
        }

        if (other.GetComponent<VrUiToggle>())
        {
            other.GetComponent<VrUiToggle>().Touch(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<VrUiButton>())
        {
            other.GetComponent<VrUiButton>().Release(gameObject);
            other.GetComponent<VrUiButton>().isPressed = false;
        }
    }
}