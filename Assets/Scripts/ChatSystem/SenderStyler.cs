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

            ///<summary>
            ///Will be null if _isClient is false.
            ///</sumamry>
            public int? _clientId { get; private set; }

            ///<summary>
            /// The scope the message has been sent to. Will be null if _isClient is false.
            ///</summary>
            public Scope _target {get; private set;}

            ///<summary>
            /// The scope of the sender
            ///</summary>
            public Scope _sender { get; private set; }

            ///<summary>
            /// If the message was sent by someone outside the target scope. Will be null if _isClient is false.
            ///</summary>
            public bool? _isForeignSend {get; private set;}

            public StylerData(int clientId, Scope target, Scope sender, bool foreignSend)
            {
                _isClient = true;
                _clientId = clientId;
                _target = target;
                _sender = sender;
                _isForeignSend = foreignSend;
            }

            ///<summary>
            /// When sent by the server. _isClient is set to false
            ///</summary>
            public StylerData(Scope target)
            {
                _isClient = false;
                _clientId = null;
                _target = target;
                _sender = null;
                _isForeignSend = null;
            }
            ///<summary>
            /// When sent by the server. _isClient is set to false
            ///</summary>
            public StylerData()
            {
                _isClient = false;
                _clientId = null;
                _target = null;
                _sender = null;
                _isForeignSend = null;
            }
        }
        public abstract string GetSenderStyle(StylerData data);
        
        public abstract StylerData GenerateData();
        public abstract StylerData GenerateData(Scope target);
        public abstract StylerData GenerateData(int clientId, Scope target, Scope sender, bool foreignSend);
    }
}