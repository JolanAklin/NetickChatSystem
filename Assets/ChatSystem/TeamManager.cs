using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public class TeamManager
    {
        [SerializeField]
        private List<Team> _teams;
        private Dictionary<string, Team> _teamNameToTeam;

        public TeamManager(List<Team> teams)
        {
            _teams = teams;
            _teamNameToTeam = new Dictionary<string, Team>();
            if (_teams.Count >= 255)
            {
                Debug.Log("There should not be more than 255 teams");
                return;
            }

            for (int i = 0; i < _teams.Count; i++)
            {
                _teams[i].ID = (byte)(i + 1);
                if (_teamNameToTeam.ContainsKey(_teams[i].Name))
                {
                    Debug.LogError(_teams[i].Name + " already exists. Name should be unique");
                    continue;
                }
                _teamNameToTeam.Add(_teams[i].Name, _teams[i]);
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

        private byte id;
        public byte ID { get => id; set { if(id == 0) id = value; } }

        private List<IChatPlayer> _players;

        public Team(string name, Color teamColor)
        {
            _name = name;
            _color = teamColor;
            id = 0;
        }

        public void AddPlayer(IChatPlayer player)
        {
            if(_players == null) _players = new List<IChatPlayer>();
            if (_players.Contains(player)) Debug.LogWarning("The player "+player.PlayerName+" is already in the " + _name + " team.");
            else
            {
                player.TeamID = id;
                _players.Add(player);
            }
        }

        public IChatPlayer[] getPlayers() { return _players.ToArray(); }
    }
}