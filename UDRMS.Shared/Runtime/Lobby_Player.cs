using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift.Server;

namespace UDRMS.Shared
{
    public class Lobby_Player
    {
        public IClient client;
        public string playerName;
        Lobby_Match currentMatch;

        public Lobby_Player()
        {
            //this.client = client;
        }
        public Lobby_Player(IClient client)
        {
            this.client = client;
        }
        public void AssignMatch(Lobby_Match lobby_Match)
        {
            this.currentMatch = lobby_Match;
        }
        public void RemoveAssignedMatch()
        {
            this.currentMatch = null;
        }
        public Lobby_Match getCurrentMatch()
        {
            return currentMatch;
        }
    }
}
