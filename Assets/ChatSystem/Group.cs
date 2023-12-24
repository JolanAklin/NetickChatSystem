using UnityEngine;

namespace ChatSystem
{
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
    }
}

