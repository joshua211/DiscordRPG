﻿using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Repositories;

public interface ICharacterRepository
{
    Task<Character> GetCharacterAsync(ulong userId, CancellationToken token = default);
    Task<Character> SaveCharacterAsync(Character character, CancellationToken token = default);
}