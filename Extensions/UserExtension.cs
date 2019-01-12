namespace WhatANerd.Extensions
{
    using Discord.WebSocket;

    public static class UserExtension
    {
        public static string GetDisplayName(this SocketGuildUser user) => user.Nickname ?? user.Username;
    }
}