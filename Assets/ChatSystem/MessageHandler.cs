using ChatSystem;
using LiteNetLib.Utils;
using Netick.Samples;
using Netick.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

namespace ChatSystem
{
    public class MessageHandler : MonoBehaviour
    {
        private NetDataReader _netDataReader = new NetDataReader();
        LNLTransportProviderWchat _transport; // TODO change this stuff so it doesnt depend on LNLTransportProviderWChat
        private void Start()
        {
            NetworkSandbox sandbox = GetComponent<NetworkSandbox>();
            _transport = (LNLTransportProviderWchat)sandbox.Transport;
            _transport.ChatMessageReceived += OnChatMessageReceived;
        }

        private void OnDestroy()
        {
            _transport.ChatMessageReceived -= OnChatMessageReceived;
        }

        private void OnChatMessageReceived(byte[] data, int id)
        {
            _netDataReader.SetSource(data);
            string message = _netDataReader.GetString();
            Debug.Log("message received from "+id+" : " + message);
        }
    }

}
