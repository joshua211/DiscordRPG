// See https://aka.ms/new-console-template for more information

using System.Reflection;
using DiscordRPG.Application.Data;
using DiscordRPG.Application.Generators;
using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.DomainServices.Progress;
using DiscordRPG.Core.Events;
using DiscordRPG.Core.ValueObjects;
using DiscordRPG.DiagnosticConsole.Commands;
using DiscordRPG.DiagnosticConsole.Importers;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Serilog.Core;

namespace DiscordRPG.DiagnosticConsole
{
    class Program
    {
        public static async Task Main()
        {
            BsonClassMap.RegisterClassMap<DungeonCreated>();
            BsonClassMap.RegisterClassMap<DungeonDeleted>();
            BsonClassMap.RegisterClassMap<CharacterDied>();
            BsonClassMap.RegisterClassMap<ActivityCreated>();
            BsonClassMap.RegisterClassMap<ActivityDeleted>();
            BsonClassMap.RegisterClassMap<AdventureResultCalculated>();
            BsonClassMap.RegisterClassMap<Equipment>();
            BsonClassMap.RegisterClassMap<Weapon>();

            var services = new ServiceCollection();
            var config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.Configure<DatabaseSettings>(config.GetSection(nameof(DatabaseSettings)));
            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            var dbSettings = services.BuildServiceProvider().GetService<IDatabaseSettings>();
            var mongoClient = new MongoClient(dbSettings.ConnectionString);
            services
                .AddSingleton<IMongoClient>(mongoClient)
                .AddSingleton<ILiveLogImporter, LiveLogImporter>()
                .AddTransient<IHistoryLogImporter, HistoryLogImporter>()
                .AddTransient<IEventImporter, EventImporter>()
                .AddTransient<IItemGenerator, ItemGenerator>()
                .AddTransient<IDungeonGenerator, DungeonGenerator>()
                .AddTransient<INameGenerator, NameGenerator>()
                .AddTransient<IItemGenerator, ItemGenerator>()
                .AddTransient<IWoundGenerator, WoundGenerator>()
                .AddTransient<IAspectGenerator, AspectGenerator>()
                .AddTransient<IExperienceGenerator, ExperienceGenerator>()
                .AddTransient<IEncounterGenerator, EncounterGenerator>()
                .AddTransient<IRarityGenerator, RarityGenerator>()
                .AddTransient<IAdventureResultService, AdventureResultService>()
                .AddTransient<IExperienceCurve, ExperienceCurve>()
                .AddSingleton<IClassService, Classes>()
                .AddSingleton<IRaceService, Races>()
                .AddSingleton<INameGenerator, NameGenerator>()
                .AddSingleton<ConsoleState>()
                .AddSingleton(Logger.None);

            var commandTypes = Assembly.GetEntryAssembly().ExportedTypes
                .Where(t => t.GetInterfaces().Contains(typeof(ICommand)));

            foreach (var t in commandTypes)
            {
                services.AddTransient(typeof(ICommand), t);
            }

            var provider = services.BuildServiceProvider();
            var commands = provider.GetServices<ICommand>();

            var entry = commands.First(c => c.CommandName == "entry");
            await entry.ExecuteAsync(commands);
        }
    }
}