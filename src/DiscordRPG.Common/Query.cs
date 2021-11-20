using MediatR;

namespace DiscordRPG.Common;

public abstract class Query<T> : IRequest<T>
{
}