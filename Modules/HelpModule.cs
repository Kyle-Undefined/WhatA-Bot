namespace WhatANerd.Modules
{
    using Qmmands;
    using System.Threading.Tasks;

    public class HelpModule : BotBase
    {
        [Command("Help")]
        public async Task HelpAsync()
        {
            await ReplyAsync("Ha, look at this nerd asking for help! Below are my commands: \n\n`Stats`, `Invite`, `SupportGuild`");
        }

        [Command("SupportGuild")]
        public async Task SupportGuildAsync()
        {
            await ReplyAsync("Here ya go, nerd! If you need more help, or have feedback on the bot, feel free to drop by the support guild! https://discord.gg/94WWV48");
        }
    }
}