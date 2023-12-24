using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public static class ReceivedMessageManager
    {
        public static void OnMessagedReceived(string from, string message, int method)
        {
            OnMessageReceivedEvent onMessageReceivedEvent = Config.instance.GetOnMessageReceivedEventById(method);
            onMessageReceivedEvent?.Invoke(message);
        }
    }
}
