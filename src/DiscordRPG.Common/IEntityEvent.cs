using EventFlow.Core;

namespace DiscordRPG.Common;

public interface IEntityEvent<T> where T : IIdentity
{
    T EntityId { get; }
}