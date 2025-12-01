using MediatR;
using LicenseManager.Licenses.Application.DTOs;

namespace LicenseManager.Licenses.Application.Commands;

public record CreateLicenseCommand(
    string Key,
    string Name,
    string Vendor,
    LicenseTermsDto Terms,
    Guid? DepartmentId) : IRequest<Guid>;

public record AssignLicenseCommand(
    Guid LicenseId,
    Guid UserId) : IRequest;

public record ReserveLicenseCommand(
    Guid LicenseId,
    Guid UserId) : IRequest;

public record InvokeLicenseCommand(
    Guid LicenseId,
    Guid UserId) : IRequest;

public record DeleteLicenseCommand(
    Guid LicenseId) : IRequest;

public record RemoveAssignmentCommand(
    Guid AssignmentId,
    Guid LicenseId) : IRequest;