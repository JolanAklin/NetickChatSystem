using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using NetickChatSystem;

public class EventHandler : ChatNetworkEventsListner
{
    private ChatMessenger _chat;
    [SerializeField] private GameObject _client;
    [SerializeField] private GameObject _ui;

    private static bool _settedUp = false; // prevent setting the same scope two or more times

    private void Start()
    {
        _chat = GetComponent<ChatMessenger>();

        if(_settedUp)
            return;
        ScopeManager.Instance.RegisterScope("Red team");
        ScopeManager.Instance.RegisterScope("Blue team");
        ScopeManager.Instance.RegisterExtendedScope("Teams", new string[] {"Red team", "Blue team"}, Scope.CheckPolicy.loose, Scope.ForeignReceivePolicy.forbidden);
        _settedUp = true;
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        base.OnClientConnected(sandbox, client);

        NetworkObject obj = sandbox.NetworkInstantiate(_client, Vector3.zero, Quaternion.identity, client);
        client.PlayerObject = obj.gameObject;
        obj.GetComponent<Client>()._netConnection = client;

        _chat.SendChatMessageToOne($"Welcome to the server Client {client.Id}", client);
        _chat.SendChatMessageToScope($"Client {client.Id} connected", Scope.Everyone);
    }

    public override void OnClientDisconnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        base.OnClientDisconnected(sandbox, client);
        _chat.SendChatMessageToScope($"Client {client.Id} disconnected", Scope.Everyone);
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
