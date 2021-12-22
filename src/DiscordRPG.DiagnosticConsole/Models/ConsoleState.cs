using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.ValueObjects;

namespace DiscordRPG.DiagnosticConsole.Models;

public class ConsoleState
{
    public Dungeon SelectedDungeon { get; set; }
    public Item SelectedItem { get; set; }
    public Character SelectedCharacter { get; set; }
    public Encounter SelectedEncounter { get; set; }
}