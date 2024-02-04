using ChatSystem;
using Netick.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SendTestMessage : NetickBehaviour
{
    private TMP_InputField _inputField;

    private ChatSystemManager _chatSystem;

    private MessageSender.Destination _destination;

    private List<PlayerController> _players = new List<PlayerController> ();

    [SerializeField]
    private TMP_Dropdown _playerSelectionDropdown;

    public override void NetworkStart()
    {
        _inputField = GetComponent<TMP_InputField>();
        _chatSystem = Sandbox.GetComponent<ChatSystemManager>();
        _destination = MessageSender.Destination.general;
    }

    public void OnDestinationChanged(TMP_Dropdown dropdown)
    {
        _destination = (MessageSender.Destination)dropdown.value;
        if(_destination == MessageSender.Destination.player)
        {
            List<string> dropDownOptions = new List<string>();
            int i = 0;
            foreach (PlayerController controller in Sandbox.FindObjectsOfType<PlayerController>())
            {
                dropDownOptions.Add(((IChatPlayer)controller).PlayerName);
                _players.Add(controller);
            }
            _playerSelectionDropdown.ClearOptions();
            _playerSelectionDropdown.AddOptions(dropDownOptions);
        }
    }

    public void OnEndEdit()
    {
        if(_destination == MessageSender.Destination.player)
        _chatSystem.MessageSender.SendMessageToServer(_chatSystem.ConnectionManager.ServerConnection, _inputField.text, _destination, _players[_playerSelectionDropdown.value].InputSourcePlayerId + 1); // need to ask Karrar about this
        else
        _chatSystem.MessageSender.SendMessageToServer(_chatSystem.ConnectionManager.ServerConnection, _inputField.text, _destination);
            _inputField.text = "";
        _inputField.Select();
    }
}
