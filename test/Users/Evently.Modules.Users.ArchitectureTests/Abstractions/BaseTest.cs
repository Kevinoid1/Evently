using System.Reflection;
using Evently.Modules.Users.Domain.Users;
using Evently.Modules.Users.Infrastructure;

namespace Evently.Modules.Users.ArchitectureTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = Users.Application.AssemblyMarker.Assembly;
    protected static readonly Assembly DomainAssembly = typeof(User).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(UsersModule).Assembly;
    protected static readonly Assembly PresentationAssembly = Users.Presentation.AssemblyMarker.Assembly;
}
