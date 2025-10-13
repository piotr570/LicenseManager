using System.Linq.Expressions;
using LicenseManager.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Infrastructure.Persistence;

public class Repository<TEntity>(LicenseManagerDbContext context) : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<TEntity?> GetByIdIncludingAsync(Guid id, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = ApplyIncludes(_dbSet.AsQueryable(), includeProperties);
        return await query.SingleOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllIncludingAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includeProperties)
    {
        var query = _dbSet.AsQueryable().AsNoTracking();

        query = ApplyFilters(query, filter);

        foreach (var include in includeProperties)
        {
            query = include(query);
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
    
    private IQueryable<TEntity> ApplyIncludes(
        IQueryable<TEntity> query, 
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return query;
    }

    private IQueryable<TEntity> ApplyFilters(
        IQueryable<TEntity> query, 
        Expression<Func<TEntity, bool>>? filter)
    {
        return filter != null ? query.Where(filter) : query;
    }
}