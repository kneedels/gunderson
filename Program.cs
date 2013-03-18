using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Gunderson
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

            if (RunInConsoleModeIfRequested(args))
            {
                return;
            }

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new IrcBotService(),
                new EmailService()
            };

            ServiceBase.Run(ServicesToRun);

        }

        static bool RunInConsoleModeIfRequested(string[] args)
        {
            if (args.Length > 0 && args[0] == "-console")
            {

                var ircBotService = new IrcBotService();
                ircBotService.ConsoleStart(args);

                var emailService = new EmailService();
                emailService.ConsoleStart(args);

                Console.WriteLine("Press enter to end service testing.");

                Console.ReadLine();
                ircBotService.ConsoleStop();
                return true;
            }
            return false;
        }
    }
}
