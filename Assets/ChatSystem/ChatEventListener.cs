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
        _connectionManager.ClientConnections.Add(client.Id, (IChatTransportConnection)client.TransportConnection);

        GameObject player = sandbox.NetworkInstantiate(PlayerPrefab, Vector3.zero, Quaternion.identity, client).gameObject;
        client.PlayerObject = player;
        player.name = "player " + (sandbox.ConnectedPlayers.Count - 1);
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        _connectionManager.ServerConnection = (IChatTransportConnection)server.TransportConnection;
    }

    public override void OnSceneLoaded(NetworkSandbox sandbox)
    {
        if (sandbox.IsClient)
            return;

        for (int i = 0; i < sandbox.ConnectedPlayers.Count; i++)
        {
            // if SpawnPlayerForHost is set to false, we don't spawn a player for the server
            // index zero is the server player

            if (!SpawnPlayerForHost && i == 0)
                continue;

            var p = sandbox.ConnectedPlayers[i];

            var spawnPos = Vector3.zero;
            GameObject player = sandbox.NetworkInstantiate(PlayerPrefab, spawnPos, Quaternion.identity, p).gameObject;
            p.PlayerObject = player;
            player.name = "player " + (sandbox.ConnectedPlayers.Count - 1);
        }
    }
}
