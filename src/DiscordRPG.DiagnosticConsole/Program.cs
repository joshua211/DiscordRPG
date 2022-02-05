// See https://aka.ms/new-console-template for more information

using System.Reflection;
using DiscordRPG.Application.Generators;
using DiscordRPG.DiagnosticConsole.Commands;
using DiscordRPG.DiagnosticConsole.Importers;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Settings;
using DiscordRPG.Domain.DomainServices;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog.Core;

namespace DiscordRPG.DiagnosticConsole
{
    class Program
    {
        public static async Task Main()
        {
            try
            {
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
                    .AddTransient<ItemGenerator>()
                    .AddTransient<IWorthCalculator, WorthCalculator>()
                    .AddTransient<DungeonGenerator>()
                    .AddTransient<NameGenerator>()
                    .AddTransient<IItemGenerator, ItemGenerator>()
                    .AddTransient<IRandomizer, Randomizer>()
                    .AddTransient<IWoundGenerator, WoundGenerator>()
                    .AddTransient<AspectGenerator>()
                    .AddTransient<IExperienceGenerator, ExperienceGenerator>()
                    .AddTransient<IEncounterGenerator, EncounterGenerator>()
                    .AddTransient<RarityGenerator>()
                    .AddTransient<AggregateImporter>((provider) => null)
                    .AddTransient<IAdventureResultService, AdventureResultService>()
                    .AddTransient<IExperienceCurve, ExperienceCurve>()
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
    }
}