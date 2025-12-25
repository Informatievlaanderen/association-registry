namespace AssociationRegistry.OpenTelemetry.Metrics;

/// <summary>
/// Detects whether the current Lambda invocation is a cold start.
/// Lambda containers are reused across invocations, so only the first invocation is a cold start.
/// </summary>
public class ColdStartDetector
{
    private bool _isColdStart = true;

    /// <summary>
    /// Returns true if this is the first invocation (cold start), false otherwise.
    /// Subsequent calls will always return false.
    /// </summary>
    public bool IsColdStart()
    {
        var wasColdStart = _isColdStart;
        _isColdStart = false;
        return wasColdStart;
    }
}
