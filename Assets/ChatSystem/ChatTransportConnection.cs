using Netick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public abstract class ChatTransportConnection : TransportConnection
    {
        public abstract void ChatSend(byte[] data);
    }
}
