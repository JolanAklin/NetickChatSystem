using ChatSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnMessageReceivedEventListener : MonoBehaviour
{
    [SerializeField] protected OnMessageReceivedEvent _onMessageReceived;
    [SerializeField] protected ChatSystemManager _chatSystemManager;
    private AbstractMessageDisplay _messageDisplay;

    private void Awake() { 
        _onMessageReceived.Register(this);
        _messageDisplay = GetComponent<AbstractMessageDisplay>();
        if( _messageDisplay == null )
        {
            Debug.LogError("No message display class found on \"" + this.name + "\"");
        }
    }
    private void OnDestroy() => _onMessageReceived.Deregister(this);


    public void RaiseEvent(int sandboxId, string message)
    {
        if (sandboxId != _chatSystemManager.SandboxId) return;
        _messageDisplay?.Display(message);
    }
}
