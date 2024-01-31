using LiteNetLib.Utils;
using Netick.Unity;
using System.Collections;
using UnityEngine;

namespace ChatSystem
{
    public class MessageSender : MonoBehaviour
    {
        private NetDataWriter _writer = new NetDataWriter();
        public void SendChatMessage(IChatTransportConnection connection, string message)
        {
            Send(connection, message);
        }

        public void SendChatMessage(IChatTransportConnection[] connections, string message)
        {
            foreach (IChatTransportConnection connection in connections)
            {
                Send(connection, message);
            }
        }

        private void Send(IChatTransportConnection connection, string message)
        {
            _writer.Reset();
            _writer.Put(message);
            connection.ChatSend(_writer.Data);
        }
    }
}