using System.Reflection;

namespace Evently.Modules.Attendance.Presentation;

public static class AssemblyMarker
{
    public static readonly Assembly Assembly = typeof(AssemblyMarker).Assembly;
}
