using Discord;
using Discord.Commands;
using DiscordRPG.Application.Generators;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Worker;
using DiscordRPG.Common;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Activity.Enums;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Enums;
using Serilog;

namespace DiscordRPG.Client.Modules;

public class DebugModule
{
    [RequireOwner]
    [Group("rpg debug")]
    public class Test : ModuleBase<SocketCommandContext>
    {
        private readonly IGuildService guildService;
        private readonly ILogger logger;
        private readonly RarityGenerator rarityGenerator;
        private readonly IShopService shopService;
        private readonly ShopWorker shopWorker;

        public Test(RarityGenerator rarityGenerator, ILogger logger, ShopWorker shopWorker, IGuildService guildService,
            IShopService shopService)
        {
            this.rarityGenerator = rarityGenerator;
            this.shopWorker = shopWorker;
            this.guildService = guildService;
            this.shopService = shopService;
            this.logger = logger.WithContext<DebugModule>();
        }

        [Command("force-shopCreation")]
        public async Task ForceShopCreation()
        {
            var context = TransactionContext.New();
            var guild = await guildService.GetGuildAsync(new GuildId(Context.Guild.Id.ToString()), context);
            if (!guild.WasSuccessful)
                logger.Here().Warning("No guild found");

            var createShopResult =
                await shopService.CreateGuildShopAsync(new GuildId(guild.Value.Id), context);
            if (!createShopResult.WasSuccessful)
            {
                logger.Here().Error(createShopResult.ErrorMessage);
                await ReplyAsync("Something went wrong: " + createShopResult.ErrorMessage);
                return;
            }

            await ReplyAsync("Created shop");
        }

        [Command("force-shopUpdate")]
        public async Task ForceShopUpdateAsync()
        {
            var context = TransactionContext.New();
            try
            {
                logger.Context(context).Debug("Forcing shop update for guild with DiscordID {Id}", Context.Guild.Id);

                var guildResult =
                    await guildService.GetGuildAsync(new GuildId(Context.Guild.Id.ToString()), context);
                if (!guildResult.WasSuccessful)
                {
                    logger.Here().Warning("No guild found to force shop update");
                    return;
                }

                await shopWorker.UpdateGuildShopAsync(guildResult.Value, context);
            }
            catch (Exception e)
            {
                logger.Context(context).Error(e, "Failed force shop update ");
            }
        }

        [Command("activity-rarityDistribution")]
        public async Task TestRarityDistribution(int luck)
        {
            logger.Here().Debug("Testing rarity distribution");
            var results = new Dictionary<ActivityDuration, Dictionary<Rarity, int>>();
            results.Add(ActivityDuration.Quick, new Dictionary<Rarity, int>());
            results.Add(ActivityDuration.Short, new Dictionary<Rarity, int>());
            results.Add(ActivityDuration.Medium, new Dictionary<Rarity, int>());
            results.Add(ActivityDuration.Long, new Dictionary<Rarity, int>());
            results.Add(ActivityDuration.ExtraLong, new Dictionary<Rarity, int>());

            try
            {
                var durations = Enum.GetValues(typeof(ActivityDuration));
                var runs = 1000000;
                foreach (ActivityDuration duration in durations)
                {
                    logger.Here().Debug("Testing {Duration}", duration.ToString());
                    var dic = results[duration];
                    foreach (Rarity rarity in Enum.GetValues(typeof(Rarity)))
                        dic[rarity] = 0;

                    for (int i = 0; i < runs; i++)
                    {
                        var rarity = rarityGenerator.GenerateRarityFromActivityDuration(duration, luck);
                        dic[rarity]++;
                    }
                }

                var embeds = new List<Embed>();
                foreach (ActivityDuration duration in durations)
                {
                    var embedBuilder = new EmbedBuilder();
                    embedBuilder
                        .WithTitle($"{duration.ToString()} distribution results")
                        .WithDescription($"{runs} runs");
                    foreach (Rarity rarity in Enum.GetValues(typeof(Rarity)))
                    {
                        var val = results[duration][rarity];
                        embedBuilder.AddField(rarity.ToString(),
                            $"{val} ({(((double) val / (double) runs) * 100):00.0000}%)", true);
                    }

                    embeds.Add(embedBuilder.Build());
                }

                await ReplyAsync(embeds: embeds.ToArray());
            }
            catch (Exception e)
            {
                logger.Here().Error(e, "Failed to test");
                await ReplyAsync(e.Message);
            }
        }
    }
}