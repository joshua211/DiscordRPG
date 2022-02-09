﻿using Discord;
using DiscordRPG.Application.Models;
using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Client.Dialogs;

public class EquipDialog : Dialog, IShareableDialog, IPageableDialog
{
    public EquipDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public CharacterReadModel Character { get; set; }
    public Item? CurrentItem { get; set; }
    public EquipmentPosition? Position { get; set; }
    public GuildId GuildId { get; set; }
    public int CurrentPage { get; set; } = 1;
    public Embed ShareableEmbed { get; set; }
}