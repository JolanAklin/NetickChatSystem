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
                if (toTeam)
                {
                    int playerIdFrom = reader.GetUShort();
                    byte packedTeamsId = reader.GetByte();
                    int teamFrom;
                    int teamTo;
                    UnpackTeamIds(packedTeamsId, out teamFrom, out teamTo);
                    string message = reader.GetString();
                    Debug.Log("from " + playerIdFrom + " in team " + teamFrom + " to " + teamTo + " message " + message);
                }
                else
                {
                    int playerIdFrom = reader.GetUShort();
                    int teamFrom = reader.GetByte();
                    int playerTo = reader.GetUShort();
                    string message = reader.GetString();
                }
            }

            static void UnpackTeamIds(byte packedIds, out int teamFrom, out int teamTo)
            {
                teamFrom = packedIds >> 4;
                teamTo = packedIds & 0x0F;
            }
        }
    }
}
