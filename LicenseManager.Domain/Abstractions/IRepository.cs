using System.Linq.Expressions;
using LicenseManager.Domain.Common;

namespace LicenseManager.Domain.Abstractions;

public interface IRepository<TEntity>
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TEntity?> GetByIdIncludingAsync(Guid id, params Expression<Func<TEntity, object>>[] includeProperties);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllIncludingAsync(Expression<Func<TEntity, bool>>? filter = null,
        params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includeProperties);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}