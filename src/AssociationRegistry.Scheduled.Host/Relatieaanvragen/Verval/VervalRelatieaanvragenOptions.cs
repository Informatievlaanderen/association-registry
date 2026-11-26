namespace AssociationRegistry.Scheduled.Host.Relatieaanvragen.Verval;

/// <summary>In UTC, like the other days in VR. Early: until the job ran, the guards of a vervallen relatieaanvraag stay.</summary>
public class VervalRelatieaanvragenOptions
{
    public string Cron { get; set; } = "0 0 1 * * ?";
}
