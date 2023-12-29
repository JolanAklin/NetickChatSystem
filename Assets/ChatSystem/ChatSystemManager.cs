using ChatSystem;
using Netick.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ChatSystem
{
    public class ChatSystemManager : MonoBehaviour
    {
        [SerializeField] private Config _chatSystemConfig;

        public ChatTransportConnection ServerConnection { get; set; }
        public bool IsClient { get; set; }
        public int SandboxId { get; set; }
        private int? _clientId = null;
        public int ClientId { get 
            { 
                if (_clientId == null)
                { 
                    if(_sandbox == null)
                    {
                        Debug.LogError("Netick is not starter. Consider starting Netick first");
                        return -1;
                    }
                    _clientId = _sandbox.LocalPlayer.PlayerId; 
                }
                return _clientId.Value;
            } 
        }
        [HideInInspector]
        public NetworkSandbox _sandbox;

        // list of all connected players. Only filled on the server
        public static Dictionary<int, ChatPlayer> _connectedPlayer = new Dictionary<int, ChatPlayer>();

        public static Config ChatSystemConfig { get; private set; }

        public Group _currentGroup;

        public ChatPlayer _player;


        private void OnValidate()
        {
            ChatSystemConfig = _chatSystemConfig;
        }

        private void Awake()
        {
            IsClient = true;
            if(ChatSystemConfig == null)
            {
                ChatSystemConfig = _chatSystemConfig;
            }
        }
    }
}

