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

        //Dictionary<IClient, Player> players = new Dictionary<IClient, Player>();

        public override Version Version => new Version(1, 0, 0);
        public override bool ThreadSafe => true;
        public Lobby_Plugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            lobbyPluginInstance = this;
            ClientManager.ClientConnected += ClientConnected;
            ClientManager.ClientDisconnected += ClientManager_ClientDisconnected;

        }

        //internal void Send(IClient client, Vector3 position)
        //{

        //}

        internal void Send()
        {
            throw new NotImplementedException();
        }

        private void ClientManager_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {

        }

        private void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Client.MessageReceived += MessageReceived;

            //Player p = new Player(e.Client, 100f, );
            //players.Add(e.Client, p);

            //using (DarkRiftWriter w = DarkRiftWriter.Create())
            //{   

            //    //using (Message m = Message.Create(Tags.connectedSendArgs, w))
            //     //   e.Client.SendMessage(m, SendMode.Reliable);
            //}

        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}
