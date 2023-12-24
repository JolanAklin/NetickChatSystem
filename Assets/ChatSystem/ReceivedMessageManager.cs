using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public class ReceivedMessageManager : MonoBehaviour
    {
        public void OnMessagedReceived(string from, string message, int method)
        {
            OnMessageReceivedEvent onMessageReceivedEvent = Config.instance.GetOnMessageReceivedEventById(method);
            onMessageReceivedEvent?.Invoke(message);
        }

        private void Start()
        {
            OnMessagedReceived("l","test", 0);
        }
    }
}
