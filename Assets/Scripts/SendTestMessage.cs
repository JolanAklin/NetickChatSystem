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

    public override void NetworkStart()
    {
        _inputField = GetComponent<TMP_InputField>();
        _chatSystem = Sandbox.GetComponent<ChatSystemManager>();
    }

    public void OnEndEdit()
    {
        _chatSystem.MessageSender.SendMessageToServer(_chatSystem.ConnectionManager.ServerConnection, _inputField.text, MessageSender.Destination.general);
        _inputField.text = "";
        _inputField.Select();
    }
}
