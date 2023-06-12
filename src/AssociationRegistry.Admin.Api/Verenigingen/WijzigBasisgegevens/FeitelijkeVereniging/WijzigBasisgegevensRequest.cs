namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;

using System;
using System.Linq;
using System.Runtime.Serialization;
using AssociationRegistry.Acties.WijzigBasisgegevens;
using Primitives;
using Vereniging;

[DataContract]
public class WijzigBasisgegevensRequest
{
    /// <summary>Nieuwe naam van de vereniging</summary>
    [DataMember]
    public string? Naam { get; set; }

    /// <summary>Nieuwe korte naam van de vereniging</summary>
    [DataMember]
    public string? KorteNaam { get; set; }

    /// <summary>Nieuwe korte beschrijving van de vereniging</summary>
    [DataMember]
    public string? KorteBeschrijving { get; set; }

    /// <summary>Nieuwe startdatum van de vereniging. Deze datum mag niet later zijn dan vandaag</summary>
    [DataMember]
    public NullOrEmpty<DateOnly> Startdatum { get; set; }

    /// <summary>
    /// De codes van de nieuwe hoofdactiviteiten volgens het verenigingsloket
    /// </summary>
    [DataMember]
    public string[]? HoofdactiviteitenVerenigingsloket { get; set; }

    /// <summary>
    /// Verberg deze vereniging van de publieke zoek API's
    /// </summary>
    [DataMember]
    public bool? IsUitgeschrevenUitPubliekeDatastroom { get; set; }

    public WijzigBasisgegevensCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            Naam is null ? null : VerenigingsNaam.Create(Naam),
            KorteNaam,
            KorteBeschrijving,
            Startdatum.IsNull ? null :
            Startdatum.IsEmpty ? AssociationRegistry.Vereniging.Startdatum.Leeg :
            AssociationRegistry.Vereniging.Startdatum.Create(Startdatum.Value),
            HoofdactiviteitenVerenigingsloket?.Select(HoofdactiviteitVerenigingsloket.Create).ToArray(),
            IsUitgeschrevenUitPubliekeDatastroom
        );
}
