using DSharpPlus;
using DSharpPlus.CommandsNext;

using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using System.IO;
using Newtonsoft.Json;

namespace ExBot
{
    class Program
    {
        static DiscordClient _discord;
        static CommandsNextModule _commands;



        public static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync (string[] args)
        {
            //Guild-ID: 334779430100795412
            //Channel-ID-Voice (dev): 359651553973370880
            //Channel-ID-Text (dev): 359645522601967617

            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var _discordjson = JsonConvert.DeserializeObject<ConfigJson>(json);

            _discord = new DiscordClient(new DiscordConfiguration
            {
                Token = _discordjson.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            _commands = _discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "-"
            });

            _commands.RegisterCommands<MyCommands>();

            _discord.Ready += _discord_Ready;
            _discord.GuildAvailable += _discord_GuildAvailable;
            _discord.ClientErrored += _discord_ClientErrored;
            //_discord.VoiceStateUpdated += _discord_VoiceStateUpdated;
            
            Join();
            //Leave();

            await _discord.ConnectAsync();
            await Task.Delay(-1);
        }

        #region _discord Client Log
        private static Task _discord_ClientErrored (ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "ExampleBot", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task _discord_GuildAvailable (GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "ExBot", $"Guild available: {e.Guild.Name}, ID: {e.Guild.Id}", DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task _discord_Ready (ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "ExBot", "Client is ready to process events.", DateTime.Now);
            return Task.CompletedTask;
        }
        #endregion _discord Client Log
        
        private static Task _discord_VoiceStateUpdated (VoiceStateUpdateEventArgs e)
        {
            try
            {
                var user = e.User.Username;
                var voicechannel = e.Channel.Id;
                if (voicechannel == 0)
                {
                    e.Client.DebugLogger.LogMessage(LogLevel.Info, "ExBot", user + "left." , DateTime.Now);
                }
                var voicechannelname = e.Channel.Name;

                e.Client.DebugLogger.LogMessage(LogLevel.Info, "ExBot", user + " " + voicechannel + " " + voicechannelname, DateTime.Now);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.CompletedTask;

        }

        private static void Join ()
        {
            try
            {
                _discord.VoiceStateUpdated += async e =>
                {
                    var user = e.User.Username;
                    var voicechannel = e.Channel.Id;
                    var voicechannelname = e.Channel.Name;
                    var textchannel = e.Guild.GetChannel(359645522601967617);

                    if (voicechannel == e.Channel.Id)
                    {
                        await textchannel.SendMessageAsync(user + " joined " + voicechannelname + "!");
                    }
                    else if(voicechannel != e.Channel.Id)
                    {
                        await textchannel.SendMessageAsync(user + " left " + voicechannelname + "!");
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public struct ConfigJson
        {
            [JsonProperty("token")]
            public string Token
            {
                get; private set;
            }
        }
    }
}
