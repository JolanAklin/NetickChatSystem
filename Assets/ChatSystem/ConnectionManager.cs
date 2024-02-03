using ChatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public class ConnectionManager
    {
        public Dictionary<int, ClientConnectionInfos> ClientConnections;
        public IChatTransportConnection ServerConnection;

        public ConnectionManager()
        {
            ClientConnections = new Dictionary<int, ClientConnectionInfos>();
        }

        public struct ClientConnectionInfos
        {
            public IChatTransportConnection Connection {get; private set;}
            public IChatPlayer Player { get; private set;}

            public ClientConnectionInfos(IChatTransportConnection connection, IChatPlayer player)
            {
                Connection = connection;
                Player = player;
            }
        }
    }
}
