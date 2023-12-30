using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ChatSystem;

public class TestSendMessage : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _button;
    [HideInInspector] public ChatSystemManager _chatSystemManager;

    public void Send()
    {
        SendMessageManager.Client.SendMessageToTeam(_chatSystemManager.ClientId, _chatSystemManager._player._group._groupId, _chatSystemManager._player._group._groupId, _inputField.text, _chatSystemManager.ServerConnection);
        _inputField.text = "";
    }
}
