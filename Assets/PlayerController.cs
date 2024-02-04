using Netick.Unity;
using Netick;
using ChatSystem;
using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.Drawing;

public class PlayerController : NetworkBehaviour, IChatPlayer
{
    [Networked(256)]
    public string PlayerName { get; set; }
    [Networked]
    public byte TeamID { get; set; }
    public IChatTransportConnection Connection { get; set; }

    [SerializeField]
    private GameObject _setNameUi;
    private GameObject _chatUi;

    [SerializeField]
    private TMP_Dropdown _teamDropdown;

    private ChatSystemManager _chatSystem;

    private Dictionary<string, byte> _kvpForDropdown = new Dictionary<string, byte>();

    public override void NetworkStart()
    {
        _chatSystem = Sandbox.GetComponent<ChatSystemManager>();
        List<string> options = new List<string>
        {
            "none"
        };
        _kvpForDropdown.Clear();
        foreach (Team team in _chatSystem.TeamManager.GetTeams())
        {
            options.Add(team.Name);
            _kvpForDropdown.Add(team.Name, team.ID);
        }
        _teamDropdown.ClearOptions();
        _teamDropdown.AddOptions(options);
        _teamDropdown.onValueChanged.AddListener(OnTeamDropdownChanged);

        _chatUi = Sandbox.FindGameObjectWithTag("ChatUI");
        if(IsInputSource && Sandbox.IsClient)
        {
            _setNameUi.SetActive(true);
            _chatUi.SetActive(false);
        }
        else
        {
            _setNameUi.SetActive(false);
            _chatUi.SetActive(false);
        }
    }

    [OnChanged(nameof(PlayerName))]
    public void OnPlayerNameChanged(OnChangedData onChangedData)
    {
        if (!IsInputSource) return;
        _chatUi.GetComponentInChildren<InfosPanel>().PlayerNameText.text = PlayerName;
    }

    [OnChanged(nameof(TeamID))]
    public void OnTeamIDChanged(OnChangedData onChangedData)
    {
        if (!IsInputSource) return;
        Team team = _chatSystem.TeamManager.GetTeam(TeamID);
        if(team != null)
        {
            string color = ColorUtility.ToHtmlStringRGB(team.Color);
            _chatUi.GetComponentInChildren<InfosPanel>().TeamText.text = "<color=#" + color + ">" + team.Name + "</color>";
        }
        else
        {
            _chatUi.GetComponentInChildren<InfosPanel>().TeamText.text = "none";
        }

    }


    public void SetPlayerName(TMP_InputField inputField)
    {
        RPCSetPlayerName(new NetworkString256(inputField.text));
        _setNameUi.SetActive(false);
        _chatUi.SetActive(true);
    }

    [Rpc(source: RpcPeers.Everyone, target: RpcPeers.Owner, isReliable: true, localInvoke: false)]
    public void RPCSetPlayerName(NetworkString256 playerName)
    {
        PlayerName = playerName.ToString();
    }

    public void OnTeamDropdownChanged(int value)
    {
        byte teamID = 0;
        if (_kvpForDropdown.ContainsKey(_teamDropdown.options[value].text))
            teamID = _kvpForDropdown[_teamDropdown.options[value].text];
        RPCChangeTeam(teamID);
    }

    [Rpc(source: RpcPeers.Everyone, target: RpcPeers.Owner, isReliable: true, localInvoke: false)]
    public void RPCChangeTeam(byte teamID)
    {
        Team team = _chatSystem.TeamManager.GetTeam(teamID);
        if (team != null)
            team.AddPlayer(this);
    }

    public string Decorator(MessageSender.Destination destination)
    {
        Team team = _chatSystem.TeamManager.GetTeam(TeamID);
        string color= "";
        if (team != null)
            color = ColorUtility.ToHtmlStringRGB(team.Color);

        string decorator = "<color=#ffb700>[ <b>" + PlayerName + "</b> ]</color> " + (TeamID == 0 ? "general" : "<color=#"+color+">team " + team.Name + "</color>") + " : ";
        if(destination == MessageSender.Destination.general)
        {
            decorator = "[GENERAL] " + decorator;
        }
        return decorator;
    }
}
