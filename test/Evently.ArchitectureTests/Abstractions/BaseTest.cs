namespace Evently.ArchitectureTests.Abstractions;

public abstract class BaseTest
{
    protected const string UsersNamespace = "Evently.Modules.Users";
    protected const string UsersIntegrationEventNamespace = "Evently.Modules.Users.IntegrationEvents";
    
    protected const string TicketingNamespace = "Evently.Modules.Ticketing";
    protected const string TicketingIntegrationEventNamespace = "Evently.Modules.Ticketing.IntegrationEvents";
    
    protected const string EventsNamespace = "Evently.Modules.Events";
    protected const string EventsIntegrationEventNamespace = "Evently.Modules.Events.IntegrationEvents";
    
    protected const string AttendanceNamespace = "Evently.Modules.Attendance";
    protected const string AttendanceIntegrationEventNamespace = "Evently.Modules.Attendance.IntegrationEvents";
}
