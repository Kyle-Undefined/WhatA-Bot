namespace WhatANerd.Modules
{
    using Discord;
    using Discord.WebSocket;
    using Qmmands;

    public class GuildContext : ICommandContext
    {
        public GuildContext(DiscordSocketClient client, IUserMessage message)
        {
            Channel = message.Channel as SocketTextChannel;
            Client = client;
            Guild = (message.Channel as SocketGuildChannel)?.Guild;
            Message = message;
            User = message.Author as SocketGuildUser;
        }

        public SocketTextChannel Channel { get; }
        public DiscordSocketClient Client { get; }
        public SocketGuild Guild { get; }
        public IUserMessage Message { get; }
        public SocketGuildUser User { get; }
    }
}