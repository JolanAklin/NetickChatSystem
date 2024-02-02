using Netick.Unity;
using Netick;
using ChatSystem;
using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

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

    private TeamManager _teamManager;

    private Dictionary<string, byte> _kvpForDropdown = new Dictionary<string, byte>();

    public override void NetworkStart()
    {
        _teamManager = Sandbox.GetComponent<TeamManager>();
        List<string> options = new List<string>
        {
            "none"
        };
        _kvpForDropdown.Clear();
        foreach (Team team in _teamManager.GetTeams())
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
        //Debug.Log(playerName.ToString() + " lorem ipsum dolore sit amat..."); // prints : my_player_name lorem ipsum dolore sit amat...
        //Debug.Log(PlayerName.ToString() + " lorem ipsum dolore sit amat..."); // prints : my_player_name
        //Debug.Log(PlayerName + " lorem ipsum dolore sit amat..."); // prints : my_player_name
        //Debug.Log(PlayerName.Length);
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
        Team team = _teamManager.GetTeam(teamID);
        if (team != null)
            team.AddPlayer(this);
    }

    public string Decorator()
    {
        Team team = _teamManager.GetTeam(TeamID);
        string color= "";
        if (team != null)
            color = ColorUtility.ToHtmlStringRGB(team.Color);
        return "<color=#ffb700>[ <b>playername</b> ]</color> " + (TeamID == 0 ? "general" : "<color=#"+color+">team " + team.Name + "</color>") + " : ";
    }
}
