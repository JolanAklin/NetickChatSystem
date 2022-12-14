using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using TMPro;

public class Chat : MonoBehaviour
{
    private ChatMessenger _messenger;

    public NetworkSandbox _sandbox;

    [SerializeField] private TMP_Text _messages;

    private void Start()
    {
        _messenger = _sandbox.FindGameObjectWithTag("NetController").GetComponent<ChatMessenger>();
        _messenger.OnClientReceiveChatMessage += OnMessageReceived;
    }

    public void SendMessage(TMP_InputField message)
    {
        _messenger.SendToServer(message.text, ScopeManager.Scope.Everyone);
        message.text = "";
    }

    private void OnMessageReceived(object sender, ChatMessenger.OnClientReceiveChatMessageEventArgs e)
    {
        _messages.text += "\n" + e.message;
    }
}
