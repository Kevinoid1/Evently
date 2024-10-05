using System.Reflection;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.Infrastructure;

namespace Evently.Modules.Events.ArchitectureTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = Events.Application.AssemblyMarker.Assembly;
    protected static readonly Assembly DomainAssembly = typeof(Event).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(EventsModule).Assembly;
    protected static readonly Assembly PresentationAssembly = Events.Presentation.AssemblyMarker.Assembly;
}
