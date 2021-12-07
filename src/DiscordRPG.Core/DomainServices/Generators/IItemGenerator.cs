using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.DomainServices.Generators;

public interface IItemGenerator
{
    IEnumerable<Item> GenerateItems(Dungeon dungeon);
}