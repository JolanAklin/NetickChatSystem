/*


MIT License

Copyright (c) 2022 Jolan Aklin

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;
using NetickChatSystem;

namespace NetickChatSystem
{
    public class ChatSystem : MonoBehaviour
    {
        public static ChatSystem Instance {get; private set;}
        private Dictionary<NetworkSandbox, ChatMessenger> _messengers = new Dictionary<NetworkSandbox, ChatMessenger>();
        private void Awake() {
            if(Instance == null)
            {
                Instance = this;
            }else
            {
                Debug.LogWarning("There is more than one ChatSystem. Destroying");
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if(Instance == this)
                Instance = null;
        }

        // internal use only
        public void AddChatMessenger(NetworkSandbox sandbox, ChatMessenger messenger)
        {
            if(_messengers.ContainsKey(sandbox))
            {
                Debug.Log($"There is already a chatmessenger registred for this sandbox ({sandbox.name}). Only one ChatMessenger per sandbox is supported.");
            }
            else
            {
                _messengers.Add(sandbox, messenger);
            }
        }

        // internal use only
        public void RemoveChatMessenger(NetworkSandbox sandbox)
        {
            if (_messengers.ContainsKey(sandbox))
            {
                _messengers.Remove(sandbox);
            }
            else
            {
                Debug.Log($"No ChatSystem for this sandbox ({sandbox.name})");
            }
        }

        ///<summary>
        ///Get the ChatMessenger in use for the given sandbox
        ///</summary>
        public ChatMessenger GetChatMessenger(NetworkSandbox sandbox)
        {
            if(_messengers.TryGetValue(sandbox, out ChatMessenger messenger))
                return messenger;
            return null;
        }
    }
}
