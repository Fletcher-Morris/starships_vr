using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class VrUiToggle : MonoBehaviour
{

    private BoxCollider boxCollider;
    private RectTransform rectTransform;

    public bool interactable = true;

    public bool isOn;

    public UnityEvent OnToggle;

    public void Touch(GameObject toucher)
    {
        if (interactable)
        {
            toucher.transform.parent.gameObject.GetComponent<VrHandController>().Vibrate(0.5f, 0.1f);

            Toggle();
        }
    }

    public void Toggle()
    {
        isOn = !isOn;
        OnToggle.Invoke();

        if (GetComponent<Toggle>())
        {
            GetComponent<Toggle>().isOn = isOn;
        }
    }

    private void Update()
    {
        if (GetComponent<Toggle>())
        {
            GetComponent<Toggle>().isOn = isOn;
        }

    }
}
