using LiteNetLib.Utils;
using Netick.Unity;
using System;
using System.Linq;
using UnityEngine;

namespace ChatSystem
{
    public class MessageHandler
    {
        private ChatSystemManager _chatSystem;
        private NetDataReader _netDataReader;
        private IChatNetworkTransport _transport;

        public event Action<string, string> MessageReceived;

        public MessageHandler(ChatSystemManager chatSystemManager, IChatNetworkTransport transport)
        {
            _chatSystem = chatSystemManager;
            _transport = transport;

            _transport.ChatMessageReceived += OnChatMessageReceived;
            _netDataReader = new NetDataReader();
        }

        public void RemoveChatMessageReceivedCallBack()
        {
            _transport.ChatMessageReceived -= OnChatMessageReceived;
        }

        private void OnChatMessageReceived(byte[] data, int id)
        {
            _netDataReader.SetSource(data);
            // dispatch the message to all client.
            if (id != 0) // that means the message comes from a client.
            {
                MessageSender.Destination destination = (MessageSender.Destination)_netDataReader.GetByte();
                string message = _netDataReader.GetString();
                string sender = _chatSystem.ConnectionManager.ClientConnections[id].Player.Decorator(destination);
                if(destination == MessageSender.Destination.team && _chatSystem.ConnectionManager.ClientConnections[id].Player.TeamID != 0)
                {
                    Team team = _chatSystem.TeamManager.GetTeam(_chatSystem.ConnectionManager.ClientConnections[id].Player.TeamID);
                    if (team == null) Debug.LogError("Team could not be found");
                    foreach (IChatPlayer player in team.getPlayers())
                    {
                        _chatSystem.MessageSender.SendMessageToClient(player.Connection, sender, message);
                    }
                }
                else if(destination == MessageSender.Destination.general)
                {
                    _chatSystem.MessageSender.SendMessageToClient(_chatSystem.ConnectionManager.ClientConnections.Values.ToArray(), sender, message);
                }
            }
            else
            {
                string sender = _netDataReader.GetString();
                string message = _netDataReader.GetString();
                string[] whitelist = { "color=", "color", "b", "lowercase", "uppercase", "i", "s", "u" };
                sender = RichTextFiltering.FilterRichText(sender, whitelist);
                message = RichTextFiltering.FilterRichText(message, whitelist);
                MessageReceived?.Invoke(sender, message);
            }
        }
    }
}
