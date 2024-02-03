using LiteNetLib.Utils;
using Netick.Unity;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace ChatSystem
{
    public class MessageSender
    {
        private NetDataWriter _writer;
        public MessageSender()
        {
            _writer = new NetDataWriter();
        }

        public void SendMessageToServer(IChatTransportConnection connection, string message, Destination destination, int playerId = -1)
        {
            SendToServer(connection, message, destination, playerId);
        }

        public void SendMessageToClient(IChatTransportConnection connection, string sender, string message)
        {
            SendToClient(connection, sender, message);
        }

        public void SendMessageToClient(ConnectionManager.ClientConnectionInfos[] connections, string sender, string message)
        {
            foreach (ConnectionManager.ClientConnectionInfos info in connections)
            {
                SendToClient(info.Connection, sender, message);
            }
        }

        private void SendToClient(IChatTransportConnection connection, string sender, string message)
        {
            _writer.Reset();
            _writer.Put(sender);
            _writer.Put(message);
            connection.ChatSend(_writer.Data);
        }

        private void SendToServer(IChatTransportConnection connection, string message, Destination destination, int playerId = -1)
        {
            if(destination == Destination.player && playerId == -1)
            {
                Debug.LogError("You must provide a playerId for sending message to a player");
                return;
            }
            _writer.Reset();
            _writer.Put((byte)destination);
            _writer.Put(message);
            connection.ChatSend(_writer.Data);
        }

        public enum Destination: byte
        {
            general = 0,
            team = 1,
            player = 2,
        }
    }
}