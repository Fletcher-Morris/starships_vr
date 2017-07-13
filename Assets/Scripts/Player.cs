using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class Player
{
    public int connectionId;
    public string playerName;
    public string playerSkin;

    public Player()
    {
        connectionId = 0;
        playerName = "New Player";
        playerSkin = "standard";
    }

    public Player(int id)
    {
        connectionId = id;
        playerName = "New Player";
        playerSkin = "standard";
    }

    public Player(int id, string name)
    {
        connectionId = id;
        playerName = name;
        playerSkin = "standard";
    }

    public Player(int id, string name, string skin)
    {
        connectionId = id;
        playerName = name;
        playerSkin = skin;
    }
}