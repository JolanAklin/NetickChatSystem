using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Unity;
using ChatSystem;

public class NetEventsListener : NetworkEventsListener
{
    [SerializeField] private ChatSystemManager _chatSystemManager;
    public override void OnStartup(NetworkSandbox sandbox)
    {
        _chatSystemManager.SandboxId = sandbox.GetInstanceID();
        _chatSystemManager.IsClient = sandbox.IsClient;
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        Debug.Log("sending chat message");
        SendMessageManager.SendMessage("this is a test", (ChatTransportConnection)client.TransportConnection);
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        _chatSystemManager.ServerConnection = (ChatTransportConnection)server.TransportConnection;
    }
}
