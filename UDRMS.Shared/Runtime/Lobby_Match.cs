using System;
using System.Linq;
using System.Collections.Generic;

using DarkRift;

namespace UDRMS.Shared
{
    //TODO: Remove from Current Match
    public class Lobby_Match : IDarkRiftSerializable
    {
        public ushort matchID;
        public ushort maxPlayersInMatch;
        public Lobby_Player matchOwner;

        public Dictionary<Lobby_Player, bool> matchPlayers = new Dictionary<Lobby_Player, bool>(); 
        public Lobby_Match(){
            
        }
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
                matchPlayers.Add(playerToAdd, false);

            playerToAdd.AssignMatch(this);
            return true;
        }
        public void RemovePlayerFromMatch(Lobby_Player playerToRemove, bool finishedMatch = false)
        {
            this.matchPlayers.Remove(playerToRemove);
            
            //TODO: Dont overflow with change host

            if(matchOwner == playerToRemove)
            {
                //TODO: Destroy Match or Change Owner
            }
            if (matchPlayers.Count <= 0)
            {
                //TODO: Destroy Match
            }

        }

        public void FinishMatch(Lobby_Player matchWinner)
        {
            foreach (Lobby_Player player in matchPlayers.Keys)
            {
                RemovePlayerFromMatch(player, true);
            }
            //TODO: Set Match Winner
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
        public void ChangeLobbyReadyStatus(Lobby_Player lobby_Player)
        {
            this.matchPlayers[lobby_Player] = !this.matchPlayers[lobby_Player];
            foreach (Lobby_Player lp in this.matchPlayers.Keys)
            {
                Lobby_Match.SendLobbyReadyStatusToPlayer(this.matchPlayers[lobby_Player], lp);
            }
        }

        public bool CanStartMatch()
        {
            foreach (var item in this.matchPlayers.Values)
            {
                if (!item)
                    return false;
            }

            return true;
        }

        public static void SendToPlayerLobbyMatchInformation(Lobby_Player lobby_Player)
        {
            Lobby_Match match = lobby_Player.getCurrentMatch();
            using (DarkRiftWriter w = DarkRiftWriter.Create())
            {
                foreach (var item in match.matchPlayers.Keys)
                {
                    w.Write(item.client.ID);            //ID do Player
                    //w.Write(item)
                    w.Write(match.matchPlayers[item]);  //Status (Ready)
                }
                using (Message m = Message.Create(UDRMS_Tags.getLobbyMatchInfo, w))
                    lobby_Player.client.SendMessage(m, SendMode.Reliable);
            }
        }
        public static void SendLobbyReadyStatusToPlayer(bool readyStatus, Lobby_Player playerToSend)
        {
            using (DarkRiftWriter w = DarkRiftWriter.Create())
            {
                w.Write(readyStatus);
                using (Message mes = Message.Create(UDRMS_Tags.connectLobbyMatch, w))
                    playerToSend.client.SendMessage(mes, SendMode.Reliable);
            }
            
           
        }


        public void Serialize(SerializeEvent w){
            w.Writer.Write(this.matchID);
            w.Writer.Write(this.matchOwner.client.ID);
            w.Writer.Write((ushort)this.matchPlayers.Count);
            w.Writer.Write(this.maxPlayersInMatch);
        }
        public void Deserialize(DeserializeEvent e){
            this.matchID = e.Reader.ReadUInt16();
            ushort matchOwnerID = e.Reader.ReadUInt16();
            ushort actualPlayers = e.Reader.ReadUInt16();
            this.maxPlayersInMatch = e.Reader.ReadUInt16();
        }
    }
}