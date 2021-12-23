using Discord.Commands;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Worker;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.ValueObjects;
using Serilog;

namespace DiscordRPG.Client.Modules;

public class DebugModule
{
    [RequireOwner]
    [Group("rpg test")]
    public class Test : ModuleBase<SocketCommandContext>
    {
        private readonly IGuildService guildService;
        private readonly ILogger logger;
        private readonly IRarityGenerator rarityGenerator;
        private readonly ShopWorker shopWorker;

        public Test(IRarityGenerator rarityGenerator, ILogger logger, ShopWorker shopWorker, IGuildService guildService)
        {
            this.rarityGenerator = rarityGenerator;
            this.shopWorker = shopWorker;
            this.guildService = guildService;
            this.logger = logger.WithContext<DebugModule>();
        }

        [Command("force-shopUpdate")]
        public async Task ForceShopUpdateAsync()
        {
            try
            {
                logger.Here().Debug("Forcing shop update for guild with DiscordID {Id}", Context.Guild.Id);

                var guildResult =
                    await guildService.GetGuildWithDiscordIdAsync(new DiscordId(Context.Guild.Id.ToString()));
                if (!guildResult.WasSuccessful)
                {
                    logger.Here().Warning("No guild found to force shop update");
                    return;
                }

                await shopWorker.UpdateGuildShopAsync(guildResult.Value);
            }
            catch (Exception e)
            {
                logger.Here().Error(e, "Failed force shop update ");
            }
        }
    }
}