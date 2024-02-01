﻿using LiteNetLib.Utils;
using Netick.Unity;
using System.Collections;
using UnityEngine;

namespace ChatSystem
{
    public class MessageSender : MonoBehaviour
    {
        private NetDataWriter _writer;
        private void Awake()
        {
            _writer = new NetDataWriter();
        }

        public void SendMessageToServer(IChatTransportConnection connection, string message)
        {
            SendToServer(connection, message);
        }

        public void SendMessageToServer(IChatTransportConnection[] connections, string message)
        {
            foreach (IChatTransportConnection connection in connections)
            {
                SendToServer(connection, message);
            }
        }

        public void SendMessageToClient(IChatTransportConnection connection, string sender, string message)
        {
            SendToClient(connection, message, sender);
        }

        public void SendMessageToClient(IChatTransportConnection[] connections, string sender, string message)
        {
            foreach (IChatTransportConnection connection in connections)
            {
                SendToClient(connection, sender, message);
            }
        }

        private void SendToClient(IChatTransportConnection connection, string sender, string message)
        {
            _writer.Reset();
            _writer.Put(sender);
            _writer.Put(message);
            connection.ChatSend(_writer.Data);
        }

        private void SendToServer(IChatTransportConnection connection, string message)
        {
            _writer.Reset();
            _writer.Put(message);
            connection.ChatSend(_writer.Data);
        }
    }
}