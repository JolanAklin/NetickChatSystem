using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Transport;
using LiteNetLib.Utils;
using LiteNetLib;

[RequireComponent(typeof(ConnectionsManager))]
public class ChatNetworkEventsListner : NetworkEventsListner
{
    private ConnectionsManager _connectionsManager;
    public override void OnStartup(NetworkSandbox sandbox)
    {
        try
        {
            _connectionsManager = GetComponent<ConnectionsManager>();
            ChatMessenger chat = GetComponent<ChatMessenger>();
            chat.Init(Sandbox, this);
        }
        catch (System.Exception)
        {
            Debug.LogError("You need to use the ChatLiteNetTransport with the ChatNetworkEventsListener");
        }
    }

    public override void OnShutdown(NetworkSandbox sandbox)
    {
        _connectionsManager.Clear();
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        _connectionsManager.SetServerConnection(server);
    }

    // add client to dictionary

    // call base.OnClientConnected() if this method is overrinden or manually call AddNetConnection
    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        AddNetConnection(client);
    }
    public void AddNetConnection(NetworkConnection connection)
    {
        _connectionsManager.AddId2NetCon(connection.Id, connection);
        _connectionsManager.AddNetickConnection2NetCon(connection.TransportConnection, connection);
    }

    // remove client from dictionary

    // call base.OnClientDisconnected() if this method is overrinden or manually call RemoveNetConnection
    public override void OnClientDisconnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        RemoveNetConnection(client);
    }

    public void RemoveNetConnection(NetworkConnection connection)
    {
        if(!_connectionsManager.RemoveNetConByNetickConnection(connection.TransportConnection))
            Debug.LogWarning("Trying to remove a NetworkConnection that wasn't registred.");
        if(!_connectionsManager.RemoveNetConById(connection.Id))
            Debug.LogWarning("Trying to remove a NetworkConnection that wasn't registred. Connection ID : " + connection.Id);
    }
}
