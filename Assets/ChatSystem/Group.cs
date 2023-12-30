using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatSystem
{
    [InitializeOnLoad]
    [CreateAssetMenu(fileName = "ChatGroup", menuName = "Chat system/group", order = 1)]
    public class Group : ScriptableObject
    {
        public uint _groupId;

        [SerializeField]
        private Color _color;

        [SerializeField]
        private string _name;
        public string Name { get { return _name; } }

        [SerializeField]
        private int _mask; // what messages the team can see

        public List<ChatPlayer> _players = new List<ChatPlayer>();

        // reset the _players list
        public Group()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                _players.Clear();
            }
        }
    }
}

