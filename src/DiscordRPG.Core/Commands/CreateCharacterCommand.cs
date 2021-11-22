﻿using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands;

public class CreateCharacterCommand : Command
{
    public CreateCharacterCommand(Character character)
    {
        Character = character;
    }

    public Character Character { get; private set; }
}