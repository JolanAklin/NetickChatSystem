using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Transport;
using LiteNetLib.Utils;

[RequireComponent(typeof(ChatNetworkEventsListner))]
[RequireComponent(typeof(ConnectionsManager))]
[RequireComponent(typeof(ScopeManager))]
public class ChatMessenger : MonoBehaviour
{
    public bool isInitialized {get; private set;}
    private NetworkSandbox _sandbox;
    private ChatNetworkEventsListner _listener;
    private NetDataWriter _writer = new NetDataWriter();
    private ConnectionsManager _connectionManager;
    private ScopeManager _scopeManager;

    private Dictionary<NetworkConnection, ScopeManager.Scope> _connectionsScope = new Dictionary<NetworkConnection, ScopeManager.Scope>();

    [SerializeField] private SenderStyler _senderStyle;

    public event System.EventHandler<OnClientReceiveChatMessageEventArgs> OnClientReceiveChatMessage;

    public class OnClientReceiveChatMessageEventArgs : System.EventArgs {
        public string message;

        public OnClientReceiveChatMessageEventArgs (string message)
        {
            this.message = message;
        }
    }

    // initialize the chat manager
    public void Init (NetworkSandbox sandbox, ChatNetworkEventsListner listner)
    {
        _sandbox = sandbox;
        _listener = listner;
        _connectionManager = GetComponent<ConnectionsManager>();
        _scopeManager = GetComponent<ScopeManager>();
        ChatLiteNetTransport._onChatReceive += OnChatMessageReceivedHandler;

        _connectionManager.OnNetworkConnectionAdded += OnNewConnection;
        _connectionManager.OnNetworkConnectionRemoved += OnRemoveConnection;

        isInitialized = true;
    }

    // register a scope for the new connection
    private void OnNewConnection(object sender, ConnectionsManager.OnNetworkConnectionEventArgs e)
    {
        _connectionsScope.Add(e.connection, ScopeManager.Scope.Everyone);
    }
    // remove the connection's scope
    private void OnRemoveConnection(object sender, ConnectionsManager.OnNetworkConnectionEventArgs e)
    {
        _connectionsScope.Remove(e.connection);
    }

    public ScopeManager.Scope GetScope(NetworkConnection connection)
    {
        if(_connectionsScope.TryGetValue(connection, out ScopeManager.Scope scope))
            return scope;
        return null;
    }

    public void OnDestroy()
    {
        ChatLiteNetTransport._onChatReceive -= OnChatMessageReceivedHandler;
        _connectionManager.OnNetworkConnectionAdded -= OnNewConnection;
        _connectionManager.OnNetworkConnectionRemoved -= OnRemoveConnection;
    }

    // called when receiving a chat message
    private void OnChatMessageReceivedHandler(object sender, ChatLiteNetTransport.OnChatReceiveEventArgs e)
    {
        NetDataReader reader = new NetDataReader(e.message);
        bool fromServer = reader.GetBool();
        string message = reader.GetString();
        // when the client and the server run on the same machine, both receive server and client messages.
        // this is for separating them
        if(fromServer && _sandbox.IsClient)
            OnClientReceiveChatMessage?.Invoke(this, new OnClientReceiveChatMessageEventArgs(message));
        if(!fromServer && _sandbox.IsServer)
            OnServerReceiveChatMessage(message, e.connection);
    }

    // client have to send theyre message to the server and the server then dispatches them to the right clients
    private void OnServerReceiveChatMessage(string message, NetickConnection connection)
    {
        if(!_connectionManager.GetNetConByNetickConnection(connection, out NetworkConnection netConn))
        {
            Debug.LogError("NetworkConnection not found");
            return;
        }
        _connectionManager.GetNetConByNetickConnection(connection, out NetworkConnection netCon);
        SendChatMessageToScope(message, new DefaultStyle.DefaultStylerData(true, netCon.Id), ScopeManager.Scope.Everyone);
        Debug.Log($"[FROM CLIENT] {message} {_sandbox.name}");
    }

    ///<summary>
    /// Send a message to a particular client. Server only
    ///</summary>
    public void SendChatMessageToOne(string message, NetworkConnection client, SenderStyler.StylerData data)
    {
        if (_sandbox.IsClient)
        {
            Debug.LogWarning("Message not sent. You are calling this function from the client. It should only be called on the server.");
            return;
        }
        SendChatMessage(message, client, data);
    }

    ///<summary>
    /// Send a message to all the client that match the given scope. Server only.
    ///</summary>
    public void SendChatMessageToScope(string message, SenderStyler.StylerData data, ScopeManager.Scope scope)
    {
        if (_sandbox.IsClient)
        {
            Debug.LogWarning("Message not sent. You are calling this function from the client. It should only be called on the server.");
            return;
        }
        foreach (NetworkConnection client in _connectionManager.GetConnections())
        {
            if(_connectionsScope[client].CheckAgainst(_scopeManager.GetScope("Red team")))
                SendChatMessage(message, client, data);
        }
    }

    private void SendChatMessage(string message, NetworkConnection client, SenderStyler.StylerData data)
    {
        _writer.Reset();
        _writer.Put(true); // true = sent by the server
        _writer.Put(_senderStyle.GetSenderStyle(data) + message);
        ChatLiteNetTransport.LNLConnection connection = (ChatLiteNetTransport.LNLConnection)client.TransportConnection;
        connection.ChatSend(_writer.Data, _writer.Data.Length);
    }

    ///<summary>
    /// Send a message to the server to dispatch to other client with the given scope
    ///</summary>
    public void SendToServer(string message, ScopeManager.Scope scope)
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
