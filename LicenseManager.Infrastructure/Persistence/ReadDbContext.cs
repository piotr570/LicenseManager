using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Infrastructure.Persistence;

public class ReadDbContext(LicenseManagerDbContext db) : IReadDbContext
{
    public IQueryable<T> Set<T>() where T : class
    {
        return db.Set<T>().AsNoTracking();
    }

    public async Task<T> GetEntityOrThrowAsync<T>(Guid id,
        CancellationToken cancellationToken = default)
        where T : Entity
    {
        var entity = await db.Set<T>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? throw new NotFoundException(typeof(T).Name);
    }
}

