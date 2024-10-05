using System.Reflection;

namespace Evently.Modules.Ticketing.Presentation;

public static class AssemblyMarker
{
    public static readonly Assembly Assembly = typeof(AssemblyMarker).Assembly;
}
