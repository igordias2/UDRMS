using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDRMS_Server_Plugin
{
    internal static class UDRMS_Tags
    {
        public static readonly ushort connectedToMS = 0;

        public static readonly ushort getLobbyMatchs = 2;
        public static readonly ushort connectLobbyMatch = 3;
        public static readonly ushort createLobbyMatch = 4;
        public static readonly ushort refreshLobbyMatchs = 5;
        public static readonly ushort getLobbyMatchInfo = 6;

    }
}
