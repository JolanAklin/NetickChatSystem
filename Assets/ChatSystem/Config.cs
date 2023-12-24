using UnityEngine;

namespace ChatSystem
{
    [CreateAssetMenu(fileName = "ChatSystemConfig", menuName = "Chat system/Config", order = 1)]
    public class Config : ScriptableObject
    {

        public static Config instance {  get; private set; }

        [SerializeField]
        private Group[] _chatGroups = new Group[0];

        private void OnValidate()
        {
            instance = this;
            for (int i = 0; i < _chatGroups.Length; i++)
            {
                if(_chatGroups[i] != null )
                {
                    _chatGroups[i]._groupId = (uint)Mathf.Pow(2, i);
                }
            }
        }

        public string[] getGroupList()
        {
            string[] groups = new string[_chatGroups.Length];
            for (int i = 0; i < _chatGroups.Length; i++)
            {
                groups[i] = _chatGroups[i].Name;
            }
            return groups;
        }
    }
}
