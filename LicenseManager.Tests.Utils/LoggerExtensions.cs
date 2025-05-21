using Microsoft.Extensions.Logging;
using Moq;

namespace LicenseManager.Tests.Utils;

public static class LoggerExtensions
{
    public static void VerifyLogContains<T>(
        this Mock<ILogger<T>> loggerMock,
        LogLevel level,
        string contains,
        Times times)
    {
        loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains(contains)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
}