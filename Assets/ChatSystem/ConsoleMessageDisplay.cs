using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleMessageDisplay : AbstractMessageDisplay
{
    public override void Display(string message)
    {
        Debug.Log("Message displayed from the ConsoleMessageDisplay : "+message);
    }
}
