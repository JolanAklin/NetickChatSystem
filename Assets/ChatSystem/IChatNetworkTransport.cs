using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChatNetworkTransport
{
    public event Action<byte[], int> ChatMessageReceived;

    public void MessageReceived(byte[] data, int sendersId);
}
