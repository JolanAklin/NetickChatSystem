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

namespace NetickChatSystem
{
    public abstract class SenderStyler : ScriptableObject
    {
        public class StylerData
        {
            public bool _isClient { get; private set; }
            
            public bool _isScopeSend {get; private set;}

            ///<summary>
            ///Will be null if _isClient is false.
            ///</sumamry>
            public int? _senderId { get; private set; }

            ///<summary>
            /// The scope the message has been sent to. Will be null if _isClient is false.
            ///</summary>
            public Scope _target {get; private set;}

            ///<summary>
            /// The client the message has been sent to. Will be null if _isScopeSend is true.
            ///</summary>
            public int? _clientTarget { get; private set; }

            ///<summary>
            /// The scope of the sender
            ///</summary>
            public Scope _sender { get; private set; }

            ///<summary>
            /// If the message was sent by someone outside the target scope. Will be null if _isClient is false.
            ///</summary>
            public bool? _isForeignSend {get; private set;}

            public StylerData(int senderId, Scope target, Scope sender, bool foreignSend)
            {
                _isClient = true;
                _senderId = senderId;
                _target = target;
                _sender = sender;
                _isForeignSend = foreignSend;
                _isScopeSend = true;
                _clientTarget = null;
            }

            public StylerData(int senderId, int targetClientId, Scope sender)
            {
                _isClient = true;
                _senderId = senderId;
                _clientTarget = targetClientId;
                _sender = sender;
                _isForeignSend = false;
                _isScopeSend = false;
            }

            ///<summary>
            /// When sent by the server. _isClient is set to false
            ///</summary>
            public StylerData(Scope target)
            {
                _isClient = false;
                _senderId = null;
                _target = target;
                _sender = null;
                _isForeignSend = null;
                _isScopeSend = true;
                _clientTarget = null;
            }
            ///<summary>
            /// When sent by the server. _isClient is set to false
            ///</summary>
            public StylerData()
            {
                _isClient = false;
                _senderId = null;
                _target = null;
                _sender = null;
                _isForeignSend = null;
                _isScopeSend = false;
                _clientTarget = null;
            }
        }

        private System.Text.UTF8Encoding _encoding = new System.Text.UTF8Encoding();
        
        public string GetSenderStyle(StylerData data)
        {
            string senderStyle = GenerateSenderStyle(data);
            if(_encoding.GetByteCount(senderStyle) > 300)
            {
                Debug.LogError("GenerateSenderStyle() should not return a string longer than 300 char");
                return null;
            }
            return senderStyle;
        }
        public abstract string GenerateSenderStyle(StylerData data);
        
        public abstract StylerData GenerateData();
        public abstract StylerData GenerateData(Scope target);
        public abstract StylerData GenerateData(int senderId, Scope target, Scope sender, bool foreignSend);
        public abstract StylerData GenerateData(int senderId, int clientTarget, Scope sender);
    }
}