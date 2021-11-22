﻿using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class Wound
{
    [BsonConstructor]
    public Wound(string description, int damageValue)
    {
        Description = description;
        DamageValue = damageValue;
    }

    public string Description { get; private set; }
    public int DamageValue { get; private set; }
}