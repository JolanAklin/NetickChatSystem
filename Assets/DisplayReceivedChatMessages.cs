using ChatSystem;
using Netick.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayReceivedChatMessages : NetickBehaviour
{
    [SerializeField]
    private GameObject _content;
    [SerializeField]
    private GameObject _textPrefab;

    public override void NetworkStart()
    {
        MessageHandler handler = Sandbox.GetComponent<MessageHandler>();
        handler.MessageReceived += OnMessageReceived;
    }

    private void OnMessageReceived (string message)
    {
        GameObject textInstance = Instantiate(_textPrefab, _content.transform);
        textInstance.GetComponent<TMP_Text>().text = message;
    }
}
