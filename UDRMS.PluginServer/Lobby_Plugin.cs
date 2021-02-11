using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DarkRift;
using DarkRift.Server;

using UDRMS.Shared;

//TODO: Client Disconnection
//TODO: Client Connection -> Login / Password

namespace UDRMS.PluginServer
{
    public class Lobby_Plugin : Plugin
    {
        public static Lobby_Plugin lobbyPluginInstance;

        Dictionary<IClient, Lobby_Player> players = new Dictionary<IClient, Lobby_Player>();
        Dictionary<ushort, Lobby_Match> matchs = new Dictionary<ushort, Lobby_Match>();
        Lobby_Configuration configuration;
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
            players.Remove(e.Client);
            //TODO: Find Player on Match and Remove
        }
        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {
                using (DarkRiftReader r = message.GetReader())
                {
                    if (message.Tag == UDRMS_Tags.getLobbyMatchs)
                    {
                        GetLobbyRequestAndSend(e, r);
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
                                //TODO: With Host is Already created a Match Destroy Current Match
                                return;
                            }
                        }
                        CreateMatch(players[e.Client], configuration.maxPlayersPerMatch);
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
                            //TODO: connect to match
                            Lobby_Match.SendToPlayerLobbyMatchInformation(lobby_Player);
                        }
                        else
                        {
                            //TODO: Send Player to Lobby or Do Nothing or Send Refresh
                        }
                    }
                    if (message.Tag == UDRMS_Tags.LoginInfo)
                    {
                        string playerName = r.ReadString();
                        
                        //TODO: change to MongoDB Implementation
                        players[e.Client].playerName = playerName;

                        //TODO: Can Login
                        bool canLogin = true;
                        using (DarkRiftWriter w = DarkRiftWriter.Create())
                        {
                            w.Write(canLogin);
                            using (Message m = Message.Create(UDRMS_Tags.LoginInfo, w))
                                e.Client.SendMessage(m, SendMode.Reliable);
                        }
                        

                    }
                }
            }
        }
        private void GetLobbyRequestAndSend(MessageReceivedEventArgs e, DarkRiftReader r)
        {
            ushort page = r.ReadUInt16();
            if(page == 0)
                page = 1;
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
                    w.Write(match);
                    // w.Write(match.matchID);
                    // w.Write(match.matchOwner.client.ID);
                    // w.Write((ushort)match.matchPlayers.Count);
                }
                using (Message mes = Message.Create(UDRMS_Tags.getLobbyMatchs, w))
                    e.Client.SendMessage(mes, SendMode.Reliable);
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
            configuration.matchsCount++;
            if (matchs.ContainsKey(configuration.matchsCount))
                configuration.matchsCount++;

            Lobby_Match match = new Lobby_Match(configuration.matchsCount, maxPlayersInMatch, matchOwner, configuration.removeMatchOnOwnerExit);

            match.JoinMatch(matchOwner.client, true);

            matchs.Add(configuration.matchsCount, match);
        }
        void RemoveMatch(ushort matchIDToRemove, ushort matchResult){
           Lobby_Match match = matchs[matchIDToRemove];

            
        }
        public List<Lobby_Match> GetLobbysPerPage(ushort page)
        {
            ushort pages = GetLobbyPages();

            if (pages < page)
                page = pages;

            List<Lobby_Match> m = new List<Lobby_Match>();

            int numbersOfLobbys = configuration.matchsPerPageToSendToPlayer + (configuration.matchsPerPageToSendToPlayer * page);

            if (numbersOfLobbys > pages)
                numbersOfLobbys = pages;

            for (int i = 1 * (configuration.matchsPerPageToSendToPlayer * page); i < numbersOfLobbys; i++)
            {
                m.Add(matchs.Values.ElementAt(i));
            }
            return m;
        }
        ushort GetLobbyPages()
        {
            return (ushort)(matchs.Count / configuration.matchsPerPageToSendToPlayer);
        }
    }
    public class Lobby_Configuration{
        public ushort matchsCount = 0;
        public ushort maxPlayersPerMatch = 8;
        public ushort matchsPerPageToSendToPlayer = 5;
        public bool removeMatchOnOwnerExit = true;

        public Lobby_Configuration(){

        }
        public Lobby_Configuration(ushort matchsCount = 0, ushort maxPlayersPerMatch = 8, ushort matchsPerPageToSendToPlayer = 5){
            this.matchsCount = matchsCount;
            this.maxPlayersPerMatch = maxPlayersPerMatch;
            this.matchsPerPageToSendToPlayer = matchsPerPageToSendToPlayer;
        }
    }
}
