using DiscordRPG.Common.Extensions;
using MediatR;
using Serilog;

namespace DiscordRPG.Common;

public abstract class QueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected readonly ILogger logger;

    protected QueryHandler(ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}