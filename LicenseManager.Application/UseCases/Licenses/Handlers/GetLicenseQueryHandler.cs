using LicenseManager.Application.Common.Exceptions;
using LicenseManager.Application.UseCases.Licenses.Models;
using LicenseManager.Application.UseCases.Licenses.Queries;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.Domain.Licenses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class GetLicenseQueryHandler(ILogger<GetLicenseQueryHandler> logger,
    IRepository<License> repository)
    : IRequestHandler<GetLicenseByIdQuery, LicenseDto>
{
    private readonly ILogger<GetLicenseQueryHandler> _logger = logger;

    public async Task<LicenseDto> Handle(GetLicenseByIdQuery query, CancellationToken cancellationToken)
    {
        var license = await repository.GetByIdIncludingAsync(query.LicenseId, 
                          x => x.Terms,
                          x => x.Assignments) 
                      ?? throw new NotFoundException(nameof(License), query.LicenseId);
        
        return new LicenseDto(license);
    }
}