using UnityEngine;

[DisallowMultipleComponent]
public abstract class AbstractMessageDisplay : MonoBehaviour
{
    public abstract void Display(string message);
}
