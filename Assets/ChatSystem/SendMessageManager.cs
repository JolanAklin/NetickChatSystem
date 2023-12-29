using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public static class SendMessageManager
    {
        public static void SendMessage(string text, ChatTransportConnection connection)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(text);
            connection.ChatSend(data);
        }
    }
}
