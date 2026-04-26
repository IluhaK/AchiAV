using System;

namespace SafeGuardXUltimate.Models;

public sealed class ActivityEvent
{
    public DateTime Time { get; init; } = DateTime.Now;
    public required string EventType { get; init; }
    public required string Message { get; init; }
}
