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
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        Debug.Log("sending chat message");
        byte[] text = System.Text.Encoding.UTF8.GetBytes("this is a test");
        ((LNLTransportProviderWchat.LNLConnection)client.TransportConnection).ChatSend(text, text.Length);
    }
}
