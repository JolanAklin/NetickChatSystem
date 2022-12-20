/*


MIT License

Copyright (c) 2022 Jolan Aklin

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/

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

    public override StylerData GenerateData(int senderId, Scope target, Scope sender, bool foreignSend)
    {
        return new StylerData(senderId, target, sender, foreignSend);
    }

    public override StylerData GenerateData(int senderId, int clientTarget, Scope sender)
    {
        return new StylerData(senderId, clientTarget, sender);
    }
    #endregion

    public override string GetSenderStyle(StylerData data)
    {
        if(data._isScopeSend)
        {
            if (data._isClient)
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
                return $"|{data._target.name.ToUpper()}| <color=#{ColorUtility.ToHtmlStringRGB(color)}>[CLIENT {data._senderId}]</color> ";
            }
            else
            {
                if (data._target != null)
                {
                    return $"|{data._target.name.ToUpper()}| <color=#{ColorUtility.ToHtmlStringRGB(_serverColor)}>[SERVER] > </color>";
                }
                return $"<color=#{ColorUtility.ToHtmlStringRGB(_serverColor)}>[SERVER] > </color>";
            }
        }else
        {
            if (data._isClient)
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
                return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[CLIENT {data._senderId}]</color> -> You : ";
            }
            else
            {
                if (data._clientTarget != null)
                {
                    return $"<color=#{ColorUtility.ToHtmlStringRGB(_serverColor)}>[SERVER]</color> -> You : ";
                }
                return $"<color=#{ColorUtility.ToHtmlStringRGB(_serverColor)}>[SERVER] > </color>";
            }
        }
    }
}