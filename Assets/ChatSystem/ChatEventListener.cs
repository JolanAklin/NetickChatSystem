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
    private ChatSystemManager _chatSystem;
    [SerializeField]
    private List<NetickBehaviour> _registeredBehaviours;

    [SerializeField]
    private GameObject PlayerPrefab;
    [SerializeField]
    private bool SpawnPlayerForHost = false;

    public override void OnStartup(NetworkSandbox sandbox)
    {
        _chatSystem = Sandbox.GetComponent<ChatSystemManager>();
        foreach (NetickBehaviour behaviour in _registeredBehaviours)
        {
            Sandbox.AttachBehaviour(behaviour);
        }
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
        {
            _chatSystem.ConnectionManager.ClientConnections.Add(client.PlayerId, new ConnectionManager.ClientConnectionInfos((IChatTransportConnection)client.TransportConnection, chatPlayer));
            chatPlayer.Connection = (IChatTransportConnection)client.TransportConnection;
        }
        else
            Debug.LogError("The player must implement IChatPlayer");
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        _chatSystem.ConnectionManager.ServerConnection = (IChatTransportConnection)server.TransportConnection;
    }
}
