﻿using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Commands;

public class CreateActivityCommand : Command
{
    public CreateActivityCommand(Activity activity)
    {
        Activity = activity;
    }

    public Activity Activity { get; private set; }
}