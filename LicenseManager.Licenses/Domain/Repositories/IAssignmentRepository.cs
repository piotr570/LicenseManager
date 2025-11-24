using LicenseManager.Licenses.Domain.Aggregates;

namespace LicenseManager.Licenses.Domain.Repositories;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assignment>> GetByLicenseIdAsync(Guid licenseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assignment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}