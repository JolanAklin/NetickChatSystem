using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Transport;
using LiteNetLib.Utils;

[RequireComponent(typeof(ChatNetworkEventsListner))]
[RequireComponent(typeof(ConnectionsManager))]
public class ChatMessenger : MonoBehaviour
{
    public bool isInitialized {get; private set;}
    private NetworkSandbox _sandbox;
    private ChatNetworkEventsListner _listener;
    private NetDataWriter _writer = new NetDataWriter();
    private ConnectionsManager _connectionManager;

    public event System.EventHandler<OnClientReceiveChatMessageEventArgs> OnClientReceiveChatMessage;

    public class OnClientReceiveChatMessageEventArgs : System.EventArgs {
        public string message;

        public OnClientReceiveChatMessageEventArgs (string message)
        {
            this.message = message;
        }
    }

    public void Init (NetworkSandbox sandbox, ChatNetworkEventsListner listner)
    {
        _sandbox = sandbox;
        _listener = listner;
        _connectionManager = GetComponent<ConnectionsManager>();
        ChatLiteNetTransport._onChatReceive += OnChatMessageReceivedHandler;

        isInitialized = true;
    }

    public void OnDestroy()
    {
        ChatLiteNetTransport._onChatReceive -= OnChatMessageReceivedHandler;
    }

    // called when receiving a chat message
    private void OnChatMessageReceivedHandler(object sender, ChatLiteNetTransport.OnChatReceiveEventArgs e)
    {
        NetDataReader reader = new NetDataReader(e.message);
        bool fromServer = reader.GetBool();
        string message = reader.GetString();
        if(fromServer && _sandbox.IsClient)
            OnClientReceiveChatMessage?.Invoke(this, new OnClientReceiveChatMessageEventArgs(message));
        if(!fromServer && _sandbox.IsServer)
            OnServerReceiveChatMessage(message, e.connection);
    }

    private void OnServerReceiveChatMessage(string message, NetickConnection connection)
    {
        if(!_connectionManager.GetNetConByNetickConnection(connection, out NetworkConnection netConn))
        {
            Debug.LogError("NetworkConnection not found");
            return;
        }
        SendChatMessageToAll($"<color=red>[client {netConn.Id}]</color> {message}");
        Debug.Log($"[FROM CLIENT] {message} {_sandbox.name}");
    }

    public void SendChatMessageToOne(string message, NetworkConnection client)
    {
        SendChatMessage(message, client);
    }

    public void SendChatMessageToAll(string message)
    {
        foreach (NetworkConnection client in _connectionManager.GetConnections())
        {
            SendChatMessage(message, client);
        }
    }
    private void SendChatMessage(string message, NetworkConnection client)
    {
        _writer.Reset();
        _writer.Put(true); // true = sent by the server
        _writer.Put(message);
        ChatLiteNetTransport.LNLConnection connection = (ChatLiteNetTransport.LNLConnection)client.TransportConnection;
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
        _writer.Put(false); // false = sent by a client
        _writer.Put(message);
        ChatLiteNetTransport.LNLConnection connection = (ChatLiteNetTransport.LNLConnection)_connectionManager.ServerConnection.TransportConnection;
        connection.ChatSend(_writer.Data, _writer.Data.Length);
    }
}
