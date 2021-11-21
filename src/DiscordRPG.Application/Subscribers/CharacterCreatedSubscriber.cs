using DiscordRPG.Common;
using DiscordRPG.Core.Events;
using Serilog;

namespace DiscordRPG.Application.Subscribers;

public class CharacterCreatedSubscriber : EventSubscriber<CharacterCreated>
{
    private readonly ILogger logger;

    public CharacterCreatedSubscriber(ILogger logger)
    {
        this.logger = logger;
    }

    public override Task Handle(CharacterCreated domainEvent, CancellationToken cancellationToken)
    {
        logger.Information("Created character {Name}", domainEvent.Character.CharacterName);

        return Task.CompletedTask;
    }
}