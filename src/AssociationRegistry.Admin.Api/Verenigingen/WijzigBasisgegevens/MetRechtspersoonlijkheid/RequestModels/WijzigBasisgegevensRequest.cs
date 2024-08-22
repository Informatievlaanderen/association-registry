namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;

using Acties.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevens;
using Common;
using System.Runtime.Serialization;
using Vereniging;

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
    /// De nieuwe roepnaam van de vereniging
    /// </summary>
    [DataMember]
    public string? Roepnaam { get; set; }

    public WijzigBasisgegevensCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            Roepnaam,
            KorteBeschrijving,
            Doelgroep is null ? null : DoelgroepRequest.Map(Doelgroep),
            HoofdactiviteitenVerenigingsloket?.Select(HoofdactiviteitVerenigingsloket.Create).ToArray()
        );
}
