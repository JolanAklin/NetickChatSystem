using ChatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public class ConnectionManager : MonoBehaviour
    {
        public Dictionary<int, IChatTransportConnection> ClientConnections = new Dictionary<int, IChatTransportConnection>();

        public IChatTransportConnection ServerConnection;
    }
}
