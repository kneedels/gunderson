using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Gunderson.BotModules;

namespace Gunderson
{
    public partial class IrcBotService : ServiceBase
    {

        private IrcBot _bot;

        public IrcBotService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Todo: read this from a config
            _bot = new IrcBot();
            ConfigureBot(_bot);
            _bot.Start();
        }

        private static void ConfigureBot(IrcBot bot)
        {
            var settings = ConfigurationManager.AppSettings;

            bot.Server = settings["IrcServer"];
            bot.Port = int.Parse(settings["IrcPort"]);
            bot.Channels = settings["IrcChannels"].Split(',');
            bot.Nickname = settings["IrcNickname"];
            bot.Username = settings["IrcUsername"];
            bot.RealName = settings["IrcRealName"];

            bot.Modules.Add(new EmailBotModule());
            bot.Modules.Add(new FileRenameMonitorModule());
        }

        protected override void OnStop()
        {
            _bot.Stop();
        }

        public void ConsoleStart(string[] args)
        {
            OnStart(args);
        }

        public void ConsoleStop()
        {
            OnStop();
        }
    }
}
