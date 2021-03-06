﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class VrUiButton : MonoBehaviour
{

    private BoxCollider boxCollider;
    private RectTransform rectTransform;

    public bool interactable = true;

    private Color normalColour;
    public Color pressedColour;

    public bool isPressed;

    public UnityEvent OnTouch;
    public UnityEvent OnRelease;

    private void Start()
    {
        normalColour = GetComponent<Image>().color;
    }

    public void Touch()
    {
        if (interactable && !isPressed)
        {
            isPressed = true;
            GetComponent<Image>().color = pressedColour;
            OnTouch.Invoke(); 
        }
    }
    public void Touch(GameObject toucher)
    {
        if (interactable && !isPressed)
        {
            isPressed = true;
            GetComponent<Image>().color = pressedColour;
            OnTouch.Invoke();
            toucher.transform.parent.gameObject.GetComponent<VrHandController>().Vibrate(0.5f, 0.1f);
        }
    }

    public void Release()
    {
        if (interactable && isPressed)
        {
            isPressed = false;
            GetComponent<Image>().color = normalColour;
            OnRelease.Invoke();
        }
    }
    public void Release(GameObject toucher)
    {
        if (interactable && isPressed)
        {
            isPressed = false;
            GetComponent<Image>().color = normalColour;
            OnRelease.Invoke();
            toucher.transform.parent.gameObject.GetComponent<VrHandController>().Vibrate(0.5f, 0.1f);
        }
    }

    private void Update()
    {
        if (isPressed)
        {
            GetComponent<Image>().color = pressedColour;
        }
        else
        {
            GetComponent<Image>().color = normalColour;
        }

    }
}
