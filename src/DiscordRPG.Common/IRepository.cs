using System.Linq.Expressions;

namespace DiscordRPG.Common;

public interface IRepository<T> where T : Entity
{
    Task SaveAsync(T entity, CancellationToken cancellationToken);

    Task UpdateAsync(T entity, CancellationToken cancellationToken);

    Task DeleteAsync(Identity id, CancellationToken cancellationToken);

    Task<T> GetAsync(Identity id, CancellationToken cancellationToken);

    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);
}