using System.Reflection;

namespace Evently.Modules.Attendance.Application;

public static class AssemblyMarker
{
    public static readonly Assembly Assembly = typeof(AssemblyMarker).Assembly;
}
