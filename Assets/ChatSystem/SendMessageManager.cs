using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ChatSystem
{
    public static class SendMessageManager
    {
        private static NetDataWriter _writer = new NetDataWriter();

        public class Client
        {
            #region Send message to the server. The server will then dispatch the message to the correct clients
            public static void SendMessageToTeam(int playerIdFrom, int teamFrom, int teamTo, string message, ChatTransportConnection connection)
            {
                if(playerIdFrom == -1) { Debug.LogError("You are trying to send a message when the client is not connected to a server"); return; }
                _writer.Reset();
                _writer.Put(true); // send to team
                _writer.Put((ushort)playerIdFrom);
                _writer.Put(PackTeamIds(teamFrom, teamTo));
                _writer.Put(message);
                connection.ChatSend(_writer.Data);
            }

            public static void SendMessageToClient(int playerIdFrom, int teamFrom, int playerIdTo, string message, ChatTransportConnection connection)
            {
                if (playerIdFrom == -1) { Debug.LogError("You are trying to send a message when the client is not connected to a server"); return; }
                _writer.Reset();
                _writer.Put(false); // send to specific client
                _writer.Put((ushort)playerIdFrom);
                _writer.Put((byte)teamFrom);
                _writer.Put((ushort)playerIdTo);
                _writer.Put(message);
                connection.ChatSend(_writer.Data);
            }
            #endregion
        }

        static byte PackTeamIds(int teamFrom, int teamTo)
        {
            // Check if IDs are in range
            if (teamFrom < 0 || teamFrom > 15 || teamTo < 0 || teamTo > 15)
            {
                throw new ArgumentOutOfRangeException("IDs must be in the range of 0 to 15.");
            }

            byte packedIds = (byte)((teamFrom << 4) | teamTo);

            return packedIds;
        }
    }
}
