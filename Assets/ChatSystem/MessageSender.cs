﻿using Netick.Unity;
using System.Collections;
using UnityEngine;

namespace ChatSystem
{
    public class MessageSender : MonoBehaviour
    {
        public void SendChatMessage(IChatTransportConnection connection, string message)
        {
            connection.ChatSend(System.Text.Encoding.UTF8.GetBytes(message));
        }
    }
}