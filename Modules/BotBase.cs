namespace WhatANerd.Modules
{
    using Discord;
    using Qmmands;
    using System.Threading.Tasks;

    public class BotBase : ModuleBase<GuildContext>
    {
        public async Task<IUserMessage> ReplyAsync(string message) => await Context.Channel.SendMessageAsync(message);
    }
}