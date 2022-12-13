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

    public class DefaultStylerData : StylerData
    {
        public bool _isClient {get; private set;}
        public int? _clientId { get; private set; }

        public DefaultStylerData(bool isClient, int? clientId = null)
        {
            _isClient = isClient;
            _clientId = clientId;
        }
    }

    public override string GetSenderStyle(StylerData data)
    {
        DefaultStylerData dsd = (DefaultStylerData)data;
        if(dsd._isClient)
        {
            
            return $"<color=#{(dsd._clientId == 0 ? ColorUtility.ToHtmlStringRGB(_client0) : ColorUtility.ToHtmlStringRGB(_otherClient))}>[CLIENT {dsd._clientId}]</color> ";
        }
        else
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(_serverColor)}>[SERVER] > </color>";
        }
    }
}
