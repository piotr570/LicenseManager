namespace LicenseManager.Application.UseCases.Reservations.Models;

public class LicenseReservationDto
{
    public Guid ReservationId { get; set; }
    public Guid LicenseId { get; set; }
    public string LicenseName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime ReservedAt { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string State { get; set; } = string.Empty;
}