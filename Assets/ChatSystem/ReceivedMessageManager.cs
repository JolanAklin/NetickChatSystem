using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    public static class ReceivedMessageManager
    {
        public class Client
        {
            public static void OnMessagedReceived(int sandboxId, string from, string message, int method)
            {
                OnMessageReceivedEvent onMessageReceivedEvent = ChatSystemManager.ChatSystemConfig.GetOnMessageReceivedEventById(method);
                onMessageReceivedEvent?.Invoke(sandboxId, message);
            }
        }

        public class Server
        {
            public static void HandleIncomingBytes(byte[] data)
            {
                NetDataReader reader = new NetDataReader(data);
                bool toTeam = reader.GetBool();
                Dictionary<uint, Group> groups = ChatSystemManager.ChatSystemConfig.getGroupDict();
                if (toTeam)
                {
                    int playerIdFrom = reader.GetUShort();
                    string playerName = ChatSystemManager._connectedPlayer[playerIdFrom].PlayerName;
                    byte packedTeamsId = reader.GetByte();
                    uint teamFrom;
                    uint teamTo;
                    UnpackTeamIds(packedTeamsId, out teamFrom, out teamTo);
                    Group groupFrom = null;
                    if (teamFrom != 0)
                        groupFrom = groups[teamFrom];
                    Group groupTo = null;
                    if (teamTo != 0)
                        groupTo = groups[teamTo];
                    string message = reader.GetString();
                    Debug.Log("from " + playerName + " in team " + (groupFrom != null ? groupFrom.Name : "no team") + " to " + (groupTo != null ? groupTo.Name : "Everyone")  + " message " + message);
                }
                else
                {
                    int playerIdFrom = reader.GetUShort();
                    uint teamFrom = reader.GetByte();
                    int playerTo = reader.GetUShort();
                    string message = reader.GetString();
                }
            }

            static void UnpackTeamIds(byte packedIds, out uint teamFrom, out uint teamTo)
            {
                teamFrom = (uint)packedIds >> 4;
                teamTo = (uint)packedIds & 0x0F;
            }
        }
    }
}
