using ChatSystem;
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

        private void OnChatMessageReceived(byte[] data)
        {
            Debug.Log("message received : " + System.Text.Encoding.UTF8.GetString(data));
        }
    }

}
