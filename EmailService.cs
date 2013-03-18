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

namespace Gunderson
{
    partial class EmailService : ServiceBase
    {
        public EmailService()
        {
            InitializeComponent();
        }

        private EmailMonitor monitor;

        protected override void OnStart(string[] args)
        {
            monitor = new EmailMonitor();

            var settings = ConfigurationManager.AppSettings;

            monitor.MailServer = settings["EmailServer"];
            monitor.MailAccount = settings["EmailAccount"];
            monitor.Password = settings["EmailPassword"];
            monitor.Port = int.Parse(settings["EmailPort"]);
            monitor.MailboxName = settings["EmailMailboxName"];
            monitor.ProcessedFolderName = settings["EmailProcessedFolderName"];
            monitor.CheckInterval = int.Parse(settings["EmailCheckInterval"]);
            monitor.MaxMessageLength = int.Parse(settings["EmailIrcModuleMaxMessageLength"]);
            monitor.SubjectMarker = settings["EmailIrcModuleSubjectMarker"];
            monitor.VerificationHeaderName = settings["EmailIrcModuleVerificationHeaderName"];
            monitor.VerificationHeaderValue = settings["EmailIrcModuleVerificationHeaderValue"];

            monitor.Start();
        }

        protected override void OnStop()
        {
            if (monitor != null)
            {
                monitor.Stop();
            }
        }

        public void ConsoleStart(string[] args)
        {
            OnStart(args);
        }
    }
}
