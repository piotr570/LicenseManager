namespace LicenseManager.SharedKernel.Abstractions;

public interface IReadDbContext
{
    IQueryable<TEntity> Set<TEntity>() where TEntity : class;

    Task<TEntity> GetEntityOrThrowAsync<TEntity>(Guid id,
        CancellationToken cancellationToken = default)
        where TEntity : Entity;
}

