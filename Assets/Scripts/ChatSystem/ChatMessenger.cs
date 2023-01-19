/*


MIT License

Copyright (c) 2022 Jolan Aklin

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Transport;
using LiteNetLib.Utils;

namespace NetickChatSystem
{
    [RequireComponent(typeof(ChatNetworkEventsListner))]
    [RequireComponent(typeof(ConnectionsManager))]
    public class ChatMessenger : MonoBehaviour
    {
        public bool isInitialized {get; private set;}
        private NetworkSandbox _sandbox;
        private ChatNetworkEventsListner _listener;
        private NetDataWriter _writer = new NetDataWriter();
        private ConnectionsManager _connectionManager;

        private Dictionary<NetworkConnection, Scope> _connectionsScope = new Dictionary<NetworkConnection, Scope>();

        [SerializeField] private SenderStyler _senderStyle;
        public static SenderStyler Styler {get; private set;}
        [Tooltip("If true, client are allowed to send message in any scopes, disregarding theirs.")]
        [SerializeField] private ForeignSendsPolicy _foreignSendsGlobalPolicy = ForeignSendsPolicy.definedByScope;

        public enum ForeignSendsPolicy
        {
            forceOff,
            definedByScope,
            forceOn
        }

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
            ChatSystem.Instance.AddChatMessenger(sandbox, this);
            _listener = listner;
            _connectionManager = GetComponent<ConnectionsManager>();
            ChatLiteNetTransport._onChatReceive += OnChatMessageReceivedHandler;
            Styler = _senderStyle;

            _connectionManager.OnNetworkConnectionAdded += OnNewConnection;
            _connectionManager.OnNetworkConnectionRemoved += OnRemoveConnection;

            isInitialized = true;
        }

        // register a scope for the new connection
        private void OnNewConnection(object sender, ConnectionsManager.OnNetworkConnectionEventArgs e)
        {
            _connectionsScope.Add(e.connection, new Scope(0, "", true));
        }
        // remove the connection's scope
        private void OnRemoveConnection(object sender, ConnectionsManager.OnNetworkConnectionEventArgs e)
        {
            _connectionsScope.Remove(e.connection);
        }

        public Scope GetScope(NetworkConnection connection)
        {
            if(_connectionsScope.TryGetValue(connection, out Scope scope))
                return scope;
            return null;
        }

        public void RenamePlayerScope(NetworkConnection connection, string newName)
        {
            if (_connectionsScope.TryGetValue(connection, out Scope scope))
            {
                scope.ChangeName(newName);
            }
        }

        public void OnDestroy()
        {
            if(ChatSystem.Instance != null)
                ChatSystem.Instance.RemoveChatMessenger(_sandbox);
            ChatLiteNetTransport._onChatReceive -= OnChatMessageReceivedHandler;
            if(_connectionManager != null)
            {
                _connectionManager.OnNetworkConnectionAdded -= OnNewConnection;
                _connectionManager.OnNetworkConnectionRemoved -= OnRemoveConnection;
            }
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
                    Scope targetScope = ScopeManager.Instance.GetScope(value);
                    if(targetScope == null)
                        return;
                    OnServerReceiveChatMessage(message, targetScope, e.connection);
                }
                else // client is targeting another client
                    OnServerReceiveChatMessage(message, (int)value, e.connection);
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
        private void OnServerReceiveChatMessage(string message, Scope target, NetickConnection sender)
        {
            if(!_connectionManager.GetNetConByNetickConnection(sender, out NetworkConnection netConn))
            {
                Debug.LogError("NetworkConnection not found");
                return;
            }
            _connectionManager.GetNetConByNetickConnection(sender, out NetworkConnection netCon);


            bool isForeignSend = !GetScope(netCon).CheckAgainst(target);
            switch (_foreignSendsGlobalPolicy)
            {
                case ForeignSendsPolicy.forceOff:
                    if (isForeignSend)
                    {
                        Debug.LogWarning($"The global foreign send policy is set to : {_foreignSendsGlobalPolicy}. The received message will be discarded.");
                        return;
                    }
                    break;
                case ForeignSendsPolicy.definedByScope:
                    if (isForeignSend)
                    {
                        switch (target.foreignReceivePolicy)
                        {
                            case Scope.ForeignReceivePolicy.forbidden:
                                Debug.LogWarning($"The scope foreign receive policy is set to {target.foreignReceivePolicy}. The message will be discarded.");
                                return;
                            case Scope.ForeignReceivePolicy.authorized:
                                break;
                        }
                    }
                    break;
                case ForeignSendsPolicy.forceOn:
                    break;
            }
            SendChatMessageToScope(message, _senderStyle.GenerateData(netCon.Id, target, GetScope(netCon), isForeignSend), target);
        }

        // send the received message to the right client
        private void OnServerReceiveChatMessage(string message, int NetConnectionID, NetickConnection sender)
        {
            if (!_connectionManager.GetNetConByNetickConnection(sender, out NetworkConnection senderNetCon))
            {
                Debug.LogError("NetworkConnection not found");
                return;
            }
            if(!_connectionManager.GetNetConById(NetConnectionID, out NetworkConnection netCon))
            {
                Debug.LogError($"No client with the ID {NetConnectionID} exist");
                return;
            }
            SendChatMessage(message, netCon, _senderStyle.GenerateData(senderNetCon.Id, netCon.Id, GetScope(senderNetCon)));
        }

        ///<summary>
        /// Send a message to a particular client. Server only
        ///</summary>
        public void SendChatMessageToOne(string message, NetworkConnection client)
        {
            if (_sandbox.IsClient)
            {
                Debug.LogWarning("Message not sent. You are calling this function from the client. It should only be called on the server.");
                return;
            }
            SendChatMessage(message, client, _senderStyle.GenerateData());
        }

        ///<summary>
        /// Send a message to all the client that match the given scope. Server only.
        ///</summary>
        private void SendChatMessageToScope(string message, SenderStyler.StylerData data, Scope scope)
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

        ///<summary>
        /// Send a message to all the client that match the given scope. Server only.
        ///</summary>
        public void SendChatMessageToScope(string message, Scope target)
        {
            SendChatMessageToScope(message, _senderStyle.GenerateData(target), target);
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
        public void SendToServer(string message, Scope target)
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

        ///<summary>
        /// Send a message to the server and the server will send it to the given client
        ///</summary>
        public void SendToServer(string message, int clientId)
        {
            if (_sandbox.IsServer)
            {
                Debug.LogWarning("Message not sent. You are calling this function from the server. It should only be called on the client.");
                return;
            }
            _writer.Reset();
            _writer.Put(false); // false = sent by a client
            _writer.Put(false); // if true, the next int will be a scope. If false the next int will be a NetworkConnection ID.
            _writer.Put(clientId);
            _writer.Put(message);
            ChatLiteNetTransport.LNLConnection connection = (ChatLiteNetTransport.LNLConnection)_connectionManager.ServerConnection.TransportConnection;
            connection.ChatSend(_writer.Data, _writer.Data.Length);
        }
    }
}