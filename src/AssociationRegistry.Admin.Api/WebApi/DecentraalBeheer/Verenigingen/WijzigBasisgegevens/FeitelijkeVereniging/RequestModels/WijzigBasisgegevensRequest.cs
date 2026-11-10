namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;

using System.Runtime.Serialization;
using AssociationRegistry.Primitives;
using AssociationRegistry.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingZonderEigenRechtspersoonlijkheid;
using Common;
using DecentraalBeheer.Vereniging;

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

    /// <summary>Nieuwe startdatum (yyyy-MM-dd) van de vereniging. Deze datum mag niet later zijn dan vandaag</summary>
    [DataMember(IsRequired = false)]
    public NullOrEmpty<DateOnly> Startdatum { get; set; }

    /// <summary>
    /// De doelgroep waar de activiteiten van deze vereniging zich op concentreert
    /// </summary>
    [DataMember]
    public DoelgroepRequest? Doelgroep { get; set; }

    /// <summary>
    /// De codes van de nieuwe hoofdactiviteiten volgens het verenigingsloket
    /// </summary>
    [DataMember]
    public string[]? HoofdactiviteitenVerenigingsloket { get; set; }

    /// <summary>
    /// De codes van de nieuwe werkingsgebieden
    /// </summary>
    [DataMember]
    public string[]? Werkingsgebieden { get; set; }

    /// <summary>
    /// Is deze vereniging uitgeschreven uit de publiek datastroom, dit kan enkel gewijzigd worden voor een feitelijke vereniging
    /// </summary>
    [DataMember]
    public bool? IsUitgeschrevenUitPubliekeDatastroom { get; set; }

    public async Task<WijzigBasisgegevensCommand> ToCommand(
        string vCode,
        IWerkingsgebiedenService werkingsgebiedenService
    ) =>
        new(
            VCode.Create(vCode),
            Naam is null ? null : VerenigingsNaam.Create(Naam),
            KorteNaam,
            KorteBeschrijving,
            Startdatum is { IsNull: false } ? Startdatum.Map(Datum.Create) : NullOrEmpty<Datum>.Null,
            Doelgroep is null ? null : DoelgroepRequest.Map(Doelgroep),
            HoofdactiviteitenVerenigingsloket?.Select(HoofdactiviteitVerenigingsloket.Create).ToArray(),
            Werkingsgebieden is null ? null : await werkingsgebiedenService.CreateMany(Werkingsgebieden),
            IsUitgeschrevenUitPubliekeDatastroom
        );
}
