using DSharpPlus;
using DSharpPlus.CommandsNext;

using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExBot
{
    class Program
    {
        static DiscordClient _discord;
        static CommandsNextModule _commands;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            OnJoin();
        }

        static async Task MainAsync (string[] args)
        {
            _discord = new DiscordClient(new DiscordConfiguration
            {
                Token = "",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            _discord.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("Louis ist ein Fag!");
            };

            _commands = _discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = ";;"
            });

            _commands.RegisterCommands<MyCommands>();
;

            await _discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static void OnJoin()
        {
            _discord.VoiceStateUpdated += async e =>
            {
                var channel = e.Guild.GetChannel(359645522601967617);
                var user = e.User.Username;
                await channel.SendMessageAsync(string.Format("@" + user + " has joined!"));
            };
        }
    }
}
