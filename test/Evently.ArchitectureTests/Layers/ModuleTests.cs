using System.Reflection;
using Evently.ArchitectureTests.Abstractions;
using Evently.Modules.Attendance.Domain.Attendees;
using Evently.Modules.Attendance.Infrastructure;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.Infrastructure;
using Evently.Modules.Ticketing.Domain.Orders;
using Evently.Modules.Ticketing.Infrastructure;
using Evently.Modules.Users.Domain.Users;
using Evently.Modules.Users.Infrastructure;
using NetArchTest.Rules;
using Xunit;

namespace Evently.ArchitectureTests.Layers;

public class ModuleTests : BaseTest
{
    [Fact]
    public void UserModule_ShouldNotHaveDependencyOnAnyOtherModules()
    {
        // arrange
        string[] otherModules = [EventsNamespace, TicketingNamespace, AttendanceNamespace];
        string[] integrationEventSubModules =
            [EventsIntegrationEventNamespace, TicketingIntegrationEventNamespace, AttendanceIntegrationEventNamespace];

        List<Assembly> userAssemblies =
        [
            typeof(User).Assembly, // domain project
            Modules.Users.Application.AssemblyMarker.Assembly, // application project
            Modules.Users.Presentation.AssemblyMarker.Assembly, // presentation project
            typeof(UsersModule).Assembly // infrastructure project
        ];
        
        //act
        TestResult result = Types.InAssemblies(userAssemblies)
            .That()
            .DoNotHaveDependencyOnAny(integrationEventSubModules)
            .Should()
            .NotHaveDependencyOnAny(otherModules)
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void TicketingModule_ShouldNotHaveDependencyOnAnyOtherModules()
    {
        // arrange
        string[] otherModules = [EventsNamespace, UsersNamespace, AttendanceNamespace];
        string[] integrationEventSubModules =
            [EventsIntegrationEventNamespace, UsersIntegrationEventNamespace, AttendanceIntegrationEventNamespace];

        List<Assembly> ticketingAssemblies =
        [
            typeof(Order).Assembly, // domain project
            Modules.Ticketing.Application.AssemblyMarker.Assembly, // application project
            Modules.Ticketing.Presentation.AssemblyMarker.Assembly, // presentation project
            typeof(TicketingModule).Assembly // infrastructure project
        ];
        
        //act
        TestResult result = Types.InAssemblies(ticketingAssemblies)
            .That()
            .DoNotHaveDependencyOnAny(integrationEventSubModules)
            .Should()
            .NotHaveDependencyOnAny(otherModules)
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void EventModule_ShouldNotHaveDependencyOnAnyOtherModules()
    {
        // arrange
        string[] otherModules = [TicketingNamespace, UsersNamespace, AttendanceNamespace];
        string[] integrationEventSubModules =
            [TicketingIntegrationEventNamespace, UsersIntegrationEventNamespace, AttendanceIntegrationEventNamespace];

        List<Assembly> ticketingAssemblies =
        [
            typeof(Event).Assembly, // domain project
            Modules.Events.Application.AssemblyMarker.Assembly, // application project
            Modules.Events.Presentation.AssemblyMarker.Assembly, // presentation project
            typeof(EventsModule).Assembly // infrastructure project
        ];
        
        //act
        TestResult result = Types.InAssemblies(ticketingAssemblies)
            .That()
            .DoNotHaveDependencyOnAny(integrationEventSubModules)
            .Should()
            .NotHaveDependencyOnAny(otherModules)
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
    
    [Fact]
    public void AttendanceModule_ShouldNotHaveDependencyOnAnyOtherModules()
    {
        // arrange
        string[] otherModules = [EventsNamespace, UsersNamespace, TicketingNamespace];
        string[] integrationEventSubModules =
            [EventsIntegrationEventNamespace, UsersIntegrationEventNamespace, TicketingIntegrationEventNamespace];

        List<Assembly> ticketingAssemblies =
        [
            typeof(Attendee).Assembly, // domain project
            Modules.Attendance.Application.AssemblyMarker.Assembly, // application project
            Modules.Attendance.Presentation.AssemblyMarker.Assembly, // presentation project
            typeof(AttendanceModule).Assembly // infrastructure project
        ];
        
        //act
        TestResult result = Types.InAssemblies(ticketingAssemblies)
            .That()
            .DoNotHaveDependencyOnAny(integrationEventSubModules)
            .Should()
            .NotHaveDependencyOnAny(otherModules)
            .GetResult();
        
        // assert
        result.ShouldBeSuccessful();
    }
}
