using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick.Unity;
using Netick;
using ChatSystem;
using System.Linq;

public class ChatEventListener : NetworkEvents
{
    private ConnectionManager _connectionManager;
    [SerializeField]
    private List<NetickBehaviour> _registeredBehaviours;

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
    }

    public override void OnConnectedToServer(NetworkSandbox sandbox, NetworkConnection server)
    {
        _connectionManager.ServerConnection = (IChatTransportConnection)server.TransportConnection;
    }
}
