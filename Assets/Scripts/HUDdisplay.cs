using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetickChatSystem;
using TMPro;

public class HUDdisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _hudText;
    [SerializeField] private float _hudTime = 4f;
    private float _elapsedTime = 0;
    private void Start()
    {
        ChatSystem.Instance.GetChatMessenger(GetComponent<Chat>()._sandbox)._chatDisplay.AddDisplay(Displays.hud, (string message) => { OnMessageReceived(message); });
    }
    public void OnMessageReceived(string message)
    {
        _hudText.text = message;
        _elapsedTime = 0;
    }

    private void Update()
    {
        if(_elapsedTime >= _hudTime)
        {
            _hudText.text = "";
        }
        _elapsedTime += Time.deltaTime;
    }
}
