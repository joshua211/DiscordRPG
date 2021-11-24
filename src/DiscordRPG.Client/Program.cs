using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordRPG.Application;
using DiscordRPG.Application.Settings;
using DiscordRPG.Client.Handlers;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
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
            .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        new Program().MainAsync().GetAwaiter().GetResult();
    }

    public async Task MainAsync()
    {
        var host = new HostBuilder().ConfigureServices(collection => ConfigureServices(collection)).Build();
        var serviceProvider = host.Services;

        var client = serviceProvider.GetService<DiscordSocketClient>();
        client.Log += LogClientMessage;

        var token = Config.GetSection("Bot")["Token"];
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        foreach (var handler in serviceProvider.GetServices<IHandler>())
        {
            await handler.InstallAsync();
        }

        await host.RunAsync();
    }

    private IServiceProvider ConfigureServices(IServiceCollection services)
    {
        Config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", false)
            .Build();
        //Common
        services.AddSingleton(Config);
        services.AddSingleton<ILogger>(Log.Logger);

        //Database Settings
        services.Configure<CharacterDatabaseSettings>(Config.GetSection(nameof(CharacterDatabaseSettings)));
        services.AddSingleton<ICharacterDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<CharacterDatabaseSettings>>().Value);
        services.Configure<GuildDatabaseSettings>(Config.GetSection(nameof(GuildDatabaseSettings)));
        services.AddSingleton<IGuildDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<GuildDatabaseSettings>>().Value);
        services.Configure<ActivityDatabaseSettings>(Config.GetSection(nameof(ActivityDatabaseSettings)));
        services.AddSingleton<IActivityDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<ActivityDatabaseSettings>>().Value);

        //Services
        var client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All,
            LogLevel = LogSeverity.Verbose,
        });
        services.AddSingleton<DiscordSocketClient>(client);
        services.AddMediatR(typeof(Core.Core).Assembly, typeof(Application.Application).Assembly);
        services.AddSingleton<ApplicationCommandHandler>();
        services.AddSingleton<ServerHandler>();
        services.AddSingleton<IHandler>(x => x.GetRequiredService<ApplicationCommandHandler>());
        services.AddSingleton<IHandler>(x => x.GetRequiredService<ServerHandler>());
        services.AddSingleton<IHandler, MessageCommandHandler>();
        services.AddSingleton(new CommandService());
        services.AddApplication();
        ApplicationCommandHandler.AddCommands(services);

        //Hangfire
        services.AddHangfire(config => { config.UseSerilogLogProvider(); });
        JobStorage.Current = new MongoStorage(
            MongoClientSettings.FromConnectionString(Config.GetSection("Hangfire")["ConnectionString"]), "Hangfire",
            new MongoStorageOptions()
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                    BackupStrategy = new CollectionMongoBackupStrategy(),
                },
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
            });
        services.AddHangfireServer();

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