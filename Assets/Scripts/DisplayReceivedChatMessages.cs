using ChatSystem;
using Netick.Unity;
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
        ChatSystemManager mgr = Sandbox.GetComponent<ChatSystemManager>();
        mgr.MessageHandler.MessageReceived += OnMessageReceived;
    }

    private void OnMessageReceived (string sender, string message)
    {
        GameObject textInstance = Instantiate(_textPrefab, _content.transform);
        textInstance.GetComponent<TMP_Text>().text = sender + " " + message;
    }
}
