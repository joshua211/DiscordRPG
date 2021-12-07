using Discord;
using Discord.Commands;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.Enums;
using Serilog;

namespace DiscordRPG.Client.Modules;

public class DebugModule
{
    [RequireOwner]
    [Group("rpg test")]
    public class Test : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger logger;
        private readonly IRarityGenerator rarityGenerator;

        public Test(IRarityGenerator rarityGenerator, ILogger logger)
        {
            this.rarityGenerator = rarityGenerator;
            this.logger = logger.WithContext<DebugModule>();
        }

        [Command("activity-rarityDistribution")]
        public async Task TestRarityDistribution()
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
                        var rarity = rarityGenerator.GenerateRarityFromActivityDuration(duration);
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