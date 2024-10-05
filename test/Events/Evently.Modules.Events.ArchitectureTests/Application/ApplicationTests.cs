using Evently.Common.Application.Messaging;
using Evently.Modules.Events.ArchitectureTests.Abstractions;
using FluentValidation;
using NetArchTest.Rules;

namespace Evently.Modules.Events.ArchitectureTests.Application;

public class ApplicationTests : BaseTest
{
    [Fact]
    public void Command_ShouldBeSealed()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommand))
            .Or()
            .ImplementInterface(typeof(ICommand<>))
            .Should()
            .BeSealed()
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();

    }
    
    [Fact]
    public void Command_ShouldHaveNamesEndingWithCommand()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommand))
            .Or()
            .ImplementInterface(typeof(ICommand<>))
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();

    }
    
    [Fact]
    public void CommandHandler_ShouldNotBePublic()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void CommandHandler_ShouldBeSealed()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .BeSealed()
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void CommandHandler_ShouldHaveNameEndingWithCommandHandler()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
     public void Query_ShouldBeSealed()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .Should()
            .BeSealed()
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();

    }
    
    [Fact]
    public void Query_ShouldHaveNamesEndingWithQuery()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .Should()
            .HaveNameEndingWith("Query")
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();

    }
    
    [Fact]
    public void QueryHandler_ShouldNotBePublic()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void QueryHandler_ShouldBeSealed()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .BeSealed()
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void QueryHandler_ShouldHaveNameEndingWithCQueryHandler()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .HaveNameEndingWith("QueryHandler")
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void Validator_ShouldNotBePublic()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .NotBePublic()
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void Validator_ShouldBeSealed()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .BeSealed()
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void Validator_ShouldHaveNameEndingWithValidator()
    {
        // act
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .HaveNameEndingWith("Validator")
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void DomainEventHandler_ShouldNotBePublic()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEventHandler<>))
            .Should()
            .NotBePublic()
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void DomainEventHandler_ShouldBeSealed()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEventHandler<>))
            .Should()
            .BeSealed()
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void DomainEventHandler_ShouldHaveNameEndingWithDomainEventHandler()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEventHandler<>))
            .Should()
            .HaveNameEndingWith("DomainEventHandler")
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
}
