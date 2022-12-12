using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Transport;
using LiteNetLib.Utils;
using LiteNetLib;

public class ChatNetworkEventsListner : NetworkEventsListner
{
    // network connection id and it's related network connection
    private Dictionary<int, NetworkConnection> _connections;
    public Dictionary<int, NetworkConnection> Connections { get => _connections; }

    private NetworkConnection _serverConnection;
    public NetworkConnection ServerConnection { get => _serverConnection; }

    private ChatLiteNetTransport _transport;


    protected void Awake() {
        _connections = new Dictionary<int, NetworkConnection>();
    }

    public override void OnStartup(NetworkSandbox sandbox)
    {
        try
        {
            _transport = (ChatLiteNetTransport)Sandbox.Config.GetTransport();
            ChatMessager chat = GetComponent<ChatMessager>();
            chat.Init(Sandbox, this);
        }
        catch (System.Exception)
        {
            Debug.LogError("You need to use the ChatLiteNetTransport with the ChatNetworkEventsListener");
        }
    }

    public override void OnShutdown(NetworkSandbox sandbox)
    {
        _serverConnection = null;
        _connections.Clear();
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        _serverConnection = server;
    }

    // add client to dictionary

    // call base.OnClientConnected() if this method is overrinden or manually call AddNetConnection
    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        AddNetConnection(client);
    }
    public void AddNetConnection(NetworkConnection connection)
    {
        _connections.Add(connection.Id, connection);
    }

    // remove client from dictionary

    // call base.OnClientDisconnected() if this method is overrinden or manually call RemoveNetConnection
    public override void OnClientDisconnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        RemoveNetConnection(client);
    }
    public void RemoveNetConnection(NetworkConnection connection)
    {
        RemoveNetConnection(connection.Id);
    }
    public void RemoveNetConnection(int id)
    {
        if(_connections.ContainsKey(id))
            _connections.Remove(id);
        else
            Debug.LogWarning("Trying to remove a NetworkConnection that wasn't registred. Connection ID : " + id);
    }

    public void SendToServer()
    {
        
    }
}
