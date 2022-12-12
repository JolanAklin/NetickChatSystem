using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Transport;
using LiteNetLib.Utils;

[RequireComponent(typeof(ChatNetworkEventsListner))]
public class ChatMessager : MonoBehaviour
{
    public bool isInitialized {get; private set;}
    private NetworkSandbox _sandbox;
    private ChatNetworkEventsListner _listener;
    private NetDataWriter _writer = new NetDataWriter();

    public void Init (NetworkSandbox sandbox, ChatNetworkEventsListner listner)
    {
        _sandbox = sandbox;
        _listener = listner;
        ChatLiteNetTransport._onChatReceive += OnChatMessageReceivedHandler;

        isInitialized = true;
    }

    public void OnDestroy()
    {
        ChatLiteNetTransport._onChatReceive -= OnChatMessageReceivedHandler;
    }

    // called when receiving a chat message
    // will only be called on the client
    private void OnChatMessageReceivedHandler(object sender, ChatLiteNetTransport.OnChatReceiveEventArgs e)
    {
        NetDataReader reader = new NetDataReader(e.message);
        string message = reader.GetString();
        OnChatMessageReceived(message);
    }

    public void OnChatMessageReceived(string message)
    {
        Debug.LogError(message + " " + _sandbox.name);
    }

    public void SendChatMessageToAll(string message)
    {
        foreach (NetworkConnection client in _listener.Connections.Values)
        {
            SendChatMessage(message, client);
        }
    }
    private void SendChatMessage(string message, NetworkConnection client)
    {
        _writer.Reset();
        _writer.Put(message);
        ChatLiteNetTransport.LNLConnection connection = (ChatLiteNetTransport.LNLConnection)_listener.Connections[client.Id].TransportConnection;
        connection.ChatSend(_writer.Data, _writer.Data.Length);
    }

    public void SendToServer(string message)
    {
        if(_sandbox.IsServer)
        {
            Debug.LogWarning("Message not sent. You are calling this function from the server. It should only be called on the client.");
            return;
        }
        _writer.Reset();
        _writer.Put(message);
        ChatLiteNetTransport.LNLConnection connection = (ChatLiteNetTransport.LNLConnection)_listener.ServerConnection.TransportConnection;
        connection.ChatSend(_writer.Data, _writer.Data.Length);
    }
}
