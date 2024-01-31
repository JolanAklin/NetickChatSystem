using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;

namespace ChatSystem
{
    public interface IChatTransportConnection
    {
        public void ChatSend(byte[] data);
    }
}
