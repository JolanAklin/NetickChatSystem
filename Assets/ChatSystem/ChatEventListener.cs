using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick.Unity;
using Netick;
using ChatSystem;

public class ChatEventListener : NetworkEvents
{
    private MessageSender _sender;

    public override void OnStartup(NetworkSandbox sandbox)
    {
        _sender = Sandbox.GetComponent<MessageSender>();
    }

    public Dictionary<int, IChatTransportConnection> ClientConnection = new Dictionary<int, IChatTransportConnection>();
    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        ClientConnection.Add(client.Id, (IChatTransportConnection)client.TransportConnection);
        _sender.SendChatMessage((IChatTransportConnection)client.TransportConnection, "a new client has connected");
    }
}
