using ChatSystem;
using Netick.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SendTestMessage : NetickBehaviour
{
    private TMP_InputField _inputField;
    private MessageSender _sender;
    private ConnectionManager _manager;

    public override void NetworkStart()
    {
        _inputField = GetComponent<TMP_InputField>();
        _sender = Sandbox.GetComponent<MessageSender>();
        _manager = Sandbox.GetComponent<ConnectionManager>();
    }

    public void OnEndEdit()
    {
        _sender.SendChatMessage(_manager.ServerConnection, _inputField.text);
        _inputField.text = "";
    }
}
