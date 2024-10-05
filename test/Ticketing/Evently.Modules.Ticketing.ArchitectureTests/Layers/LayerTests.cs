using Evently.Modules.Ticketing.ArchitectureTests.Abstractions;
using NetArchTest.Rules;

namespace Evently.Modules.Ticketing.ArchitectureTests.Layers;

public class LayerTests : BaseTest
{
    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOnApplicationLayer()
    {
        Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }
    
    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOnInfrastructureLayer()
    {
        Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }
    
    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOnInfrastructureLayer()
    {
        Types
            .InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }
    
    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOnPresentationLayer()
    {
        Types
            .InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }
    
    [Fact]
    public void PresentationLayer_ShouldNotHaveDependencyOnInfrastructureLayer()
    {
        Types
            .InAssembly(PresentationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }
}
