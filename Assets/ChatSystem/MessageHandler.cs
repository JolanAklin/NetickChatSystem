using LiteNetLib.Utils;
using Netick.Unity;
using System;
using System.Linq;
using UnityEngine;

namespace ChatSystem
{
    public class MessageHandler : MonoBehaviour
    {
        private NetDataReader _netDataReader;
        private IChatNetworkTransport _transport;
        private MessageSender _sender;
        private ConnectionManager _connectionManager;

        public event Action<string> MessageReceived;

        private void Start()
        {
            NetworkSandbox sandbox = GetComponent<NetworkSandbox>();
            _sender = GetComponent<MessageSender>();
            _connectionManager = GetComponent<ConnectionManager>();
            _transport = (IChatNetworkTransport)sandbox.Transport;
            _transport.ChatMessageReceived += OnChatMessageReceived;

            _netDataReader = new NetDataReader();
        }

        private void OnDestroy()
        {
            _transport.ChatMessageReceived -= OnChatMessageReceived;
        }

        private void OnChatMessageReceived(byte[] data, int id)
        {
            _netDataReader.SetSource(data);
            string message = _netDataReader.GetString();
            // dispatch the message to all client.
            if(id != 0) // that means the message comes from a client.
            {
                _sender.SendChatMessage(_connectionManager.ClientConnections.Values.ToArray(), message);
            }
            else
            {
                MessageReceived?.Invoke(message);
            }
        }
    }

}
