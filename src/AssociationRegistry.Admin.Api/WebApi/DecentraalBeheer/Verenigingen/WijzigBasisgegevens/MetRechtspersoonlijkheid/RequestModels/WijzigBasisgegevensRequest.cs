namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;

using AssociationRegistry.Vereniging;
using Common;
using DecentraalBeheer.Acties.Basisgegevens.VerenigingMetRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using System.Runtime.Serialization;

[DataContract]
public class WijzigBasisgegevensRequest
{
    /// <summary>Nieuwe korte beschrijving van de vereniging</summary>
    [DataMember]
    public string? KorteBeschrijving { get; set; }

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
    /// De nieuwe roepnaam van de vereniging
    /// </summary>
    [DataMember]
    public string? Roepnaam { get; set; }

    public WijzigBasisgegevensCommand ToCommand(string vCode, IWerkingsgebiedenService werkingsgebiedenService)
        => new(
            VCode.Create(vCode),
            Roepnaam,
            KorteBeschrijving,
            Doelgroep is null ? null : DoelgroepRequest.Map(Doelgroep),
            HoofdactiviteitenVerenigingsloket?.Select(HoofdactiviteitVerenigingsloket.Create).ToArray(),
            Werkingsgebieden?.Select(werkingsgebiedenService.Create).ToArray()
        );
}
