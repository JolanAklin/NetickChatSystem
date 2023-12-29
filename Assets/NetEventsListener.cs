using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Unity;
using ChatSystem;

public class NetEventsListener : NetworkEventsListener
{
    [SerializeField] private ChatSystemManager _chatSystemManager;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _playerSpawnPos;
    public override void OnStartup(NetworkSandbox sandbox)
    {
        _chatSystemManager.SandboxId = sandbox.GetInstanceID();
        _chatSystemManager.IsClient = sandbox.IsClient;
        _chatSystemManager._sandbox = sandbox;
    }

    public override void OnClientConnected(NetworkSandbox sandbox, NetworkConnection client)
    {
        var spawnPos = _playerSpawnPos.position + Vector3.left * (1 + sandbox.ConnectedPlayers.Count);
        GameObject playerObject = sandbox.NetworkInstantiate(_playerPrefab, spawnPos, Quaternion.identity, client).gameObject;
        client.PlayerObject = playerObject;
        ChatPlayer player = playerObject.GetComponent<ChatPlayer>();

        player.PlayerName = "player " + client.PlayerId.ToString();
        Debug.Log("player " + client.PlayerId.ToString());
        Debug.Log("set player name to " + player.PlayerName);


        ChatSystemManager._connectedPlayer.Add(client.PlayerId, player);
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        _chatSystemManager.ServerConnection = (ChatTransportConnection)server.TransportConnection;
    }

    // could use a dictionnary later. Karrar will fix the sandbox.localplayer.playerid returning 0 in the OnConnectedToServer
    public override void OnObjectCreated(NetworkObject entity)
    {
        if(entity.gameObject.TryGetComponent(out ChatPlayer player))
        {
            player._chatSystemManager = _chatSystemManager;
        }
    }
}
