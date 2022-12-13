using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Transport;

public class EventHandler : ChatNetworkEventsListner
{
    private ChatMessenger _chat;
    [SerializeField] private GameObject _chatUIPrefab; 

    private void Awake()
    {
        _chat = GetComponent<ChatMessenger>();
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        base.OnClientConnected(sandbox, client);

        _chat.SendChatMessageToOne("I'm sending a very long message to test if the code will break or still hold up good but i think there will be a moment were the sentence will be cut and you'll never be able to read all of it", client);
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        base.OnConnectedToServer(sandbox, server);

        Instantiate(_chatUIPrefab, Vector3.zero, Quaternion.identity).GetComponent<Chat>()._sandbox = sandbox;
    }
}
