using System.Collections;
using System.Collections.Generic;
using Netick;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultStyler", menuName = "ChatSystem/DefaultStyler", order = 0)]
public class DefaultStyle : SenderStyler
{
    [SerializeField] private Color _client0 = Color.green;
    [SerializeField] private Color _otherClient = new Color(1,.57f,0);
    [SerializeField] private Color _serverColor = Color.red;

    // If you want more data than what is available by default, this is where you'll get more.
    #region generate styler data
    public override StylerData GenerateData()
    {
        return new StylerData();
    }

    public override StylerData GenerateData(ScopeManager.Scope target)
    {
        return new StylerData(target);
    }

    public override StylerData GenerateData(int clientId, ScopeManager.Scope target, ScopeManager.Scope sender, bool foreignSend)
    {
        return new StylerData(clientId, target, sender, foreignSend);
    }
    #endregion

    public override string GetSenderStyle(StylerData data)
    {
        if(data._isClient)
        {
            return $"|{data._target.name.ToUpper()}| <color=#{(data._clientId == 0 ? ColorUtility.ToHtmlStringRGB(_client0) : ColorUtility.ToHtmlStringRGB(_otherClient))}>[CLIENT {data._clientId}]</color> ";
        }
        else
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(_serverColor)}>[SERVER] > </color>";
        }
    }
}
