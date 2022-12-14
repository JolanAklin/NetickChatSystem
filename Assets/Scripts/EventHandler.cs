using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Transport;

public class EventHandler : ChatNetworkEventsListner
{
    private ChatMessenger _chat;
    private ScopeManager _scopeManager;
    [SerializeField] private GameObject _client; 

    private void Awake()
    {
        _chat = GetComponent<ChatMessenger>();
        _scopeManager = GetComponent<ScopeManager>();

        _scopeManager.RegisterScope("Red team");
        _scopeManager.RegisterScope("Blue team");
        _scopeManager.RegisterExtendedScope("Teams", new ScopeManager.Scope[] {_scopeManager.GetScope("Red team"), _scopeManager.GetScope("Blue team")});
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        base.OnClientConnected(sandbox, client);

        sandbox.NetworkInstantiate(_client, Vector3.zero, Quaternion.identity, client).GetComponent<Client>()._netConnection = client;
    }
}
