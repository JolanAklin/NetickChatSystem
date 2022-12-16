using System.Collections;
using System.Collections.Generic;
using Netick;
using UnityEngine;
using NetickChatSystem;

[CreateAssetMenu(fileName = "DefaultStyler", menuName = "ChatSystem/DefaultStyler", order = 0)]
public class DefaultStyle : SenderStyler
{
    [SerializeField] private Color _redTeamColor = Color.red;
    [SerializeField] private Color _blueTeamColor = Color.blue;
    [SerializeField] private Color _teamsColor = Color.cyan;
    [SerializeField] private Color _everyoneColor = Color.gray;
    [SerializeField] private Color _serverColor = Color.magenta;

    // If you want more data than what is available by default, this is where you'll get more.
    #region generate styler data
    public override StylerData GenerateData()
    {
        return new StylerData();
    }

    public override StylerData GenerateData(Scope target)
    {
        return new StylerData(target);
    }

    public override StylerData GenerateData(int clientId, Scope target, Scope sender, bool foreignSend)
    {
        return new StylerData(clientId, target, sender, foreignSend);
    }
    #endregion

    public override string GetSenderStyle(StylerData data)
    {
        if(data._isClient)
        {
            Color color = Color.black;
            switch (data._sender.name)
            {
                case "Everyone":
                    color = _everyoneColor;
                    break;
                case "Blue team":
                    color = _blueTeamColor;
                    break;
                case "Red team":
                    color = _redTeamColor;
                    break;
                case "Teams":
                    color = _teamsColor;
                    break;
            }
            return $"|{data._target.name.ToUpper()}| <color=#{ColorUtility.ToHtmlStringRGB(color)}>[CLIENT {data._clientId}]</color> ";
        }
        else
        {
            if(data._target != null)
            {
                return $"|{ data._target.name.ToUpper()}| <color=#{ColorUtility.ToHtmlStringRGB(_serverColor)}>[SERVER] > </color>";
            }
            return $"<color=#{ColorUtility.ToHtmlStringRGB(_serverColor)}>[SERVER] > </color>";
        }
    }
}