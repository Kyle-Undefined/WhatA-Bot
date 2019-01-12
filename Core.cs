namespace WhatANerd
{
    using Discord;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;
    using Qmmands;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using WhatANerd.Models;
    using WhatANerd.Services;

    public static class Core
    {
        private static async Task Main()
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 50
                }))
                .AddSingleton(new CommandService(new CommandServiceConfiguration
                {
                    CaseSensitive = false,
                    DefaultRunMode = RunMode.Sequential,
                    IgnoreExtraArguments = true
                }))
                .AddSingleton(configuration.GetSection("Tokens").Get<Tokens>())
                .AddSingleton(configuration.GetSection("Support").Get<Support>())
                .AddSingleton<Random>()
                .AddSingleton<MessageHandlerService>()
                .AddTransient<LogService>()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Information);
                    builder.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                })
                .BuildServiceProvider();

            var bot = new Startup(services);
            await bot.StartAsync();
        }
    }
}