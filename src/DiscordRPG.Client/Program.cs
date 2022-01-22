using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordRPG.Application;
using DiscordRPG.Application.Settings;
using DiscordRPG.Client.Handlers;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain;
using EventFlow.AspNetCore.Extensions;
using EventFlow.DependencyInjection.Extensions;
using EventFlow.Extensions;
using EventFlow.MongoDB.Extensions;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;

namespace DiscordRPG.Client;

public class Program
{
    public static IHostEnvironment HostEnvironment;
    public static string Version;

    private static string template =
        "[{Timestamp:dd.MM HH:mm:ss} {Level}][{SourceContext:l}.{Method}] {TransactionId} {Message}{NewLine}{Exception}";

    public static IConfigurationRoot Config { get; private set; }

    private static LoggerConfiguration GetLoggerConfiguration() => new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
        .MinimumLevel.Override("EventFlow", LogEventLevel.Verbose)
        .MinimumLevel.Verbose()
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: template);

    public static void Main(string[] args)
    {
        HostEnvironment = new HostingEnvironment();
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        HostEnvironment.EnvironmentName = string.IsNullOrEmpty(env) ? "Production" : env;
        Version = Assembly.GetEntryAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;

        var config = GetLoggerConfiguration();
        Log.Logger = config.CreateBootstrapLogger();

        try
        {
            Log.Information("Starting Application in {Env} environment on version {Version}",
                HostEnvironment.EnvironmentName, Version);
            new Program().MainAsync().GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Fatal Application Error");
        }
    }

    public async Task MainAsync()
    {
        var host = Host.CreateDefaultBuilder().UseSerilog()
            .ConfigureServices(collection => ConfigureServices(collection)).Build();

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

        Log.Information("Running Application in {Env} environment on version {Version}",
            HostEnvironment.EnvironmentName, Version);

        await host.RunAsync();
    }

    private IServiceProvider ConfigureServices(IServiceCollection services)
    {
        ConventionRegistry.Register(
            "DictionaryRepresentationConvention",
            new ConventionPack {new DictionaryRepresentationConvention(DictionaryRepresentation.ArrayOfArrays)},
            _ => true);

        Config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        //Database Settings
        services.Configure<DatabaseSettings>(Config.GetSection(nameof(DatabaseSettings)));
        services.AddSingleton<IDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

        //Common
        var loggerConfig = GetLoggerConfiguration();
        IDatabaseSettings settings = services.BuildServiceProvider().GetService<IDatabaseSettings>();
        ;
        if (HostEnvironment.IsProduction())
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var collection = mongoClient.GetDatabase(settings.DiagnosticDatabaseName);
            loggerConfig.WriteTo.MongoDB(collection, LogEventLevel.Debug, settings.DiagnosticLogCollectionName);
        }

        Log.Logger = loggerConfig.CreateLogger();

        services.AddSingleton(Config);
        services.AddSingleton<IHostEnvironment>(HostEnvironment);
        services.AddSingleton<ILogger>(Log.Logger);

        //Services
        var client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All,
            LogLevel = LogSeverity.Info,
        });
        services.AddSingleton<DiscordSocketClient>(client);
        services.AddSingleton<ApplicationCommandHandler>();
        services.AddSingleton<ServerHandler>();
        services.AddSingleton<IHandler>(x => x.GetRequiredService<ApplicationCommandHandler>());
        services.AddSingleton<IHandler>(x => x.GetRequiredService<ServerHandler>());
        services.AddSingleton<IHandler, WorkerHandler>();
        services.AddSingleton<IHandler, MessageCommandHandler>();
        services.AddSingleton(new CommandService());
        services.AddDomain();
        services.AddApplication();
        services.AddEventFlow(options =>
            options
                .ConfigureMongoDb(settings.ConnectionString, settings.DatabaseName)
                .UseMongoDbEventStore()
                .UseLibLog(LibLogProviders.Serilog)
                .ConfigureApplication()
                .ConfigureDomain()
                .AddAspNetCore());
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
                Log.Logger.WithContext(msg.Source).Information(msg.Exception, msg.Message);
                break;
            case LogSeverity.Debug:
                Log.Logger.WithContext(msg.Source).Debug(msg.Exception, msg.Message);
                break;
            case LogSeverity.Verbose:
                Log.Logger.WithContext(msg.Source).Verbose(msg.Exception, msg.Message);
                break;
            case LogSeverity.Critical:
                Log.Logger.WithContext(msg.Source).Fatal(msg.Exception, msg.Message);
                break;
            case LogSeverity.Warning:
                Log.Logger.WithContext(msg.Source).Warning(msg.Exception, msg.Message);
                break;
            case LogSeverity.Error:
                Log.Error(msg.Exception, msg.Message);
                break;
        }

        return Task.CompletedTask;
    }
}