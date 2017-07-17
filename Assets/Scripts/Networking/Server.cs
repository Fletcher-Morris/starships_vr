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
    public int playerPing;
    public Vector3 playerPosition;
}

public class Server : MonoBehaviour
{
    private static Server instance = null;

    private const int MAX_CONNECTIONS = 8;

    private int port = 7777;

    private int hostId;
    private int webHostId;

    private int reliableChannel;
    private int unreliableChannel;

    private bool isStarted;
    private byte error;

    public string externalIp;

    public List<ServerClient> clients = new List<ServerClient>();

    public float pingFrequency = 2f;
    private float pingTimer = 2f;

    public float movementUpdateRate = 0.1f;
    private float lastMovementUpdate;

    public bool shallowDebug = false;
    public bool deepDebug = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("Canvas"));

        StartCoroutine(GetExternalIp());
    }

    private IEnumerator GetExternalIp()
    {
        WWW www = new WWW("http://checkip.dyndns.org");

        yield return www;
        string webText =  www.text;
        string[] webTextSplit;
        webTextSplit = webText.Split(' ');
        webText = webTextSplit[webTextSplit.Length - 1];
        webTextSplit = webText.Split('<');
        webText = webTextSplit[0];

        externalIp = webText;
    }

    public void StartHost()
    {
        StartCoroutine(GetExternalIp());

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTIONS);

        hostId = NetworkTransport.AddHost(topo, port, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, port, null);

        ServerLog("SERVER STARTED");
        ServerLog("External IP is " + externalIp);
        isStarted = true;
    }

    public void StopHost()
    {
        NetworkTransport.Shutdown();
        isStarted = false;
        clients.Clear();
    }

    public void ToggleDebug(Toggle toggle)
    {
        shallowDebug = toggle.isOn;
    }

    public void ToggleDeepDebug(Toggle toggle)
    {
        deepDebug = toggle.isOn;
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
                ServerLog("Player " + connectionId + " has connected");
                OnConnect(connectionId);
                break;
            case NetworkEventType.DataEvent:       //3
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                if (shallowDebug)
                {
                    ServerLog("Receiving from " + connectionId + " : " + msg);
                }
                string[] splitData = msg.Split('|');
                switch (splitData[0])
                {
                    case "NAMEIS":
                        OnNameIs(connectionId, splitData[1]);
                        break;

                    case "ECHO":
                        OnEcho(connectionId, DateTime.Parse(splitData[1]), DateTime.Now);
                        break;

                    case "MOVEMENTINPUT":
                        OnMovementInput(connectionId, splitData);
                        break;

                    default:
                        ServerLog("Invalid Message : " + msg);
                        Send("INVALIDMESSAGE", reliableChannel, connectionId);
                        break;
                }
                break;
            case NetworkEventType.DisconnectEvent: //4
                OnDisconnection(connectionId);
                break;
        }

        pingTimer -= Time.deltaTime;
        if(pingTimer <= 0)
        {
            Send("PING|" + DateTime.Now.ToString(), reliableChannel, clients);
            pingTimer = pingFrequency;
        }

        //  Ask for player positions
        if(Time.time - lastMovementUpdate > movementUpdateRate)
        {
            lastMovementUpdate = Time.time;

            Send("ASKINPUT|", unreliableChannel, clients);
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

    private void OnDisconnection(int conId)
    {
        ServerLog(clients.Find(x => x.connectionId == conId).playerName + " (player " + conId + ") has disconnected");
        //  Remove the player from client list
        clients.Remove(clients.Find(x => x.connectionId == conId));
        //  Tell everyone that someone disconnected
        Send("DC|" + conId, reliableChannel, clients);
    }

     private void ServerLog(string input)
    {
        if(input.Split('|')[0].Contains("PING") || input.Split('|')[0].Contains("ECHO") || input.Split('|')[0].Contains("ASKPOSITION") || input.Split('|')[0].Contains("MYPOSITION"))
        {
            if (deepDebug == false)
            {
                return;
            }
        }

        GameObject.Find("ServerConsole").transform.GetChild(0).gameObject.GetComponent<Text>().text += ("\n" + input);
        Debug.Log(input);
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

    private void OnMovementInput(int conId, string[] data)
    {
        List<ServerClient> sendToClients = new List<ServerClient>();

        foreach(ServerClient client in clients)
        {
            if(client.connectionId != conId)
            {
                sendToClients.Add(client);
            }
        }
        string msg = "MOVEMENTOUTPUT|" + conId.ToString() + '|' + data[1] + '|' + data[2];

        Send(msg, unreliableChannel, sendToClients);
    }

    private void Send(string message, int channelId, int conId)
    {
        List<ServerClient> c = new List<ServerClient>();
        c.Add(clients.Find(x => x.connectionId == conId));
        Send(message, channelId, c);
    }
    private void Send(string message, int channelId, List<ServerClient> c)
    {
        if (shallowDebug)
        {
            ServerLog("Sending : " + message);
        }
        byte[] msg = Encoding.Unicode.GetBytes(message);
        foreach (ServerClient client in c)
        {
            NetworkTransport.Send(hostId, client.connectionId, channelId, msg, message.Length * sizeof(char), out error);
        }
    }
}