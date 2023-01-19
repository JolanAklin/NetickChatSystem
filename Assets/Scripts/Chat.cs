using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using TMPro;
using NetickChatSystem;

public class Chat : MonoBehaviour
{
    private ChatMessenger _messenger;
    private ScopeManager _scopeManager;

    public NetworkSandbox _sandbox;

    public Client _client;

    [SerializeField] private TMP_Text _messages;

    private void Start()
    {
        _messenger = ChatSystem.Instance.GetChatMessenger(_sandbox);
        _scopeManager = ScopeManager.Instance;
        _messenger._chatDisplay.AddDisplay(Displays.chat, (string message) => {OnMessageReceived(message);});

        _client = GetComponentInParent<Client>();
    }

    private void OnDestroy()
    {
        _messenger._chatDisplay.AddDisplay(Displays.chat, null);
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

    private void OnMessageReceived(string message)
    {
        _messages.text += "\n" + message;
    }

    private void SendToClient()
    {
        _client.SendChatMessage("test", 0);
    }
}
