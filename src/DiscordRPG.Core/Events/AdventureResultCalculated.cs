﻿using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Progress;
using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class AdventureResultCalculated : DomainEvent
{
    public AdventureResultCalculated(Character character, bool hasFailed, Dungeon dungeon,
        ApplyExperienceResult experienceResult,
        ApplyItemsResult itemResult, ApplyWoundsResult woundsResult, List<Encounter> encounters,
        List<Wound> woundsBefore)
    {
        Character = character;
        HasFailed = hasFailed;
        Dungeon = dungeon;
        ExperienceResult = experienceResult;
        ItemResult = itemResult;
        WoundsResult = woundsResult;
        Encounters = encounters;
    }

    public ApplyExperienceResult ExperienceResult { get; private set; }
    public ApplyItemsResult ItemResult { get; private set; }
    public ApplyWoundsResult WoundsResult { get; private set; }
    public Character Character { get; private set; }
    public bool HasFailed { get; }
    public Dungeon Dungeon { get; private set; }
    public List<Encounter> Encounters { get; private set; }
}