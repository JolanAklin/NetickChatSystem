using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using TMPro;

public class Chat : MonoBehaviour
{
    private ChatMessenger _messenger;
    private ScopeManager _scopeManager;

    public NetworkSandbox _sandbox;

    private Client _client;

    [SerializeField] private TMP_Text _messages;

    private void Start()
    {
        _messenger = _sandbox.FindGameObjectWithTag("NetController").GetComponent<ChatMessenger>();
        _scopeManager = _messenger.GetComponent<ScopeManager>();
        _messenger.OnClientReceiveChatMessage += OnMessageReceived;

        _client = GetComponentInParent<Client>();
    }

    public void SendMessage(TMP_InputField message)
    {
        _client.SendChatMessage(message.text);
        message.text = "";
    }

    public void OnScopeChanged(int value)
    {
        _client.RPC_ChangeScope((Client.ScopeEnum)value);
    }

    public void OnTargetScopeChanged(int value)
    {
        _client.ChangeScope((Client.ScopeEnum)value);
    }

    private void OnMessageReceived(object sender, ChatMessenger.OnClientReceiveChatMessageEventArgs e)
    {
        _messages.text += "\n" + e.message;
    }
}
