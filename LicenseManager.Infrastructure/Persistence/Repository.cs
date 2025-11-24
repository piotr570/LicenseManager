using LicenseManager.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Infrastructure.Persistence;

public class Repository<TEntity>(LicenseManagerDbContext context)
    : IRepository<TEntity> where TEntity : Entity
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public void Delete(TEntity entity) => _dbSet.Remove(entity);
}