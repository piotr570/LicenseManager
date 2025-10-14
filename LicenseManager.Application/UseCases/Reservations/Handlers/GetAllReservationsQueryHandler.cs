using LicenseManager.Application.UseCases.Reservations.Models;
using LicenseManager.Application.UseCases.Reservations.Queries;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Users;
using MediatR;
using LicenseReservation = LicenseManager.Domain.Reservations.LicenseReservation;

namespace LicenseManager.Application.UseCases.Reservations.Handlers;

public class GetAllReservationsQueryHandler(
    IRepository<LicenseReservation> reservationRepository,
    IRepository<License> licenseRepository,
    IRepository<User> userRepository)
    : IRequestHandler<GetAllReservationsQuery, List<LicenseReservationDto>>
{
    public async Task<List<LicenseReservationDto>> Handle(GetAllReservationsQuery request, CancellationToken cancellationToken)
    {
        var reservations = await reservationRepository.GetAllAsync(cancellationToken);
        var licenses = await licenseRepository.GetAllAsync(cancellationToken);
        var users = await userRepository.GetAllAsync(cancellationToken);

        var licenseDict = licenses.ToDictionary(l => l.Id);
        var userDict = users.ToDictionary(u => u.Id);

        var result = reservations.Select(r => new LicenseReservationDto
        {
            ReservationId = r.Id,
            LicenseId = r.LicenseId,
            LicenseName = licenseDict.TryGetValue(r.LicenseId, out var license) ? license.Name : string.Empty,
            UserId = r.UserId,
            UserName = userDict.TryGetValue(r.UserId, out var user) ? user.Name : string.Empty,
            ReservedAt = r.ReservedAt,
            ExpirationDate = r.ExpirationDate,
            State = r.State.ToString()
        }).ToList();

        return result;
    }
}


