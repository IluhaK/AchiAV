using SafeGuardXUltimate.Models;
using System.Collections.Generic;

namespace SafeGuardXUltimate.Services;

public sealed class ActivityFeedService
{
    public IEnumerable<ActivityEvent> GetInitialEvents()
    {
        return new[]
        {
            new ActivityEvent { EventType = "Firewall", Message = "Inbound demo packet blocked." },
            new ActivityEvent { EventType = "Web", Message = "Phishing URL simulation quarantined." },
            new ActivityEvent { EventType = "AI", Message = "Behavioral heuristic recalibrated." }
        };
    }
}
