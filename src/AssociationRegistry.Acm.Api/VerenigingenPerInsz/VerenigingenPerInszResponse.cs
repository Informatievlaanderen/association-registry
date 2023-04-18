namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public class VerenigingenPerInszResponse
{
    public VerenigingenPerInszResponse(string insz,
        ImmutableArray<Vereniging> verenigingen)
    {
        Insz = insz;
        Verenigingen = verenigingen;
    }

    /// <summary>
    ///     Dit is de unieke identificatie van een persoon, dit kan een rijksregisternummer of bisnummer zijn
    /// </summary>
    [DataMember]
    public string Insz { get; init; }

    /// <summary>De lijst van verenigingen waarvoor deze persoon vertegenwoordiger is</summary>
    [DataMember]
    public ImmutableArray<Vereniging> Verenigingen { get; init; }
}
