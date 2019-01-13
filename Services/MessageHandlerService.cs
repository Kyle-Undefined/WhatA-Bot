namespace WhatANerd.Services
{
    using Discord;
    using Discord.WebSocket;
    using Newtonsoft.Json;
    using Qmmands;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using WhatANerd.Extensions;
    using WhatANerd.Models;
    using WhatANerd.Modules;

    public class MessageHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly LogService _log;
        private readonly double _probability = 0.01;
        private readonly IServiceProvider _services;
        private readonly Support _support;
        private readonly Random _random = new Random();

        public MessageHandlerService(DiscordSocketClient client, CommandService command, LogService log, IServiceProvider services, Support support)
        {
            _client = client;
            _commands = command;
            _log = log;
            _services = services;
            _support = support;
        }

        public async Task OnCommandErroredAsync(ExecutionFailedResult result, ICommandContext originalContext, IServiceProvider _)
        {
            if (!(originalContext is GuildContext context))
                return;

            if (!string.IsNullOrWhiteSpace(result.Exception.ToString()))
            {
                await _log.LogMessage(new LogMessage(LogSeverity.Error, "Command", string.Empty, result.Exception));

                (_client.GetChannel(_support.ChannelId) as SocketTextChannel)?.SendMessageAsync($"**Command**: {context.Message}\n**Reason**: {result.Reason}\n**Exception**: ```\n{result.Exception.Message}\n{result.Exception.TargetSite}\n```");

                await context.Channel.SendMessageAsync("Hey ya nerd! You broke me! I'll pass this along so it can be looked into. Don't let it happen again...");
            }
        }

        public async Task OnCommandExecutedAsync(Command command, CommandResult _, ICommandContext originalContext, IServiceProvider __)
        {
            if (!(originalContext is GuildContext context))
                return;

            await _log.LogMessage(new LogMessage(LogSeverity.Info, "Command", $"Executed [{command.Name}] for [{context.User.GetDisplayName()}] in [{context.Guild.Name}/{context.Channel.Name}]"));
        }

        public async Task OnMessageReceivedAsync(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message) || message.Author.IsBot || message.Channel is IPrivateChannel)
                return;

            var context = new GuildContext(_client, message);

            if (message.HasMentionPrefix(_client.CurrentUser, out var output))
            {
                var result = await _commands.ExecuteAsync(output, context, _services);
                await CommandExecutedResultAsync(result, context);
                return;
            }

            if (_random.NextDouble() < _probability)
            {
                await context.Channel.SendMessageAsync($"{context.User.GetDisplayName()} is a nerd!");

                if (!File.Exists("data.json"))
                    File.WriteAllText("data.json", JsonConvert.SerializeObject(new Data()));

                var data = JsonConvert.DeserializeObject<Data>(File.ReadAllText("data.json"));
                data.TotalNerds++;
                File.WriteAllText("data.json", JsonConvert.SerializeObject(data));
            }
        }

        private async Task CommandExecutedResultAsync(IResult result, GuildContext context)
        {
            if (result.IsSuccessful)
                return;

            switch (result)
            {
                case ExecutionFailedResult _:
                case CommandResult _:
                    return;

                case CommandNotFoundResult _:
                    await context.Message.AddReactionAsync(new Emoji("\u2754"));
                    break;
            }
        }
    }
}