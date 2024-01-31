using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick.Unity;
using Netick;
using ChatSystem;
using System.Linq;

public class ChatEventListener : NetworkEvents
{
    private MessageSender _sender;
    private ConnectionManager _connectionManager;

    public override void OnStartup(NetworkSandbox sandbox)
    {
        _sender = Sandbox.GetComponent<MessageSender>();
        _connectionManager = Sandbox.GetComponent<ConnectionManager>();
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        _connectionManager.ClientConnections.Add(client.Id, (IChatTransportConnection)client.TransportConnection);
        _sender.SendChatMessage(_connectionManager.ClientConnections.Values.ToArray(), "a new client has connected " + client.Id);
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        _connectionManager.ServerConnection = (IChatTransportConnection)server.TransportConnection;
        _sender.SendChatMessage(_connectionManager.ServerConnection, "je suis le nouveau client " + sandbox.LocalPlayer.PlayerId);
    }
}
