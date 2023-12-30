using ChatSystem;
using Netick.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using UnityEditor.Compilation;

namespace ChatSystem
{
    public abstract class ChatPlayer : NetworkBehaviour
    {
        //[HideInInspector]
        public ChatSystemManager _chatSystemManager;

        [Networked]
        public string PlayerName { get; set; }

        public Group _group;

        public ChatTransportConnection Connection { get; set; }
    }
}
