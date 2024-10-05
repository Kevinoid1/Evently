using Evently.Modules.Events.ArchitectureTests.Abstractions;
using MassTransit;
using NetArchTest.Rules;

namespace Evently.Modules.Events.ArchitectureTests.Presentation;

public class PresentationTests : BaseTest
{
    [Fact]
    public void IntegrationEventHandler_ShouldBeSealed()
    {
        TestResult result = Types.InAssembly(PresentationAssembly)
            .That()
            .ImplementInterface(typeof(IConsumer<>))
            .Should()
            .BeSealed()
            .GetResult();
        
        result.ShouldBeSuccessful();
    }

    [Fact]
    public void IntegrationEventHandler_ShouldHaveNameEndingWithConsumer()
    {
        TestResult result = Types.InAssembly(PresentationAssembly)
            .That()
            .ImplementInterface(typeof(IConsumer<>))
            .Should()
            .HaveNameEndingWith("Consumer")
            .GetResult();
        
        result.ShouldBeSuccessful();
    }
}
