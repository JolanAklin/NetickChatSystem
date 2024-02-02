using Netick.Unity;
using Netick;
using ChatSystem;
using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerController : NetworkBehaviour, IChatPlayer
{
    [Networked]
    public string PlayerName { get; set; }

    [SerializeField]
    private GameObject _setNameUi;
    private GameObject _chatUi;

    public override void NetworkStart()
    {
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

    [Rpc(source: RpcPeers.Everyone, target: RpcPeers.Owner, isReliable: true, localInvoke: true)]
    public void RPCSetPlayerName(NetworkString256 playerName)
    {
        PlayerName = playerName.ToString();
    }
}
