using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meebey.SmartIrc4net;

namespace Gunderson.BotModules
{
    public interface IBotModule
    {
        void Register(IrcBot client);
    }
}
