using LicenseManager.Application;
using LicenseManager.Application.Abstraction;
using LicenseManager.Application.Common.Exceptions;
using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Application.UseCases.Licenses.Handlers;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Abstractions;
using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Licenses.Factories.Assignment;
using LicenseManager.Domain.Users;
using LicenseManager.Infrastructure;
using LicenseManager.Infrastructure.Persistence;
using LicenseManager.Infrastructure.Services;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;
using LicenseManager.SharedKernel.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Xunit;

namespace LicenseManager.Domain.IntegrationTests.Licenses;

public class AssignLicenseCommandHandlerIntegrationTests : IAsyncLifetime
{
    private readonly ServiceCollection _serviceCollection = [];
    private readonly PostgreSqlContainer _postgresContainer;
    private ServiceProvider _serviceProvider;

    public AssignLicenseCommandHandlerIntegrationTests()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("postgres")
            .WithUsername("root")
            .WithPassword("Admin123!")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        _serviceCollection.AddDbContext<LicenseManagerDbContext>(options =>
            options.UseNpgsql(_postgresContainer.GetConnectionString()));
        ConfigureServices();
        _serviceProvider = _serviceCollection.BuildServiceProvider();

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LicenseManagerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
    
    private void ConfigureServices()
    {
        _serviceCollection.AddDomainLayer();
        _serviceCollection.AddApplicationLayer();
        _serviceCollection.AddLogging(configure => configure.AddConsole());
        _serviceCollection.AddScoped<IUnitOfWork, UnitOfWork<LicenseManagerDbContext>>();
        _serviceCollection.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        _serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>)); 
        
        _serviceProvider = _serviceCollection.BuildServiceProvider();
    }

    public async Task DisposeAsync()
    {
        await _serviceProvider.DisposeAsync();
        await _postgresContainer.StopAsync();
    }

    [Fact]
    public async Task Handle_ShouldAssignLicenseForUser_WhenCommandIsValid()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LicenseManagerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        var terms = new LicenseTerms(
            licenseId: Guid.NewGuid(),
            type: LicenseType.Single,
            mode: LicenseMode.TimeBased,
            maxUsers: 1,
            isRenewable: true,
            expirationDate: SystemClock.Now.AddMonths(6),
            renewalDate: SystemClock.Now.AddMonths(6),
            usageLimit: null
        );
    
        var license = License.Create(Guid.NewGuid(), "KEY123", "VendorX", "LicenseX", terms);
        var user = new User("test@example.com", "Test", DepartmentType.Board);

        await dbContext.AddAsync(license);
        await dbContext.AddAsync(user);

        await dbContext.SaveChangesAsync(CancellationToken.None);
        
        var command = new AssignLicenseCommand(license.Id, user.Id);
        var handler = CreateHandler(scope);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var assignment = dbContext.LicenseAssignments.Single(x => x.LicenseId == license.Id);
        Assert.Equal(assignment.UserId, user.Id);   
        Assert.Equal(assignment.LicenseId, license.Id);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessRuleException_WhenLicenseNotFound()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LicenseManagerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        
        var user = new User("test@example.com", "Test", DepartmentType.Board);
        await dbContext.AddAsync(user);
        await dbContext.SaveChangesAsync();
        
        var command = new AssignLicenseCommand(Guid.NewGuid(), user.Id);
        var handler = CreateHandler(scope);
    
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_ShouldThrowBusinessRuleException_WhenBusinessRuleFails()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LicenseManagerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        
        var terms = new LicenseTerms(
            licenseId: Guid.NewGuid(),
            type: LicenseType.Single,
            mode: LicenseMode.TimeBased,
            maxUsers: 1,
            isRenewable: true,
            expirationDate: SystemClock.Now.AddMonths(6),
            renewalDate: SystemClock.Now.AddMonths(6),
            usageLimit: null
        );
        var license = License.Create(Guid.NewGuid(), "KEY123", "VendorX", "LicenseX", terms);
        var user = new User("test@example.com", "Test", DepartmentType.Board);
        var anotherUser = new User("test2@example.com", "Test2", DepartmentType.Board);

        await dbContext.AddAsync(license);
        await dbContext.AddAsync(user);
        await dbContext.AddAsync(anotherUser);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        
        var command = new AssignLicenseCommand(license.Id, user.Id);
        var command2 = new AssignLicenseCommand(license.Id, anotherUser.Id);

        var handler = CreateHandler(scope);
        await handler.Handle(command, CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => 
            handler.Handle(command2, CancellationToken.None));
    }
    
    private static AssignLicenseCommandHandler CreateHandler(IServiceScope scope)
    {
        var licenseRepository = scope.ServiceProvider.GetRequiredService<IRepository<License>>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AssignLicenseCommandHandler>>();  
        var ruleProvider = scope.ServiceProvider.GetRequiredService<ILicenseBusinessRuleProvider>();  
        var policyFactory = scope.ServiceProvider.GetRequiredService<ILicenseAssignmentPolicyFactory>();  

        return new AssignLicenseCommandHandler(ruleProvider, 
            policyFactory, 
            licenseRepository, 
            userRepository, 
            unitOfWork, 
            logger);
    }
}
