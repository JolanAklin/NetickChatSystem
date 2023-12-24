using ChatSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnMessageReceivedEventListener : MonoBehaviour
{
    [SerializeField] protected OnMessageReceivedEvent _onMessageReceived;
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


    public void RaiseEvent(string message)
    {
        _messageDisplay?.Display(message);
    }
}
