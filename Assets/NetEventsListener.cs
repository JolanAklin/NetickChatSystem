using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using Netick.Unity;
using ChatSystem;

public class NetEventsListener : NetworkEventsListener
{
    [SerializeField] private ChatSystemManager _chatSystemManager;
    public override void OnStartup(NetworkSandbox sandbox)
    {
        _chatSystemManager.SandboxId = sandbox.GetInstanceID();
    }
}
