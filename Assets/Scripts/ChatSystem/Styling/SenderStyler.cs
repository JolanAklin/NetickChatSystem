using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netick;

public abstract class SenderStyler : ScriptableObject
{
    public class StylerData
    {
        
    }
    public abstract string GetSenderStyle(StylerData data);
    
}
