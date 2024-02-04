using Netick;
using Netick.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public class ChatSystemManager: MonoBehaviour
    {
        [SerializeField]
        private List<Team> _teams = new List<Team>();

        public ConnectionManager ConnectionManager {  get; private set; }
        public MessageSender MessageSender { get; private set; }
        public MessageHandler MessageHandler { get; private set; }
        public NetworkSandbox Sandbox { get; private set; }
        public TeamManager TeamManager { get; private set; }

        private void Start()
        {
            Sandbox = GetComponent<NetworkSandbox>();
            ConnectionManager = new ConnectionManager();
            MessageSender = new MessageSender();
            MessageHandler = new MessageHandler(this, (IChatNetworkTransport)Sandbox.Transport);
            TeamManager = new TeamManager(_teams);
        }

        private void OnDestroy()
        {
            MessageHandler.RemoveChatMessageReceivedCallBack();
        }

        public void RegisterPlayer(GameObject player, NetworkConnection client)
        {
            if (player.TryGetComponent(out IChatPlayer chatPlayer))
            {
                ConnectionManager.ClientConnections.Add(client.PlayerId, new ConnectionManager.ClientConnectionInfos((IChatTransportConnection)client.TransportConnection, chatPlayer));
                chatPlayer.Connection = (IChatTransportConnection)client.TransportConnection;
            }
            else
                Debug.LogError("The player must implement IChatPlayer");
        }

        public void SaveServerConnection(NetworkConnection connection)
        {
            ConnectionManager.ServerConnection = (IChatTransportConnection)connection.TransportConnection;
        }
    }
}
