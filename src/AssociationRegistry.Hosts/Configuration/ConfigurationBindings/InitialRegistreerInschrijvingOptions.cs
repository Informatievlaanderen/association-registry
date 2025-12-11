namespace AssociationRegistry.Hosts.Configuration.ConfigurationBindings;

public class InitialRegistreerInschrijvingOptions
{
    public const string SectionName = "InitialRegistreerInschrijvingOptions";
    public int MaximumParallelMessages { get; set; } = 10;
    public int PauseTimeInSeconds { get; set; } = 10;
    public int FailurePercentageThreshold { get; set; } = 10;
    public int TrackingPeriodInSeconds { get; set; } = 60;
}
