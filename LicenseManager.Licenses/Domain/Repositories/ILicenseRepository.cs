using LicenseManager.Licenses.Domain.Aggregates;

namespace LicenseManager.Licenses.Domain.Repositories;

public interface ILicenseRepository
{
    Task<License?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<License>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<License?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task AddAsync(License license, CancellationToken cancellationToken = default);
    Task UpdateAsync(License license, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}