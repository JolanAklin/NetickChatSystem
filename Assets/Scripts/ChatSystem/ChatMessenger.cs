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
    [Tooltip("If true, client are allowed to send message in any scopes, disregarding theirs.")]
    [SerializeField] private bool _allowForeignSends = false;

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
        // when the client and the server run on the same machine, both receive server and client messages.
        // this is for separating them
        if(fromServer && _sandbox.IsClient)
        {
            string message = ReadMessageFromServer(reader);
            OnClientReceiveChatMessage?.Invoke(this, new OnClientReceiveChatMessageEventArgs(message));
        }
        if(!fromServer && _sandbox.IsServer)
        {
            string message = ReadMessageFromClient(reader, out bool isScopeTarget, out uint value);
            if(isScopeTarget) // client is targeting a scope
            {
                ScopeManager.Scope targetScope = _scopeManager.GetScope(value);
                if(targetScope == null)
                    return;
                OnServerReceiveChatMessage(message, targetScope, e.connection);
            }
            else // client is target another client
                OnServerReceiveChatMessage(message, value, e.connection);
        }
    }

    #region readers
    // only the client will specify a target in his messages
    private string ReadMessageFromClient(NetDataReader reader, out bool isScopeTarget, out uint value)
    {
        isScopeTarget = reader.GetBool();
        value = reader.GetUInt();
        return reader.GetString();
    }

    private string ReadMessageFromServer(NetDataReader reader)
    {
        return reader.GetString();
    }
    #endregion

    // Clients have to send their message to the server and the server then dispatches them to the right clients
    // sender is the client that sent the message to the server
    private void OnServerReceiveChatMessage(string message, ScopeManager.Scope target, NetickConnection sender)
    {
        if(!_connectionManager.GetNetConByNetickConnection(sender, out NetworkConnection netConn))
        {
            Debug.LogError("NetworkConnection not found");
            return;
        }
        _connectionManager.GetNetConByNetickConnection(sender, out NetworkConnection netCon);

        if(!_allowForeignSends)
        {
            if(!GetScope(netCon).CheckAgainst(target))
            {
                Debug.LogWarning("Received a foreign send, discarding.");
                return;
            }
        }
        // @TODO make a function to get the styler data out of there.
        SendChatMessageToScope(message, new DefaultStyle.DefaultStylerData(true, netCon.Id), target);
    }

    private void OnServerReceiveChatMessage(string message, uint NetConnectionID, NetickConnection sender)
    {
        if (!_connectionManager.GetNetConByNetickConnection(sender, out NetworkConnection netConn))
        {
            Debug.LogError("NetworkConnection not found");
            return;
        }
        _connectionManager.GetNetConByNetickConnection(sender, out NetworkConnection netCon);
        // @TODO send message to only on client
        Debug.LogWarning("this is not working at the moment");
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
            if(_connectionsScope[client].CheckAgainst(scope))
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
    public void SendToServer(string message, ScopeManager.Scope target)
    {
        if(_sandbox.IsServer)
        {
            Debug.LogWarning("Message not sent. You are calling this function from the server. It should only be called on the client.");
            return;
        }
        _writer.Reset();
        _writer.Put(false); // false = sent by a client
        _writer.Put(true); // if true, the next int will be a scope. If false the next int will be a NetworkConnection ID.
        _writer.Put(target.scope);
        _writer.Put(message);
        ChatLiteNetTransport.LNLConnection connection = (ChatLiteNetTransport.LNLConnection)_connectionManager.ServerConnection.TransportConnection;
        connection.ChatSend(_writer.Data, _writer.Data.Length);
    }
}
