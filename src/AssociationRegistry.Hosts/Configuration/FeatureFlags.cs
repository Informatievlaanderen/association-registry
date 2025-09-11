namespace AssociationRegistry.Hosts.Configuration;

public static class FeatureFlags
{
    public static bool IsTestingMode() => false;
        // string.Equals(
        //     Environment.GetEnvironmentVariable(WellknownFeatureFlags.TestMode),
        //     "true",
        //     StringComparison.OrdinalIgnoreCase);
}

public static class WellknownFeatureFlags
{
    public const string TestMode = "TestMode";
}
