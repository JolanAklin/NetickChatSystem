using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    [CreateAssetMenu(fileName = "ChatSystemConfig", menuName = "Chat system/Config", order = 1)]
    public class Config : ScriptableObject
    {

        public static Config instance {  get; private set; }

        [SerializeField]
        private Group[] _chatGroups = new Group[0];

        [SerializeField]
        private OnMessageReceivedEvent[] _onMessageReceivedEvents = new OnMessageReceivedEvent[0];

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

            for (int i = 0; i < _onMessageReceivedEvents.Length; i++)
            {
                if (_onMessageReceivedEvents[i] != null)
                {
                    _onMessageReceivedEvents[i]._eventId = (uint)i;
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

        public OnMessageReceivedEvent GetOnMessageReceivedEventById(int id)
        {
            if(_onMessageReceivedEvents.Length > id)
                if (_onMessageReceivedEvents[id] != null)
                    return _onMessageReceivedEvents[id];
            return null;
        }
    }
}
