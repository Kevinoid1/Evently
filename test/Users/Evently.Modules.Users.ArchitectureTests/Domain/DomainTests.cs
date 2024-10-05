using System.Reflection;
using Evently.Common.Domain.Abstractions;
using Evently.Modules.Users.ArchitectureTests.Abstractions;
using FluentAssertions;
using NetArchTest.Rules;

namespace Evently.Modules.Users.ArchitectureTests.Domain;

public class DomainTests : BaseTest
{

    [Fact]
    public void DomainEvents_ShouldBeSealed()
    {
        TestResult testResult = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(DomainEvent))
            .Should()
            .BeSealed()
            .GetResult();
        
        testResult.ShouldBeSuccessful();
    }

    [Fact]
    public void DomainEvents_ShouldHaveNamesEndingWithDomainEvent()
    {
        TestResult testResult = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(DomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();
        
        testResult.ShouldBeSuccessful();
    }

    [Fact]
    public void Entities_ShouldHavePrivateParameterlessConstructor()
    {
        // arrange & act
        ConstructorInfo[] constructors = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes()
            .SelectMany(type => type.GetConstructors())
            .Where(constructor => !(constructor.IsPrivate && constructor.GetParameters().Length == 0))
            .ToArray();
        
        // assert
        constructors.Should().BeEmpty();
    }
    
    
    [Fact]
    public void Entities_ShouldOnlyHavePrivateConstructors()
    {
        // arrange & act
        /*ConstructorInfo[] constructors = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes()
            .SelectMany(type => type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            .ToArray()*/ // didn't catch internal constructors 
        
        // arrange & act
        ConstructorInfo[] constructors = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes()
            .SelectMany(type => type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            .Where(constructor => !constructor.IsPrivate )
            .ToArray();
        
        // assert
        constructors.Should().BeEmpty();
    }
}
