using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToDynamic : MonoBehaviour
{

    public bool onStart = true;

    private void Start()
    {
        if (onStart)
        {
            MakeDynamic(gameObject);
        }
    }

    public void MakeDynamic(GameObject obj)
    {
        foreach (GameObject child in RosmarusExtensions.GetAllChildren(gameObject))
        {
            child.isStatic = false;
        }
        gameObject.isStatic = false;
    }
}