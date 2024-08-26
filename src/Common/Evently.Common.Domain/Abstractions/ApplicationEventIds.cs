using Microsoft.Extensions.Logging;

namespace Evently.Common.Domain.Abstractions;

public sealed class ApplicationEventIds
{
    public static readonly EventId UserRegistered = new(1, "UserRegistered"); 
}
