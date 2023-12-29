using ChatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ChatPlayer
{
    [SerializeField] private TestSendMessage _testSendMessage;
    

    public override void NetworkStart()
    {
        if(IsInputSource)
        {
            _testSendMessage._chatSystemManager = _chatSystemManager;
            _chatSystemManager._player = this;
        }
        else
        {
            Destroy(_testSendMessage.gameObject);
        }
    }
}
