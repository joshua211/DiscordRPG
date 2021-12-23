using MediatR;

namespace DiscordRPG.Core.Commands.Shops;

public class DeleteShopCommand : IRequest<ExecutionResult>
{
    public Identity ShopId { get; private set; }
}