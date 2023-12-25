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
    [SerializeField] private ChatSystemManager _chatSystemManager;

    public void Send()
    {
        ReceivedMessageManager.OnMessagedReceived(_chatSystemManager.SandboxId, "test", _inputField.text, 0);
        _inputField.text = "";
    }
}
