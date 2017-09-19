using DSharpPlus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExBot
{
    class MyBot
    {
        DiscordConfig _discord;

        public MyBot()
        {
            _discord = new DiscordConfig
            {
                LogLevel = LogSeverity.Info,
            };

            
        }

        static async Task MainAsync(string[] args)
        {

        }

        private void Log(object sender, LogMessage e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
