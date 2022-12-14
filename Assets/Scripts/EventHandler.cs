using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Transport;

public class EventHandler : ChatNetworkEventsListner
{
    private ChatMessenger _chat;
    private ScopeManager _scopeManager;
    [SerializeField] private GameObject _chatUIPrefab; 

    private void Awake()
    {
        _chat = GetComponent<ChatMessenger>();
        _scopeManager = GetComponent<ScopeManager>();
        _scopeManager.RegisterScope("Red team");
        _scopeManager.RegisterScope("Blue team");
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        base.OnClientConnected(sandbox, client);

        _chat.SendChatMessageToOne("I'm sending a very long message to test if the code will break or still hold up good but i think there will be a moment were the sentence will be cut and you'll never be able to read all of it", client, ChatMessenger.Styler.GenerateData());
        if(client.Id == 0)
        {
            _chat.GetScope(client).AddScope(_scopeManager.GetScope("Red team"));
        }
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        base.OnConnectedToServer(sandbox, server);

        Instantiate(_chatUIPrefab, Vector3.zero, Quaternion.identity).GetComponent<Chat>()._sandbox = sandbox;
    }
}
