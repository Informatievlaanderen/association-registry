namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;

using Acties.Registratie.RegistreerFeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using Common;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

[DataContract]
public class RegistreerFeitelijkeVerenigingRequest
{
    /// <summary>Naam van de vereniging</summary>
    [DataMember]
    [Required]
    public string Naam { get; set; } = null!;

    /// <summary>Korte naam van de vereniging</summary>
    [DataMember]
    public string? KorteNaam { get; set; }

    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember]
    public string? KorteBeschrijving { get; set; }

    /// <summary>Datum waarop de vereniging gestart is. Deze datum mag niet later zijn dan vandaag</summary>
    [DataMember]
    public DateOnly? Startdatum { get; set; }

    /// <summary>
    /// De doelgroep waar de activiteiten van deze vereniging zich op concentreert
    /// </summary>
    [DataMember]
    public DoelgroepRequest? Doelgroep { get; set; }

    /// <summary>
    /// Is deze vereniging uitgeschreven uit de publieke datastroom
    /// </summary>
    [DataMember]
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }

    /// <summary>De contactgegevens van deze vereniging</summary>
    [DataMember]
    public ToeTeVoegenContactgegeven[] Contactgegevens { get; set; } = [];

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    [DataMember]
    public ToeTeVoegenLocatie[] Locaties { get; set; } = [];

    /// <summary>De vertegenwoordigers van deze vereniging</summary>
    [DataMember]
    public ToeTeVoegenVertegenwoordiger[] Vertegenwoordigers { get; set; } = [];

    /// <summary>De codes van de hoofdactivititeiten volgens het verenigingsloket</summary>
    [DataMember]
    public string[] HoofdactiviteitenVerenigingsloket { get; set; } = [];

    /// <summary>De codes van de werkingsgebieden</summary>
    [DataMember]
    public string[]? Werkingsgebieden { get; set; } = [];

    public RegistreerFeitelijkeVerenigingCommand ToCommand()
        => new(
            VerenigingsNaam.Create(Naam),
            KorteNaam,
            KorteBeschrijving,
            Datum.CreateOptional(Startdatum),
            DoelgroepRequest.Map(Doelgroep),
            IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens.Select(ToeTeVoegenContactgegeven.Map).ToArray(),
            Locaties.Select(ToeTeVoegenLocatie.Map).ToArray(),
            Vertegenwoordigers.Select(ToeTeVoegenVertegenwoordiger.Map).ToArray(),
            HoofdactiviteitenVerenigingsloket.Select(HoofdactiviteitVerenigingsloket.Create).ToArray(),
            Werkingsgebieden?.Select(Werkingsgebied.Create).ToArray() ?? AssociationRegistry.Vereniging.Werkingsgebieden.NietBepaald);
}
