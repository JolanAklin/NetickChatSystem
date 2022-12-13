using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChatMasks", menuName = "ChatSystem/ChatMasks", order = 0)]
public class ChatMasks : ScriptableObject
{
    [SerializeField] private string[] _masksName = new string[32];

    private Dictionary<string, uint> _nameToMask = new Dictionary<string, uint>();

    private void OnValidate() {
        _nameToMask.Clear();
        for (int i = 0; i < _masksName.Length; i++)
        {
            if(!_nameToMask.ContainsKey(_masksName[i]) && _masksName[i] != "" && _masksName[i] != null)
                _nameToMask.Add(_masksName[i], ((uint)1)<<i);
        }
    }

    public bool GetMask(string name, out uint mask)
    {

        bool found = _nameToMask.TryGetValue(name, out mask);
        if(!found)
            Debug.LogError($"Mask \"{name}\" doesn't exist");
        return found;
    }

    ///<summary>
    /// Check if the given ChatFlag has the given mask
    ///</summary>
    private bool HasMask(string name, ChatMessenger.ChatFlag flag )
    {
        if(!GetMask(name, out uint mask))
            return false;
        return (mask & flag.flag) == mask;
    }
}
