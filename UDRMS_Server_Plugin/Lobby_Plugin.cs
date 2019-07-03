using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DarkRift;
using DarkRift.Server;

namespace UDRMS_Server_Plugin
{
    public class Lobby_Plugin : Plugin
    {
        public static Lobby_Plugin lobbyPluginInstance;

        Dictionary<IClient, Lobby_Player> players = new Dictionary<IClient, Lobby_Player>();

        Dictionary<ushort, Lobby_Match> matchs = new Dictionary<ushort, Lobby_Match>();
        ushort matchsCount = 0;
        ushort maxPlayers = 4;

        ushort matchsPerPageToSendToPlayer = 10;

        public override Version Version => new Version(1, 0, 0);
        public override bool ThreadSafe => true;


        public Lobby_Plugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            lobbyPluginInstance = this;
            ClientManager.ClientConnected += ClientConnected;
            ClientManager.ClientDisconnected += ClientManager_ClientDisconnected;

        }
        private void ClientConnected(object sender, ClientConnectedEventArgs e)
        {

            e.Client.MessageReceived += MessageReceived;
            Lobby_Player lP = new Lobby_Player(e.Client);
            players.Add(e.Client, lP);

            using (Message m = Message.CreateEmpty(UDRMS_Tags.connectedToMS))
                e.Client.SendMessage(m, SendMode.Reliable);

        }
        private void ClientManager_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {

        }
        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {
                using (DarkRiftReader r = message.GetReader())
                {
                    if (message.Tag == UDRMS_Tags.getLobbyMatchs)
                    {
                        ushort page = r.ReadUInt16();
                        List<Lobby_Match> m = GetLobbysPerPage(page);

                        if (m.Count == 0)
                        {
                            using (Message mes = Message.CreateEmpty(UDRMS_Tags.getLobbyMatchs))
                                e.Client.SendMessage(mes, SendMode.Reliable);

                            return;
                        }

                        using (DarkRiftWriter w = DarkRiftWriter.Create())
                        {
                            foreach (Lobby_Match match in m)
                            {
                                w.Write(match.matchID);
                                w.Write(match.matchOwner.client.ID);
                                w.Write((ushort)match.matchPlayers.Count);
                            }
                            using (Message mes = Message.Create(UDRMS_Tags.getLobbyMatchs, w))
                                e.Client.SendMessage(mes, SendMode.Reliable);
                        }
                    }
                    if (message.Tag == UDRMS_Tags.connectLobbyMatch)
                    {
                        ushort matchToJoin = r.ReadUInt16();

                        bool canJoin = matchs[matchToJoin].AddPlayerToMatch(players[e.Client]);

                        matchs[matchToJoin].JoinMatch(e.Client, canJoin);
                    }
                    if (message.Tag == UDRMS_Tags.createLobbyMatch)
                    {
                        foreach (Lobby_Match match in matchs.Values)
                        {
                            if (match.matchOwner == players[e.Client])
                            {
                                //Destroy Match
                                return;
                            }
                        }
                        CreateMatch(players[e.Client], maxPlayers);
                    }
                    if (message.Tag == UDRMS_Tags.refreshLobbyMatchs)
                    {
                        using (DarkRiftWriter w = DarkRiftWriter.Create())
                        {
                            ushort totalLobbyPages = GetLobbyPages();
                            w.Write(totalLobbyPages);
                            using (Message m = Message.Create(UDRMS_Tags.refreshLobbyMatchs, w))
                                e.Client.SendMessage(m, SendMode.Reliable);
                        }
                    }
                    if (message.Tag == UDRMS_Tags.getLobbyMatchInfo)
                    {
                        Lobby_Player lobby_Player = players[e.Client];

                        if (lobby_Player.getCurrentMatch() != null)
                        {
                            //connect to match
                            Lobby_Match.SendToPlayerLobbyMatchInformation(lobby_Player);
                        }
                        else
                        {
                            //Send Player to Lobby
                        }
                    }
                }
            }
        }

        //private bool playerIsOnAMatch(Lobby_Player lobby_Player)
        //{
        //    foreach (var item in matchs.Values)
        //    {
        //        if (item.matchPlayers.Contains(lobby_Player))
        //            return true;
        //    }
        //    return false;
        //}

        void CreateMatch(Lobby_Player matchOwner, ushort maxPlayersInMatch)
        {
            matchsCount++;
            if (matchs.ContainsKey(matchsCount))
                matchsCount++;

            Lobby_Match match = new Lobby_Match(matchsCount, maxPlayersInMatch, matchOwner);

            match.JoinMatch(matchOwner.client, true);

            matchs.Add(matchsCount, match);
        }
        public List<Lobby_Match> GetLobbysPerPage(ushort page)
        {
            ushort pages = GetLobbyPages();
            if (pages < page)
                page = pages;

            List<Lobby_Match> m = new List<Lobby_Match>();

            int numbersOfLobbys = matchsPerPageToSendToPlayer + (matchsPerPageToSendToPlayer * page);

            if (numbersOfLobbys > pages)
                numbersOfLobbys = pages;

            for (int i = 1 * (matchsPerPageToSendToPlayer * page); i < numbersOfLobbys; i++)
            {
                m.Add(matchs.Values.ElementAt(i));
            }
            return m;
        }
        ushort GetLobbyPages()
        {
            return (ushort)(matchs.Count / matchsPerPageToSendToPlayer);
        }
    }
}
