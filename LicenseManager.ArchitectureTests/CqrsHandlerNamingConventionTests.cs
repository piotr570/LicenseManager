using FluentAssertions;
using LicenseManager.Application;
using MediatR;
using NetArchTest.Rules;
using Xunit;
using System.Linq;

namespace LicenseManager.ArchitectureTests;

public class CqrsHandlerNamingConventionTests
{
    [Fact]
    public void Handlers_Should_HaveNameEndingWithConvention()
    {
        var result = Types.InAssembly(ApplicationRegistration.ApplicationAssembly)
            .That()
            .AreClasses()
            .And()
            .ImplementInterface(typeof(IRequestHandler<>))
            .Or()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .GetTypes();

        Assert.True(result.All(t => t.Name.EndsWith("QueryHandler") || t.Name.EndsWith("CommandHandler")));
    }
}