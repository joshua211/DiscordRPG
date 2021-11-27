using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetDungeonAdventureQuery : Query<DungeonResult>
{
    public GetDungeonAdventureQuery(Character character, Dungeon dungeon)
    {
        Character = character;
        Dungeon = dungeon;
    }

    public Character Character { get; private set; }
    public Dungeon Dungeon { get; private set; }
}