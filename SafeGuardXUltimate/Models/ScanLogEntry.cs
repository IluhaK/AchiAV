using System;

namespace SafeGuardXUltimate.Models;

public sealed class ScanLogEntry
{
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public required string Message { get; init; }
}
