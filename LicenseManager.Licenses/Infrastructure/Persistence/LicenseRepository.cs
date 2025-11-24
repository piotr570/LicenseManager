using LicenseManager.Licenses.Domain.Aggregates;
using LicenseManager.Licenses.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Licenses.Infrastructure.Persistence;

public sealed class LicenseRepository(LicensesDbContext context) : ILicenseRepository
{
    public async Task<License?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Licenses
            .Include(x => x.Assignments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<License>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Licenses
            .Include(x => x.Assignments)
            .ToListAsync(cancellationToken);
    }

    public async Task<License?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await context.Licenses
            .Include(x => x.Assignments)
            .FirstOrDefaultAsync(x => x.Key == key, cancellationToken);
    }

    public async Task AddAsync(License license, CancellationToken cancellationToken = default)
    {
        await context.Licenses.AddAsync(license, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(License license, CancellationToken cancellationToken = default)
    {
        context.Licenses.Update(license);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var license = await GetByIdAsync(id, cancellationToken);
        if (license != null)
        {
            context.Licenses.Remove(license);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}