﻿using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands;

public class CreateGuildCommandHandler : CommandHandler<CreateGuildCommand>
{
    private readonly IGuildRepository repository;

    public CreateGuildCommandHandler(IMediator mediator, IGuildRepository repository) : base(mediator)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(CreateGuildCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await repository.SaveGuildAsync(request.Guild, cancellationToken);
            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}