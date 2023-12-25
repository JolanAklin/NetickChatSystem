using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// from https://www.youtube.com/watch?v=lgA8KirhLEU and modified for the needs
namespace ChatSystem
{
    [CreateAssetMenu(fileName = "OnMessageReceivedEvent", menuName = "Chat system/OnMessageReceivedEvent")]
    public class OnMessageReceivedEvent : ScriptableObject
    {
        public uint _eventId;

        HashSet<OnMessageReceivedEventListener> _listeners = new HashSet<OnMessageReceivedEventListener>();

        public void Invoke(int sandboxId, string message)
        {
            foreach (var listener in _listeners)
            {
                listener.RaiseEvent(sandboxId, message);
            }
        }

        public void Register(OnMessageReceivedEventListener onMessageReceivedEventListener) => _listeners.Add(onMessageReceivedEventListener);
        public void Deregister(OnMessageReceivedEventListener onMessageReceivedEventListener) => _listeners.Remove(onMessageReceivedEventListener);
    }
}
