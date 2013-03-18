using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AE.Net.Mail;

namespace Gunderson
{
    class EmailMonitor
    {
        public delegate void ProcessEmail(EmailData email);
        public static event ProcessEmail EmailReceived;

        public string MailServer;
        public string MailAccount;
        public string Password;
        public int Port;
        public string MailboxName;
        public string ProcessedFolderName;
        public int CheckInterval;
        public int MaxMessageLength;

        public string EndOfMessageText = "-- " + System.Environment.NewLine + "-- " + System.Environment.NewLine +
                                         "To post to this group,";

        public string SubjectMarker;

        public string VerificationHeaderName;
        public string VerificationHeaderValue;

        public EmailMonitor()
        {

        }

        private Timer _timer;

        public void Start()
        {
            _timer = new Timer(CheckInterval);
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, args) => CheckEmail();
            _timer.Start();


        }

        public void Stop()
        {
            _timer.Stop();
        }


        private void CheckEmail()
        {
            ImapClient client = null;

            try
            {
                client = new ImapClient(MailServer, MailAccount, Password, ImapClient.AuthMethods.Login, Port, true);

                client.SelectMailbox(MailboxName);
                int messageCount = client.GetMessageCount();
                if (messageCount > 0)
                {
                    var messages = client.GetMessages(0, messageCount, false);

                    foreach (MailMessage message in messages.OrderBy(m=>m.Date))
                    {

                        ProcessAndFireEvent(message);

                        client.MoveMessage(message.Uid, ProcessedFolderName);
                    }
                }
            }
            catch (Exception e)
            {
                // :(
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                }
            }
        }

        private void ProcessAndFireEvent(MailMessage message)
        {

            if (!ShouldFireEvent(message))
            {
                return;
            }

            EmailData data = new EmailData();

            data.From = message.From.DisplayName;

            data.Subject = ScrubAndShortenSubject(message.Subject);
            data.Time = message.Date;
            data.ContentSummary = ScrubAndShortenBody(message.Body);

            Task.Factory.StartNew(() =>
                {
                    lock (this)
                    {
                        EmailReceived(data);
                    }
                });
        }

        private bool ShouldFireEvent(MailMessage message)
        {
            HeaderValue headerValue;

            if (message.Headers.TryGetValue(VerificationHeaderName, out headerValue))
            {
                return headerValue.Value == VerificationHeaderValue;
            }

            return false;
        }

        private string ScrubAndShortenSubject(string input)
        {
            string temp = input;

            temp = temp.Replace(SubjectMarker, "");

            return ScrubAndShorten(temp);
        }

        private string ScrubAndShortenBody(string input)
        {
            string temp = input;

            int endOfMessageIndex = temp.IndexOf(EndOfMessageText);

            if (endOfMessageIndex > 0)
            {
                temp = temp.Substring(0, endOfMessageIndex);
            }

            return ScrubAndShorten(temp);
        }

        private string ScrubAndShorten(string input)
        {

            string retVal = input.Replace(System.Environment.NewLine, " ");

            retVal = retVal.Trim();

            if (retVal.Length > MaxMessageLength)
            {
                retVal = retVal.Substring(0, 140) + " (...)";
            }

            return retVal;
        }
    }
}
