using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class ServerClient
{
    public int connectionId;
    public string playerName;
    public string playerClass;

    public int playerPing;
}

public class Server : MonoBehaviour
{
    private const int MAX_CONNECTIONS = 8;

    private int port = 7777;

    private int hostId;
    private int webHostId;

    private int reliableChannel;
    private int unreliableChannel;

    private bool isStarted;
    private byte error;

    public List<ServerClient> clients = new List<ServerClient>();

    public float pingFrequency = 2f;
    private float pingTimer = 2f;

    public bool detailedDebug = false;

    public void StartHost()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTIONS);

        hostId = NetworkTransport.AddHost(topo, port, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, port, null);

        Debug.Log("SERVER STARTED");
        isStarted = true;
    }

    private void Update()
    {
        if (!isStarted)
            return;

        string conPlrs = "";
        foreach (ServerClient conPlr in clients)
        {
            conPlrs += conPlr.playerName + "\n";
        }
        GameObject.Find("ConnectedPlayersTextBox").GetComponent<Text>().text = conPlrs;

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
            case NetworkEventType.ConnectEvent:    //2
                Debug.Log("Player " + connectionId + " has connected");
                OnConnect(connectionId);
                break;
            case NetworkEventType.DataEvent:       //3
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                if (detailedDebug)
                {
                    Debug.Log("Receiving from " + connectionId + " : " + msg);
                }
                string[] splitData = msg.Split('|');
                switch (splitData[0])
                {
                    case "NAMEIS":
                        OnNameIs(connectionId, splitData[1]);
                        break;

                    case "DC":
                        break;

                    case "ECHO":
                        OnEcho(connectionId, DateTime.Parse(splitData[1]), DateTime.Now);
                        break;

                    default:
                        Debug.Log("Invalid Message : " + msg);
                        break;
                }
                break;
            case NetworkEventType.DisconnectEvent: //4
                Debug.Log("Player " + connectionId + " has disconnected");
                break;
        }

        pingTimer -= Time.deltaTime;
        if(pingTimer <= 0)
        {
            Send("PING|" + DateTime.Now.ToString(), reliableChannel, clients);
            pingTimer = pingFrequency;
        }
    }

    private void OnConnect(int conId)
    {
        ServerClient c = new ServerClient();
        c.connectionId = conId;
        c.playerName = "TEMP";
        clients.Add(c);

        //  When a player joins thee server, tell them their ID
        //  Request their name, and send the name of other players
        string msg = "ASKNAME|" + conId + "|";
        foreach (ServerClient client in clients)
            msg += client.playerName + '%' + client.connectionId + '|';

        msg = msg.Trim('|');
        Send(msg, reliableChannel, conId);
    }

    private void Send(string message, int channelId, int conId)
    {
        List<ServerClient> c = new List<ServerClient>();
        c.Add(clients.Find(x => x.connectionId == conId));
        Send(message, channelId, c);
    }
    private void Send(string message, int channelId, List<ServerClient> c)
    {
        if (detailedDebug)
        {
            Debug.Log("Sending : " + message);
        }
        byte[] msg = Encoding.Unicode.GetBytes(message);
        foreach (ServerClient client in c)
        {
            NetworkTransport.Send(hostId, client.connectionId, channelId, msg, message.Length * sizeof(char), out error);
        }
    }

    private void OnNameIs(int conId, string playerName)
    {
        //  Link the name to th connection ID
        clients.Find(x => x.connectionId == conId).playerName = playerName;
        //  Tell clients that a new player connected
        Send("CON|" + playerName + '|' + conId, reliableChannel, clients);
    }

    private void OnEcho(int conId, DateTime pingTime, DateTime echoTime)
    {
        int avTripTime = Mathf.RoundToInt(Convert.ToSingle((echoTime - pingTime).TotalSeconds));
        clients.Find(x => x.connectionId == conId).playerPing = avTripTime;

        Send("PINGRESULT|" + avTripTime.ToString(), reliableChannel, conId);
    }
}