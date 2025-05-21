using LicenseManager.Application;
using LicenseManager.Application.Abstraction;
using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Application.UseCases.Licenses.Handlers;
using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common;
using LicenseManager.Domain.Common.Exceptions;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Licenses.Factories.Creation;
using LicenseManager.Infrastructure;
using LicenseManager.Infrastructure.Persistence;
using LicenseManager.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Xunit;

namespace LicenseManager.Domain.IntegrationTests.Licenses;

public class AddLicenseCommandHandlerIntegrationTests : IAsyncLifetime
{
    private ServiceProvider _serviceProvider;
    private readonly ServiceCollection _serviceCollection = [];
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly ILicenseFactory _factory = new LicenseFactory(); 

    public AddLicenseCommandHandlerIntegrationTests()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("postgres")
            .WithUsername("root")
            .WithPassword("Admin123!")
            .Build();
    }
    
    private void ConfigureServices()
    {
        _serviceCollection.AddDomainLayer();
        _serviceCollection.AddApplicationLayer();
        _serviceCollection.AddScoped<IUnitOfWork, UnitOfWork<LicenseManagerDbContext>>();
        _serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>)); 
        _serviceCollection.AddLogging(configure => configure.AddConsole());
        _serviceCollection.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        _serviceCollection.AddLogging(configure => configure.AddConsole());
        
        _serviceProvider = _serviceCollection.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        _serviceCollection.AddDbContext<LicenseManagerDbContext>(options =>
            options.UseNpgsql(_postgresContainer.GetConnectionString()));
        ConfigureServices();
        _serviceProvider = _serviceCollection.BuildServiceProvider();

        // Ensure database is created within a scope
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LicenseManagerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _serviceProvider.DisposeAsync();
        await _postgresContainer.StopAsync();
    }

    [Fact]
    public async Task Handle_ShouldAddLicenseToDatabase_WhenValidCommand()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LicenseManagerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        
        var command = new AddLicenseCommand(
            "TEST-KEY-123",
            "Test Vendor",
            "Test License",
            LicenseType.Single,
            LicenseMode.TimeBased,
            true,
            1,
            null,
            SystemClock.Now.AddYears(1),
            SystemClock.Now.AddYears(1).AddDays(-30)
        );

        var handler = CreateHandler(scope);

        // Act
        var licenseId = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, licenseId);
        var license = await dbContext.Licenses.FindAsync(licenseId);
        Assert.NotNull(license);
        Assert.Equal(command.Key, license.Key);
        Assert.Equal(command.Name, license.Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnDifferentIds_WhenAddingMultipleLicenses()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LicenseManagerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        
        var command1 = new AddLicenseCommand(
            "TEST-KEY-123",
            "Test Vendor",
            "Test License",
            LicenseType.Single,
            LicenseMode.TimeBased,
            true,
            1,
            null,
            SystemClock.Now.AddYears(1),
            SystemClock.Now.AddYears(1).AddDays(-30)
        );
    
        var command2 = new AddLicenseCommand(
            "TEST-KEY-321",
            "Test Vendor",
            "Test License",
            LicenseType.Single,
            LicenseMode.TimeBased,
            true,
            1,
            null,
            SystemClock.Now.AddYears(1),
            SystemClock.Now.AddYears(1).AddDays(-30)
        );

        var handler = CreateHandler(scope);
    
        // Act
        var licenseId1 = await handler.Handle(command1, CancellationToken.None);
        var licenseId2 = await handler.Handle(command2, CancellationToken.None);
    
        // Assert
        Assert.NotEqual(licenseId1, licenseId2);
        var licensesCount = dbContext.Licenses.Count();
        Assert.Equal(2, licensesCount);
    }
    
    [Fact]
    public async Task Handle_ShouldThrow_WhenLicenseKeyAlreadyExists()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LicenseManagerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        
        const string existingKey = "DUPLICATE-KEY";
        
        // Add initial license
        var initialLicense = new LicenseFactory().Create(
            existingKey,
            "Test Vendor",
            "Test License",
            LicenseType.Single,
            LicenseMode.TimeBased,
            null,
            true,
            SystemClock.Now.AddYears(1),
            SystemClock.Now.AddYears(2),
            null
        );
        
        await dbContext.AddAsync(initialLicense);
        await dbContext.SaveChangesAsync(CancellationToken.None);
    
        var duplicateCommand = new AddLicenseCommand(
            existingKey,
            "Test Vendor",
            "Test License",
            LicenseType.Single,
            LicenseMode.TimeBased,
            true,
            1,
            null,
            SystemClock.Now.AddYears(1),
            SystemClock.Now.AddYears(2)
        );
    
        var handler = CreateHandler(scope);
    
        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => 
            handler.Handle(duplicateCommand, CancellationToken.None));
    }
    
    private AddLicenseCommandHandler CreateHandler(IServiceScope scope)
    {
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<License>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AddLicenseCommandHandler>>();  

        return new AddLicenseCommandHandler(repository, unitOfWork, _factory, logger);
    }
}
