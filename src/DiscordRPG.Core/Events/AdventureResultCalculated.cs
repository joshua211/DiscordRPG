using DiscordRPG.Core.DomainServices.Progress;
using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class AdventureResultCalculated : DomainEvent
{
    public AdventureResultCalculated(Character character, Dungeon dungeon, ApplyExperienceResult experienceResult,
        ApplyItemsResult itemResult, ApplyWoundsResult woundsResult)
    {
        Character = character;
        Dungeon = dungeon;
        ExperienceResult = experienceResult;
        ItemResult = itemResult;
        WoundsResult = woundsResult;
    }

    public ApplyExperienceResult ExperienceResult { get; private set; }
    public ApplyItemsResult ItemResult { get; private set; }
    public ApplyWoundsResult WoundsResult { get; private set; }
    public Character Character { get; private set; }
    public Dungeon Dungeon { get; private set; }
}