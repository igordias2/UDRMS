using System;
using System.Linq;
using System.Collections.Generic;

using DarkRift;

namespace UDRMS_Server_Plugin
{
    public class Lobby_Match
    {
        public ushort matchID;
        public ushort maxPlayersInMatch;
        public Lobby_Player matchOwner;

        public List<Lobby_Player> matchPlayers = new List<Lobby_Player>(); 

        public Lobby_Match(ushort matchID, ushort maxPlayersInMatch, Lobby_Player matchOwner)
        {
            this.matchID = matchID;
            this.maxPlayersInMatch = maxPlayersInMatch;
            this.matchOwner = matchOwner;
            this.AddPlayerToMatch(matchOwner);
        }

        public bool AddPlayerToMatch(Lobby_Player playerToAdd)
        {
            if (matchPlayers.Count >= maxPlayersInMatch)
                return false;
            else
                matchPlayers.Add(playerToAdd);

            return true;
        }
        public void RemovePlayerFromMatch(Lobby_Player playerToRemove)
        {
            if(matchOwner == playerToRemove)
            {
                //Destroy Match or Change Owner
            }
            if (matchPlayers.Count <= 0)
            {
                //Destroy Match
            }
            this.matchPlayers.Remove(playerToRemove);
        }
        public void JoinMatch(DarkRift.Server.IClient p, bool canJoin)
        {
            using (DarkRiftWriter w = DarkRiftWriter.Create())
            {
                w.Write(canJoin);
                w.Write(this.matchID);
                using (Message mes = Message.Create(UDRMS_Tags.connectLobbyMatch, w))
                    p.SendMessage(mes, SendMode.Reliable);
            }
        }
    }
}