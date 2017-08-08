using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class RosmarusExtensions
{
    public static string RandomString(int minLength, int maxLength, bool allowNumbers)
    {
        string output = "";

        const string letterGlyphs = "abcdefghijklmnopqrstuvwxyz";
        const string allGlyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

        int stringLength = Random.Range(minLength, maxLength);

        for(int i = 0; i < stringLength; i++)
        {
            if (allowNumbers)
            {
                output += allGlyphs[Random.Range(0, allGlyphs.Length)];
            }
            else
            {
                output += letterGlyphs[Random.Range(0, letterGlyphs.Length)];
            }
        }

        return output;
    }

    public static List<GameObject> GetAllChildren(GameObject parent)
    {
        List<GameObject> output = new List<GameObject>();

        foreach (Transform child in parent.GetComponentInChildren<Transform>())
        {
            output.Add(child.gameObject);
            foreach(GameObject grandChild in GetAllChildren(child.gameObject))
            {
                output.Add(grandChild);
            }
        }

        return output;
    }
}