using LicenseManager.Licenses.Domain.Aggregates;
using LicenseManager.Licenses.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Licenses.Infrastructure.Persistence;

public sealed class AssignmentRepository(LicensesDbContext context) : IAssignmentRepository
{
    public async Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Assignments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Assignment>> GetByLicenseIdAsync(Guid licenseId, CancellationToken cancellationToken = default)
    {
        return await context.Assignments
            .Where(a => a.LicenseId == licenseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Assignment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Assignments
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        await context.Assignments.AddAsync(assignment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        context.Assignments.Update(assignment);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await GetByIdAsync(id, cancellationToken);
        if (assignment != null)
        {
            context.Assignments.Remove(assignment);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}