namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

/// <summary>Appsettings section RelatieaanvraagOptions.</summary>
public class RelatieaanvraagOptions
{
    /// <summary>
    ///     Days from the aanvraagdatum to the vervaldatum of a new relatieaanvraag. A test environment can set a short
    ///     termijn, e.g. 1, to test verval by hand.
    /// </summary>
    public int VervaltermijnInDagen { get; set; } = 90;
}
