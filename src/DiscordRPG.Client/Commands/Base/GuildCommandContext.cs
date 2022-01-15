using DiscordRPG.Application.Models;
using DiscordRPG.Common;

namespace DiscordRPG.Client.Commands.Base;

public class GuildCommandContext
{
    public GuildCommandContext(CharacterReadModel? character, ActivityReadModel? activity, DungeonReadModel? dungeon,
        GuildReadModel? guild, TransactionContext context)
    {
        Character = character;
        Activity = activity;
        Dungeon = dungeon;
        Guild = guild;
        Context = context;
    }

    public CharacterReadModel? Character { get; private set; }
    public ActivityReadModel Activity { get; private set; }
    public DungeonReadModel? Dungeon { get; private set; }
    public GuildReadModel? Guild { get; private set; }
    public TransactionContext Context { get; private set; }
}