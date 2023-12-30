using ChatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DispatchMessages
{
    public static void DispatchGroup(Group target, int playerIdFrom, Group groupFrom, string message)
    {
        if (target == null) return;
        foreach (ChatPlayer player in target._players)
        {
            if (player != null)
            {
                SendMessageManager.Server.SendMessageToTeam(playerIdFrom, target._groupId, groupFrom._groupId, message, player.Connection);
            }
        }
    }
}
