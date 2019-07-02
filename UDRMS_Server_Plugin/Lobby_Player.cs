using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift.Server;

namespace UDRMS_Server_Plugin
{
    public class Lobby_Player
    {
        public IClient client;
        
        public Lobby_Player(IClient client)
        {
            this.client = client;
        }
    }
}
