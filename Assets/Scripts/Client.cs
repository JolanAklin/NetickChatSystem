using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using TMPro;
using NetickChatSystem;

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
    private Scope _targetScope;
    public NetworkConnection _netConnection;

    private GameObject _chat;
    public override void NetworkStart()
    {
        _messenger = Sandbox.FindGameObjectWithTag("NetController").GetComponent<ChatMessenger>();
        _manager = _messenger.GetComponent<ScopeManager>();
        _targetScope = Scope.Everyone;
        if(IsClient && IsInputSource && !IsOwner)
        {
            _chat = Sandbox.FindGameObjectWithTag("Chat");
            _chat.GetComponent<Chat>()._client = this;
        }
    }

    public override void OnInputSourceLeft()
    {
        Sandbox.Destroy(this.Object);
    }

    public override void NetworkDestroy()
    {
        Destroy(_chat);
    }

    private void OnDestroy()
    {
        Destroy(_chat);
    }

    [Rpc(source: RpcPeers.InputSource, target: RpcPeers.Owner, isReliable: true, localInvoke: false)]
    public void RPC_ChangeScope(ScopeEnum wantedScope)
    {
        if(!IsOwner)
            return;
        Scope scope = _messenger.GetScope(Sandbox.RpcSource);
        scope.Clear();
        scope.ChangeName("Everyone");
        switch (wantedScope)
        {
            case ScopeEnum.RedTeam:
                scope.AddScope(_manager.GetScope("Red team"));
                scope.ChangeName("Red team");
                break;
            case ScopeEnum.BlueTeam:
                scope.AddScope(_manager.GetScope("Blue team"));
                scope.ChangeName("Blue team");
                break;
            case ScopeEnum.BothTeam:
                scope.AddScope(_manager.GetScope("Teams"));
                scope.ChangeName("Teams");
                break;
        }
    }

    public void ChangeScope(ScopeEnum scope)
    {
        switch (scope)
        {
            case ScopeEnum.Everyone:
                _targetScope = Scope.Everyone;
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
