// using System.Linq.Expressions;
// using LicenseManager.Application.HostedServices;
// using LicenseManager.Application.HostedServices.Interfaces;
// using LicenseManager.Domain.Licenses;
// using LicenseManager.Domain.Licenses.Enums;
// using LicenseManager.Domain.Users;
// using LicenseManager.SharedKernel.Abstractions;
// using LicenseManager.SharedKernel.Common;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Xunit;
//
// namespace LicenseManager.Tests.Application.HostedServices;
//
// public class LicenseReservationCleanupServiceTests
// {
//     private readonly Mock<IRepository<License>> _licenseRepositoryMock;
//     private readonly Mock<IUnitOfWork> _unitOfWorkMock;
//     private readonly Mock<ILogger<LicenseReservationCleanupService>> _loggerMock;
//     private readonly ILicenseReservationCleanupService _cleanupService;
//
//     public LicenseReservationCleanupServiceTests()
//     {
//         _licenseRepositoryMock = new Mock<IRepository<License>>();
//         _loggerMock = new Mock<ILogger<LicenseReservationCleanupService>>();
//         _unitOfWorkMock = new Mock<IUnitOfWork>();
//
//         _cleanupService = new LicenseReservationCleanupService(
//             _licenseRepositoryMock.Object,
//             _unitOfWorkMock.Object,
//             _loggerMock.Object
//         );
//     }
//
//     [Fact]
//     public async Task CleanupAsync_ShouldInvokeCleanupOnExpiredReservations()
//     {
//         // Arrange
//         SystemClock.Set(SystemClock.Now.AddDays(-100));
//
//         var terms = new LicenseTerms(
//             licenseId: Guid.NewGuid(),
//             type: LicenseType.Single,
//             mode: LicenseMode.TimeBased,
//             maxUsers: 1,
//             isRenewable: true,
//             expirationDate: SystemClock.Now.AddMonths(1), 
//             renewalDate: SystemClock.Now.AddMonths(6),
//             usageLimit: null
//         );
//     
//         var license = License.Create(
//             Guid.NewGuid(), 
//             "Key123", 
//             "VendorA", 
//             "LicenseA", 
//             terms
//         );
//
//         var user = new User("User1", "", DepartmentType.Board);
//
//         license.Reserve(user);
//
//         var licenses = new List<License> { license };
//
//         _licenseRepositoryMock
//             .Setup(repo => repo.GetAllIncludingAsync(
//                 null,
//                 It.IsAny<Func<IQueryable<License>, IQueryable<License>>[]>()))
//             .ReturnsAsync(licenses);
//         SystemClock.Reset();
//         
//         // Act
//         await _cleanupService.CleanupExpiredReservationsAsync(CancellationToken.None);
//
//         // Assert
//         Assert.Empty(license.Reservations); 
//         _licenseRepositoryMock.Verify(repo => repo.Update(It.IsAny<License>()), Times.Once);
//         _unitOfWorkMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task CleanupAsync_ShouldNotFailWhenNoLicensesExist()
//     {
//         // Arrange
//         _licenseRepositoryMock
//             .Setup(repo => repo.GetAllIncludingAsync(
//                 It.IsAny<Expression<Func<License, bool>>>(),
//                 It.IsAny<Func<IQueryable<License>, IQueryable<License>>[]>()))
//             .ReturnsAsync(new List<License>());
//
//         // Act
//         await _cleanupService.CleanupExpiredReservationsAsync(CancellationToken.None);
//
//         // Assert
//         _licenseRepositoryMock.Verify(repo => repo.Update(It.IsAny<License>()), Times.Never);
//     }
// }