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

        public static Config ChatSystemConfig { get; private set; }

        public int SandboxId;

        private void OnValidate()
        {
            ChatSystemConfig = _chatSystemConfig;
        }

        private void Awake()
        {
            if(ChatSystemConfig == null)
            {
                ChatSystemConfig = _chatSystemConfig;
            }
        }
    }
}

