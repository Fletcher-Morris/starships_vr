using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public GameObject lobbyPlayerPrefab;
    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    private InputField addressInputField;

    private const int MAX_CONNECTIONS = 8;

    private int port = 7777;
    private string connectToIp = "127.0.0.1";

    private int hostId;
    private int webHostId;

    private int myclientId;
    private int connectionId;

    private int reliableChannel;
    private int unreliableChannel;

    private float connectionTime;
    private bool isConnected = false;
    private bool isStarted = false;
    private byte error;

    public string externalIp;

    public bool deepDebug = false;

    private string playerName;
    public int myPing;

    private void Start()
    {
        addressInputField = GameObject.Find("AddressInputField").GetComponent<InputField>();
        playerName = RosmarusExtensions.RandomString(5, 5, true);
        GameObject.Find("NameTextBox").GetComponent<Text>().text = playerName;
        StartCoroutine(GetExternalIp());
    }

    private IEnumerator GetExternalIp()
    {
        WWW www = new WWW("http://checkip.dyndns.org");

        yield return www;
        string webText = www.text;
        string[] webTextSplit;
        webTextSplit = webText.Split(' ');
        webText = webTextSplit[webTextSplit.Length - 1];
        webTextSplit = webText.Split('<');
        webText = webTextSplit[0];

        externalIp = webText;
    }

    public void Connect()
    {
        StartCoroutine(GetExternalIp());
        if(addressInputField.text != "")
        {
            connectToIp = addressInputField.text;
        }

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTIONS);

        hostId = NetworkTransport.AddHost(topo, 0);
        connectionId = NetworkTransport.Connect(hostId, connectToIp, port, 0, out error);

        Debug.Log("CONNECTED TO SERVER");
        connectionTime = Time.time;
        isConnected = true;
    }

    public void Disconnect()
    {
        NetworkTransport.Shutdown();
        isConnected = false;
        isStarted = false;
        players.Clear();
    }

    private void Update()
    {
        string conPlrs = "";

        foreach(Player conPlr in players.Values)
        {
            conPlrs += conPlr.playerName + "\n";
        }

        GameObject.Find("ConnectedPlayersTextBox").GetComponent<Text>().text = conPlrs;

        if (!isConnected)
            return;

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:       //3
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                if (deepDebug)
                {
                    Debug.Log("Receiving : " + msg);
                }
                string[] splitData = msg.Split('|');
                switch (splitData[0])
                {
                    case "ASKNAME":
                        OnAskName(splitData);
                        break;

                    case "CON":
                        SpawnLobbyPlayer(splitData[1], int.Parse(splitData[2]));
                        Debug.Log(splitData[1].ToString() + " and " + int.Parse(splitData[2]).ToString());
                        break;

                    case "DC":
                        PlayerDisconnected(int.Parse(splitData[1]));
                        break;

                    case "PING":
                        OnPing(splitData);
                        break;

                    case "PINGRESULT":
                        myPing = int.Parse(splitData[1]);
                        break;

                    default:
                        Debug.Log("Invalid Message : " + msg);
                        break;
                }
                break;
        }
    }

    private void Send(string message, int channelId)
    {
        if (deepDebug)
        {
            Debug.Log("Sending : " + message); 
        }
        byte[] msg = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);
    }

    private void OnAskName(string[] data)
    {
        //  Set this client's ID
        myclientId = int.Parse(data[1]);

        //  Send our name to the server
        Send("NAMEIS|" + playerName, reliableChannel);

        //  Create the other players
        for (int i = 2; i < data.Length - 1; i++)
        {
            string[] d = data[i].Split('%');

            SpawnLobbyPlayer(d[0], int.Parse(d[1]));
        }
    }

    private void OnPing(string[] data)
    {
        //  Send back the ping data
        Send("ECHO|" + data[1], reliableChannel);
    }

    private void SpawnLobbyPlayer(string playerName, int conId)
    {
        Debug.Log("SPAWN PLAYER : " + playerName);

        Player p = new Player();
        p.playerName = playerName;
        p.connectionId = conId;

        players.Add(conId, p);
    }

    private void PlayerDisconnected(int conId)
    {
        Debug.Log(players[conId].playerName + " (player " + conId + ") has disconnected");
        players.Remove(conId);
    }
}