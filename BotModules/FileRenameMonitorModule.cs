using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunderson.BotModules
{
    public class FileRenameMonitorModule : IBotModule
    {

        private IrcBot _bot;

        public string FileDirectory;
        public string FileName;
        public string UpdateMessage;

        public void Register(IrcBot client)
        {
            Configure();
            if (Directory.Exists(FileDirectory))
            {
                _bot = client;

                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = FileDirectory;
                watcher.Filter = FileName;
                watcher.NotifyFilter = NotifyFilters.FileName;

                watcher.Renamed += watcher_Renamed;
                watcher.EnableRaisingEvents = true;
            }
            else
            {
                // log something eventually
            }
        }

        private void Configure()
        {
            FileDirectory = ConfigurationManager.AppSettings["FileMonitorDirectory"];
            FileName = ConfigurationManager.AppSettings["FileMonitorFileName"];
            UpdateMessage = "\u0002" + ConfigurationManager.AppSettings["FileMonitorUpdateMessage"];
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            _bot.Client.RfcPrivmsg(_bot.Channels[0], UpdateMessage);
        }

    }
}
