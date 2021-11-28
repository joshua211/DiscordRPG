﻿using DiscordRPG.Application.Services;
using DiscordRPG.Common;

namespace DiscordRPG.Application.Interfaces.Services;

public interface IDungeonService
{
    Task<Result<Dungeon>> CreateDungeonAsync(DiscordId serverId, DiscordId threadId, uint charLevel,
        TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result<Dungeon>> GetDungeonFromChannelIdAsync(DiscordId channelId, TransactionContext parentContext = null,
        CancellationToken token = default);

    Task<Result> GetDungeonAdventureResultAsync(Identity chadId, DiscordId threadId,
        TransactionContext parentContext = null,
        CancellationToken token = default);
}