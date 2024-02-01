using LiteNetLib.Utils;
using Netick.Unity;
using System;
using System.Linq;
using UnityEngine;

namespace ChatSystem
{
    public class MessageHandler : MonoBehaviour
    {
        private NetDataReader _netDataReader = new NetDataReader();
        LNLTransportProviderWchat _transport; // TODO change this stuff so it doesnt depend on LNLTransportProviderWChat
        private MessageSender _sender;
        private ConnectionManager _connectionManager;

        public event Action<string> MessageReceived;

        private void Start()
        {
            NetworkSandbox sandbox = GetComponent<NetworkSandbox>();
            _sender = GetComponent<MessageSender>();
            _connectionManager = GetComponent<ConnectionManager>();
            _transport = (LNLTransportProviderWchat)sandbox.Transport;
            _transport.ChatMessageReceived += OnChatMessageReceived;
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
