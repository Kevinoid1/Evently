using System.Reflection;

namespace Evently.Modules.Events.Presentation;

public static class AssemblyMarker
{
    public static readonly Assembly Assembly = typeof(AssemblyMarker).Assembly;
}
