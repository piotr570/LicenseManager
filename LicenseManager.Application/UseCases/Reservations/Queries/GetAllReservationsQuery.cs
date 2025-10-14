using MediatR;
using LicenseManager.Application.UseCases.Reservations.Models;

namespace LicenseManager.Application.UseCases.Reservations.Queries;

public class GetAllReservationsQuery : IRequest<List<LicenseReservationDto>>
{
}

