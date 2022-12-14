using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;

public abstract class SenderStyler : ScriptableObject
{
    public class StylerData
    {
        ///<summary>
        /// The scope the message has been sent to
        ///</summary>
        public ScopeManager.Scope scope;
        ///<summary>
        /// If the message was sent by someone outside the target scope
        ///</summary>
        public bool isForeignSend;
    }
    public abstract string GetSenderStyle(StylerData data);
    
}
