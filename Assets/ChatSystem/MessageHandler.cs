using LiteNetLib.Utils;
using Netick.Unity;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using UnityEngine;

namespace ChatSystem
{
    public class MessageHandler : MonoBehaviour
    {
        private NetDataReader _netDataReader;
        private IChatNetworkTransport _transport;
        private MessageSender _sender;
        private ConnectionManager _connectionManager;
        private TeamManager _teamManager;

        public event Action<string, string> MessageReceived;

        private void Start()
        {
            NetworkSandbox sandbox = GetComponent<NetworkSandbox>();
            _sender = GetComponent<MessageSender>();
            _connectionManager = GetComponent<ConnectionManager>();
            _teamManager = GetComponent<TeamManager>();
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
            // dispatch the message to all client.
            if (id != 0) // that means the message comes from a client.
            {
                bool toTeam = _netDataReader.GetBool();
                string message = _netDataReader.GetString();
                string sender = _connectionManager.ClientConnections[id].Player.Decorator();
                if(toTeam && _connectionManager.ClientConnections[id].Player.TeamID != 0)
                {
                    Team team = _teamManager.GetTeam(_connectionManager.ClientConnections[id].Player.TeamID);
                    if (team == null) Debug.LogError("Team could not be found");
                    foreach (IChatPlayer player in team.getPlayers())
                    {
                        _sender.SendMessageToClient(player.Connection, sender, message);
                    }
                }
                else
                {
                    _sender.SendMessageToClient(_connectionManager.ClientConnections.Values.ToArray(), sender, message);
                }
            }
            else
            {
                string sender = _netDataReader.GetString();
                string message = _netDataReader.GetString();
                message = HttpUtility.HtmlEncode(message);
                string[] whitelist = { "color=", "color", "b", "lowercase", "uppercase", "i", "s", "u" };
                sender = RichTextFiltering.FilterRichText(sender, whitelist);
                MessageReceived?.Invoke(sender, message);
            }
        }
    }
}
