using MediatR;

namespace DiscordRPG.Common;

public abstract class Command : IRequest<ExecutionResult>
{
}