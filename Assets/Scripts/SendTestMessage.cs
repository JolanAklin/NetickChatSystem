using ChatSystem;
using Netick.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SendTestMessage : NetickBehaviour
{
    private TMP_InputField _inputField;

    private ChatSystemManager _chatSystem;

    private MessageSender.Destination _destination;

    public override void NetworkStart()
    {
        _inputField = GetComponent<TMP_InputField>();
        _chatSystem = Sandbox.GetComponent<ChatSystemManager>();
        _destination = MessageSender.Destination.general;
    }

    public void OnDestinationChanged(TMP_Dropdown dropdown)
    {
        _destination = (MessageSender.Destination)dropdown.value;
    }

    public void OnEndEdit()
    {
        _chatSystem.MessageSender.SendMessageToServer(_chatSystem.ConnectionManager.ServerConnection, _inputField.text, _destination);
        _inputField.text = "";
        _inputField.Select();
    }
}
