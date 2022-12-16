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

namespace NetickChatSystem
{
    public class ConnectionsManager : MonoBehaviour
    {
        // network connection id and it's related network connection
        private Dictionary<int, NetworkConnection> _connections;
        // use for linking a netickconnection to it's related networkconnection
        private Dictionary<NetickConnection, NetworkConnection> _netickConnection2NetworkConnection;

        private NetworkConnection _serverConnection;
        public NetworkConnection ServerConnection { get => _serverConnection; }

        private ChatLiteNetTransport _transport;

        #region events
        // called when an item is added in _connections
        public event System.EventHandler<OnNetworkConnectionEventArgs> OnNetworkConnectionAdded;
        // called when an item is removed from _connections
        public event System.EventHandler<OnNetworkConnectionEventArgs> OnNetworkConnectionRemoved;

        public class OnNetworkConnectionEventArgs : System.EventArgs
        {
            public NetworkConnection connection;
        }
        #endregion

        private void Awake() {
            _connections = new Dictionary<int, NetworkConnection>();
            _netickConnection2NetworkConnection = new Dictionary<NetickConnection, NetworkConnection>();
        }

        #region getters and setters
        public bool GetNetConById(int id, out NetworkConnection connection)
        {
            return _connections.TryGetValue(id, out connection);
        }
        public bool GetNetConByNetickConnection(NetickConnection nConnection, out NetworkConnection connection)
        {
            return _netickConnection2NetworkConnection.TryGetValue(nConnection, out connection);
        }

        public bool AddId2NetCon(int id, NetworkConnection connection)
        {
            if(_connections.ContainsKey(id))
                return false;
            _connections.Add(id, connection);
            OnNetworkConnectionAdded?.Invoke(this, new OnNetworkConnectionEventArgs() {connection = connection});
            return true;
        }

        public bool AddNetickConnection2NetCon(NetickConnection nConnection, NetworkConnection connection)
        {
            if (_netickConnection2NetworkConnection.ContainsKey(nConnection))
                return false;
            _netickConnection2NetworkConnection.Add(nConnection, connection);
            return true;
        }

        public bool RemoveNetConById(int id)
        {
            if(_connections.ContainsKey(id))
                OnNetworkConnectionRemoved?.Invoke(this, new OnNetworkConnectionEventArgs() { connection = _connections[id] });
            return _connections.Remove(id);
        }

        public bool RemoveNetConByNetickConnection(NetickConnection nConnection)
        {
            return _netickConnection2NetworkConnection.Remove(nConnection);
        }

        public void SetServerConnection(NetworkConnection serverConnection)
        {
            if(_serverConnection != null)
            {
                Debug.Log("You are trying to overwrite the server NetworkConnection. This is not permitted. Call Clear() first.");
                return;
            }
            _serverConnection = serverConnection;
        }

        public NetworkConnection[] GetConnections()
        {
            NetworkConnection[] conns = new NetworkConnection[_connections.Count];
            _connections.Values.CopyTo(conns, 0);
            return conns;
        }
        #endregion
        
        public void Clear()
        {
            _connections.Clear();
            _netickConnection2NetworkConnection.Clear();
            _serverConnection = null;
        }
    }
}