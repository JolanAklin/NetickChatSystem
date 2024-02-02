using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ChatSystem
{
    public class TeamManager : MonoBehaviour
    {
        [SerializeField]
        private List<Team> _teams = new List<Team>();

        private void Awake()
        {
            if(_teams.Count >= 255)
            {
                Debug.Log("There should not be more than 255 teams");
                return;
            }

            for (int i = 0; i < _teams.Count; i++)
            {
                _teams[i].ID = (byte)(i+1);
            }
        }

        public Team GetTeam(byte id)
        {
            if (id == 0 || id > _teams.Count) return null;
            return _teams[id-1];
        }

        public Team[] GetTeams()
        {
            return _teams.ToArray();
        }
    }

    [Serializable]
    public class Team
    {
        [SerializeField]
        private string _name;
        public string Name { get => _name; }
        [SerializeField]
        private Color _color;
        public Color Color { get => _color; }

        [SerializeField]
        private byte id;
        public byte ID { get => id; set { if(id == 0) id = value; } }

        public Team(string name, Color teamColor)
        {
            _name = name;
            _color = teamColor;
            id = 0;
        }
    }
}