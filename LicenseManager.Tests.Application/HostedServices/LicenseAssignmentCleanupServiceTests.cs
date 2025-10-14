// using System.Linq.Expressions;
// using LicenseManager.Application.HostedServices;
// using LicenseManager.Domain.Licenses;
// using LicenseManager.Domain.Licenses.Abstractions;
// using LicenseManager.Domain.Licenses.Policies;
// using LicenseManager.Domain.Licenses.Services;
// using LicenseManager.SharedKernel.Abstractions;
// using LicenseManager.SharedKernel.Common;
// using LicenseManager.Tests.Utils;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Xunit;
//
// namespace LicenseManager.Tests.Application.HostedServices;
//
// public class LicenseAssignmentCleanupServiceTests : TestBase
// {
//     private readonly LicenseAssignmentCleanupService _cleanupService;
//     private readonly Mock<IRepository<License>> _repositoryMock;
//     private readonly Mock<IUnitOfWork> _unitOfWorkMock;
//     private readonly Mock<ILogger<LicenseAssignmentCleanupService>> _loggerMock;
//
//     public LicenseAssignmentCleanupServiceTests()
//     {
//         _repositoryMock = new Mock<IRepository<License>>();
//         _unitOfWorkMock = new Mock<IUnitOfWork>();
//         _loggerMock = new Mock<ILogger<LicenseAssignmentCleanupService>>();
//         ILicenseAssignmentCleanupDomainService cleanupDomainServiceMock = new LicenseAssignmentCleanupDomainService();
//
//         _cleanupService = new LicenseAssignmentCleanupService(
//             _repositoryMock.Object,
//             _unitOfWorkMock.Object,
//             _loggerMock.Object,
//             cleanupDomainServiceMock
//         );
//     }
//
//     [Fact]
//     public async Task CleanupNotUsedAssignmentsAsync_ShouldCleanupNotUsedAssignments()
//     {
//         // Arrange
//         var now = SystemClock.Now;
//         var licenses = new List<License>()
//         {
//             CreateLicense(),
//             CreateLicense()
//         };
//
//         foreach (var license in licenses)
//         {
//             license.AssignUser(CreateUser(), new ServerLicenseAssignmentPolicy(), new List<IBusinessRule>(), now);
//             license.Assignments.First().MarkAsNotUsed();
//         }
//
//         _repositoryMock
//             .Setup(r => r.GetAllIncludingAsync(
//                 It.IsAny<Expression<Func<License, bool>>>(),
//                 It.IsAny<Func<IQueryable<License>, IQueryable<License>>[]>()))
//             .ReturnsAsync(licenses);
//         
//         // Act
//         await _cleanupService.CleanupNotUsedAssignmentsAsync(CancellationToken.None);
//
//         // Assert
//         Assert.All(licenses, l => Assert.Empty(l.Assignments));
//         _repositoryMock.Verify(r => r.GetAllIncludingAsync(It.IsAny<Expression<Func<License, bool>>>(),
//             It.IsAny<Func<IQueryable<License>, IQueryable<License>>[]>()), Times.Once);
//         _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//         _loggerMock.VerifyLogContains(LogLevel.Information, "Running Not Used License Assignments Cleanup Service", Times.Once());
//         _loggerMock.VerifyLogContains(LogLevel.Information, $"Cleaned up unused assignments for {licenses[0].Id} license.", Times.Once());
//         _loggerMock.VerifyLogContains(LogLevel.Information, $"Cleaned up unused assignments for {licenses[1].Id} license.", Times.Once());
//     }
//
//     [Fact]
//     public async Task CleanupNotUsedAssignmentsAsync_ShouldDoNothingIfNoLicensesExist()
//     {
//         // Arrange
//         _repositoryMock
//             .Setup(r => r.GetAllIncludingAsync(
//                 It.IsAny<Expression<Func<License, bool>>>(),
//                 It.IsAny<Func<IQueryable<License>, IQueryable<License>>[]>()))
//             .ReturnsAsync(new List<License>()); 
//     
//         // Act
//         await _cleanupService.CleanupNotUsedAssignmentsAsync(CancellationToken.None);
//     
//         // Assert
//         _repositoryMock.Verify(r => r.Update(It.IsAny<License>()), Times.Never);
//         _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never); 
//     }
// }