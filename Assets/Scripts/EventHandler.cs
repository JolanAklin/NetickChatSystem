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
    [SerializeField] private GameObject _ui;

    private void Awake()
    {
        _chat = GetComponent<ChatMessenger>();
        _scopeManager = GetComponent<ScopeManager>();

        _scopeManager.RegisterScope("Red team");
        _scopeManager.RegisterScope("Blue team");
        _scopeManager.RegisterExtendedScope("Teams", new ScopeManager.Scope[] {_scopeManager.GetScope("Red team"), _scopeManager.GetScope("Blue team")}, ScopeManager.Scope.CheckPolicy.loose, ScopeManager.Scope.ForeignReceivePolicy.forbidden);
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        base.OnClientConnected(sandbox, client);

        NetworkObject obj = sandbox.NetworkInstantiate(_client, Vector3.zero, Quaternion.identity, client);
        client.PlayerObject = obj.gameObject;
        obj.GetComponent<Client>()._netConnection = client;

        _chat.SendChatMessageToOne($"Welcome to the server Client {client.Id}", client);
        _chat.SendChatMessageToScope($"Client {client.Id} connected", ScopeManager.Scope.Everyone);
    }

    public override void OnClientDisconnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        base.OnClientDisconnected(sandbox, client);
        _chat.SendChatMessageToScope($"Client {client.Id} disconnected", ScopeManager.Scope.Everyone);
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        base.OnConnectedToServer(sandbox, server);

        Instantiate(_ui, Vector3.zero, Quaternion.identity, transform).GetComponent<Chat>()._sandbox = Sandbox;
    }

    public override void OnDisconnectedFromServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        Netick.Network.Shutdown();
    }

    public override void OnShutdown(NetworkSandbox sandbox)
    {
        base.OnShutdown(sandbox);

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
