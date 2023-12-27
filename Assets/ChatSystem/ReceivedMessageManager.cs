using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public static class ReceivedMessageManager
    {
        public static void OnMessagedReceived(int sandboxId, string from, string message, int method)
        {
            OnMessageReceivedEvent onMessageReceivedEvent = ChatSystemManager.ChatSystemConfig.GetOnMessageReceivedEventById(method);
            onMessageReceivedEvent?.Invoke(sandboxId, message);
        }

        public static void HandleIncomingBytes(byte[] data)
        {
            string text = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log("received chat message : " + text);
        }
    }
}
