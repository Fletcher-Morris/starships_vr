using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkGameManager : NetworkManager {

    public List<Player> players = new List<Player>();

    public void SetUpHost()
    {
        networkPort = 7777;

        StartHost();
    }

    public void SetUpClient()
    {
        networkPort = 7777;
        networkAddress = "localhost";

        StartClient();
    }

    public void QuitServer()
    {
        StopClient();
        StopHost();
    }
}