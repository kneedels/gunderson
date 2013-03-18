using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meebey.SmartIrc4net;

namespace Gunderson.BotModules
{
    public class EmailBotModule : IBotModule
    {
        private IrcBot _bot;
        private string ListName;

        public void Register(IrcBot bot)
        {
            _bot = bot;
            Configure();
            EmailMonitor.EmailReceived += ProcessEmail;
        }

        public void Configure()
        {
            ListName = ConfigurationManager.AppSettings["EmailIrcModuleListName"];
        }

        public void ProcessEmail(EmailData email)
        {
            try
            {
                string message = string.Format(
                    "\u0002\u00035{0} \u00032wrote {3} with subject: \u00035\"{1}\" \u00032and body: \u00035{2}",
                     email.From,
                     email.Subject,
                     email.ContentSummary,
                     ListName);

                _bot.Client.RfcPrivmsg(_bot.Channels[0], message);
            }
            catch (Exception e)
            {
                // :(
            }
        }

    }
}
