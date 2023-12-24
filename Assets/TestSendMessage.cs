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

    public void Send()
    {
        ReceivedMessageManager.OnMessagedReceived("test", _inputField.text, 0);
        _inputField.text = "";
    }
}
