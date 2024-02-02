using Netick;
using System.Collections;
using UnityEngine;

namespace ChatSystem
{
    public interface IChatPlayer
    {
        [Networked]
        public string PlayerName { get; set; }

        [Rpc(source: RpcPeers.Everyone, target: RpcPeers.Owner, isReliable: true, localInvoke: true)]
        public void RPCSetPlayerName(NetworkString256 playerName);
    }
}