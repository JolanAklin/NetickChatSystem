using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;

public class EventHandler : ChatNetworkEventsListner
{
    private ChatMessager _chat;

    new protected void Awake()
    {
        base.Awake();
        _chat = GetComponent<ChatMessager>();
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        base.OnClientConnected(sandbox, client);

        _chat.SendChatMessageToAll("I'm sending a very long message to test if the code will break or still hold up good but i think there will be a moment were the sentence will be cut and you'll never be able to read all of it");
    }
}
