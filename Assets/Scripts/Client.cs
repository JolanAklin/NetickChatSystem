using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using TMPro;

public class Client : NetworkBehaviour
{
    public enum ScopeEnum
    {
        Everyone,
        RedTeam,
        BlueTeam,
        BothTeam,
    }

    [SerializeField] private GameObject _ui;
    private ChatMessenger _messenger;
    private ScopeManager _manager;
    private ScopeManager.Scope _targetScope;
    public NetworkConnection _netConnection;
    public override void NetworkStart()
    {
        _messenger = Sandbox.FindGameObjectWithTag("NetController").GetComponent<ChatMessenger>();
        _manager = _messenger.GetComponent<ScopeManager>();
        _targetScope = ScopeManager.Scope.Everyone;
        if(IsClient && IsInputSource && !IsOwner)
            Instantiate(_ui, Vector3.zero, Quaternion.identity, transform).GetComponent<Chat>()._sandbox = Sandbox;
    }

    [Rpc(source: RpcPeers.InputSource, target: RpcPeers.Owner, isReliable: true, localInvoke: false)]
    public void RPC_ChangeScope(ScopeEnum wantedScope)
    {
        if(!IsOwner)
            return;
        ScopeManager.Scope scope = _messenger.GetScope(Sandbox.RpcSource);
        scope.Clear();
        switch (wantedScope)
        {
            case ScopeEnum.RedTeam:
                scope.AddScope(_manager.GetScope("Red team"));
                break;
            case ScopeEnum.BlueTeam:
                scope.AddScope(_manager.GetScope("Blue team"));
                break;
            case ScopeEnum.BothTeam:
                scope.AddScope(_manager.GetScope("Teams"));
                break;
        }
    }

    public void ChangeScope(ScopeEnum scope)
    {
        switch (scope)
        {
            case ScopeEnum.Everyone:
                _targetScope = ScopeManager.Scope.Everyone;
                break;
            case ScopeEnum.RedTeam:
                _targetScope = _manager.GetScope("Red team");
                break;
            case ScopeEnum.BlueTeam:
                _targetScope = _manager.GetScope("Blue team");
                break;
            case ScopeEnum.BothTeam:
                _targetScope = _manager.GetScope("Teams");
                break;
        }
    }

    public void SendChatMessage(string message)
    {
        _messenger.SendToServer(message, _targetScope);
        message = "";
    }
}
