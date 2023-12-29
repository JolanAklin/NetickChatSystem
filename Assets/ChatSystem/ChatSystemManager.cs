using ChatSystem;
using Netick.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public class ChatSystemManager : MonoBehaviour
    {
        [SerializeField] private Config _chatSystemConfig;

        public ChatTransportConnection ServerConnection { get; set; }
        public bool IsClient { get; set; }
        public int SandboxId { get; set; }

        public static Config ChatSystemConfig { get; private set; }


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

