using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{
    public partial class Server
    {
        void PermissionDeniedMessage(OnlineUser user)
        {
            ServerMessage("server", user, langFile["command_permission_denied"]);
        }
    }
}
