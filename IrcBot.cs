using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gunderson.BotModules;
using Meebey.SmartIrc4net;

namespace Gunderson
{
    public class IrcBot
    {

        public string Server { get; set; }
        public int Port { get; set; }
        public string[] Channels { get; set; }

        public string Nickname { get; set; }
        public string RealName { get; set; }
        public string Username { get; set; }

        public List<IBotModule> Modules { get; set; }

        private readonly IrcClient _irc;

        public IrcClient Client
        {
            get { return _irc; }
        }


        public IrcBot() :
            this(null, 0, null)
        {

        }

        public IrcBot(string server, int port, string[] channels)
        {
            Server = server;
            Port = port;
            Channels = channels;

            Modules = new List<IBotModule>();

            _irc = new IrcClient();
            _irc.AutoRetry = true;
            _irc.AutoRetryDelay = 10;
            _irc.AutoRetryLimit = 9999;
            _irc.OnConnected += _irc_OnConnected;
            _irc.OnDisconnected += _irc_OnDisconnected;
        }

        void _irc_OnDisconnected(object sender, EventArgs e)
        {
            Connect();
        }

        public void Start()
        {
            foreach (var module in Modules)
            {
                module.Register(this);
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(StartInternal));
        }

        private void StartInternal(object state)
        {
            Connect();
        }

        private void Connect()
        {
            _irc.Connect(Server, Port);
        }

        public void Stop()
        {
            _irc.RfcQuit("Doesn't Gil get a lick?!", Priority.Critical);
            _irc.Disconnect();

        }
        
        void _irc_OnConnected(object sender, EventArgs e)
        {
            _irc.Login(Nickname, RealName, 0, Username);

            foreach (var channel in Channels)
            {
                _irc.RfcJoin(channel);
            }

            _irc.Listen();
        }

    }
}
