using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordRPG.Application;
using DiscordRPG.Application.Settings;
using DiscordRPG.Client.Handlers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace DiscordRPG.Client;

public class Program
{
    public static IConfigurationRoot Config { get; private set; }

    public static void Main(string[] args)
    {
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        new Program().MainAsync().GetAwaiter().GetResult();
    }

    public async Task MainAsync()
    {
        var serviceProvider = ConfigureServices();
        var client = serviceProvider.GetService<DiscordSocketClient>();
        client.Log += LogClientMessage;

        var token = Config.GetSection("Bot")["Token"];

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        foreach (var handler in serviceProvider.GetServices<IHandler>())
        {
            await handler.InstallAsync();
        }

        await Task.Delay(-1);
    }

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        Config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", false)
            .Build();
        services.AddSingleton(Config);
        services.AddSingleton<ILogger>(Log.Logger);

        services.Configure<CharacterDatabaseSettings>(Config.GetSection(nameof(CharacterDatabaseSettings)));
        services.AddSingleton<ICharacterDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<CharacterDatabaseSettings>>().Value);

        services.Configure<GuildDatabaseSettings>(Config.GetSection(nameof(GuildDatabaseSettings)));
        services.AddSingleton<IGuildDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<GuildDatabaseSettings>>().Value);

        services.AddSingleton<DiscordSocketClient>();
        services.AddMediatR(typeof(Core.Core).Assembly, typeof(Application.Application).Assembly);
        services.AddSingleton<ApplicationCommandHandler>();
        services.AddSingleton<IHandler>(x => x.GetRequiredService<ApplicationCommandHandler>());
        services.AddSingleton<IHandler, MessageCommandHandler>();
        services.AddSingleton<IHandler, ServerHandler>();
        services.AddSingleton(new CommandService());
        services.AddApplication();
        ApplicationCommandHandler.AddCommands(services);

        return services.BuildServiceProvider();
    }

    private static Task LogClientMessage(LogMessage msg)
    {
        switch (msg.Severity)
        {
            case LogSeverity.Info:
                Log.Information(msg.Message);
                break;
            case LogSeverity.Debug:
                Log.Debug(msg.Message);
                break;
            case LogSeverity.Verbose:
                Log.Verbose(msg.Message);
                break;
            case LogSeverity.Critical:
                Log.Fatal(msg.Message);
                break;
            case LogSeverity.Warning:
                Log.Warning(msg.Message);
                break;
            case LogSeverity.Error:
                Log.Error(msg.Message);
                break;
        }

        return Task.CompletedTask;
    }
}