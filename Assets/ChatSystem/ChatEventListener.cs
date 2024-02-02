using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick.Unity;
using Netick;
using ChatSystem;
using System.Linq;
using Netick.Samples.FPS;

public class ChatEventListener : NetworkEvents
{
    private ConnectionManager _connectionManager;
    [SerializeField]
    private List<NetickBehaviour> _registeredBehaviours;

    [SerializeField]
    private GameObject PlayerPrefab;
    [SerializeField]
    private bool SpawnPlayerForHost = false;

    public override void OnStartup(NetworkSandbox sandbox)
    {
        foreach (NetickBehaviour behaviour in _registeredBehaviours)
        {
            Sandbox.AttachBehaviour(behaviour);
        }
        _connectionManager = Sandbox.GetComponent<ConnectionManager>();
    }

    public override void OnShutdown(NetworkSandbox sandbox)
    {
        foreach (NetickBehaviour behaviour in _registeredBehaviours)
        {
            Sandbox.DeattachBehaviour(behaviour);
        }
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {

        GameObject player = sandbox.NetworkInstantiate(PlayerPrefab, Vector3.zero, Quaternion.identity, client).gameObject;
        client.PlayerObject = player;
        player.name = "player " + (sandbox.ConnectedPlayers.Count - 1);

        if (player.TryGetComponent(out IChatPlayer chatPlayer))
            _connectionManager.ClientConnections.Add(client.PlayerId, new ConnectionManager.ClientConnectionInfos((IChatTransportConnection)client.TransportConnection, chatPlayer));
        else
            Debug.LogError("The player must implement IChatPlayer");
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        _connectionManager.ServerConnection = (IChatTransportConnection)server.TransportConnection;
    }
}
