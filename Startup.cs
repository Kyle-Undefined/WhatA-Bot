namespace WhatANerd
{
    using Discord;
    using Discord.WebSocket;
    using Microsoft.Extensions.DependencyInjection;
    using Qmmands;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using WhatANerd.Models;
    using WhatANerd.Services;

    public class Startup
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly LogService _log;
        private readonly MessageHandlerService _messageHandler;
        private readonly IServiceProvider _services;
        private readonly Tokens _tokens;

        public Startup(IServiceProvider services)
        {
            _services = services;
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _commands = _services.GetRequiredService<CommandService>();
            _log = _services.GetRequiredService<LogService>();
            _messageHandler = _services.GetRequiredService<MessageHandlerService>();
            _tokens = _services.GetRequiredService<Tokens>();
        }

        public async Task StartAsync()
        {
            SetupEvents();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            await _client.LoginAsync(TokenType.Bot, _tokens.Discord);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task OnJoinedGuildAsync(SocketGuild guild)
        {
            await _client.SetActivityAsync(new Game($"over {_client.Guilds.Sum(x => x.Users.Count)} nerds!", ActivityType.Watching));
            await _log.LogMessage(new LogMessage(LogSeverity.Info, "JoinedGuild", $"[{guild.Name}] - Total guilds: {_client.Guilds.Count}"));
            await guild.DefaultChannel.SendMessageAsync("Hey nerds! I'm a simple bot, I just like to call people nerds! I'll be keeping an eye on all of you from now on... You can view my commands by tagging me with the word help like so: `@WhatANerd help`");
        }

        private async Task OnLeftGuildAsync(SocketGuild guild)
        {
            await _client.SetActivityAsync(new Game($"over {_client.Guilds.Sum(x => x.Users.Count)} nerds!", ActivityType.Watching));
            await _log.LogMessage(new LogMessage(LogSeverity.Info, "LeftGuild", $"[{guild.Name}] - Total guilds: {_client.Guilds.Count}"));
        }

        private async Task OnReadyAsync()
        {
            var nerds = _client.Guilds.Sum(x => x.Users.Count);
            await _client.SetActivityAsync(new Game($"over {nerds} nerds!", ActivityType.Watching));
            await _log.LogMessage(new LogMessage(LogSeverity.Info, "Presence", $"Activity has been set to: [{ActivityType.Watching}] over {nerds} nerds!"));

            SetupPresenceTimer();
        }

        private void SetupEvents()
        {
            _client.JoinedGuild += OnJoinedGuildAsync;
            _client.LeftGuild += OnLeftGuildAsync;
            _client.Log += _log.LogMessage;
            _client.MessageReceived += _messageHandler.OnMessageReceivedAsync;
            _client.Ready += OnReadyAsync;

            _commands.CommandErrored += _messageHandler.OnCommandErroredAsync;
            _commands.CommandExecuted += _messageHandler.OnCommandExecutedAsync;
        }

        private void SetupPresenceTimer()
        {
            var presenceTimer = new Timer(async _ => await _client.SetActivityAsync(new Game($"over {_client.Guilds.Sum(x => x.Users.Count)} nerds!", ActivityType.Watching)));

            presenceTimer.Change(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }
    }
}