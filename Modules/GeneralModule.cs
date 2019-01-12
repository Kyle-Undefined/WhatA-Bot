namespace WhatANerd.Modules
{
    using Newtonsoft.Json;
    using Qmmands;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WhatANerd.Models;

    public class GeneralModule : BotBase
    {
        [Command("Invite")]
        public async Task InviteAsync()
        {
            await ReplyAsync("This nerd wants to invite me to his guild, let's go! <https://goo.gl/nXyp7W>");
        }

        [Command("Stats")]
        public async Task StatsAsync()
        {
            var data = JsonConvert.DeserializeObject<Data>(File.ReadAllText("data.json"));
            var app = await Context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
            var sb = new StringBuilder();

            sb.AppendLine("```");
            sb.Append("Guilds: ")
                .Append(Context.Client.Guilds.Count)
                .AppendLine();
            sb.Append("Nerds: ")
                .Append(Context.Client.Guilds.Sum(x => x.Users.Count))
                .AppendLine();
            sb.Append("Nerds called out: ")
                .AppendLine(data.TotalNerds.ToString());
            sb.Append("Uptime: ")
                .AppendLine((DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss"));
            sb.Append("Heap Size: ")
                .Append(Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2))
                .AppendLine(" MB");
            sb.Append("Developer: ")
                .Append(app.Owner)
                .AppendLine(" - https://github.com/Kyle-Undefined");
            sb.AppendLine("```");

            await ReplyAsync($"Check these stats out nerd!\n\n{sb}");
        }
    }
}