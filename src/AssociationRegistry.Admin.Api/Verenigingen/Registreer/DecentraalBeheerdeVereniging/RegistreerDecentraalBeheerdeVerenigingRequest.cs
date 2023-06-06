namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Acties.RegistreerFeitelijkeVereniging;
using Common;
using Vereniging;

[DataContract]
public class RegistreerDecentraalBeheerdeVerenigingRequest
{
    /// <summary>Naam van de vereniging</summary>
    [DataMember]
    [Required]
    public string Naam { get; init; } = null!;

    /// <summary>Korte naam van de vereniging</summary>
    [DataMember]
    public string? KorteNaam { get; init; }

    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember]
    public string? KorteBeschrijving { get; init; }

    /// <summary>Datum waarop de vereniging gestart is. Deze datum mag niet later zijn dan vandaag</summary>
    [DataMember]
    public DateOnly? Startdatum { get; init; }

    /// <summary>De contactgegevens van deze vereniging</summary>
    [DataMember]
    public ToeTeVoegenContactgegeven[] Contactgegevens { get; set; } = Array.Empty<ToeTeVoegenContactgegeven>();

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    [DataMember]
    public ToeTeVoegenLocatie[] Locaties { get; set; } = Array.Empty<ToeTeVoegenLocatie>();

    /// <summary>De vertegenwoordigers van deze vereniging</summary>
    [DataMember]
    public ToeTeVoegenVertegenwoordiger[] Vertegenwoordigers { get; set; } = Array.Empty<ToeTeVoegenVertegenwoordiger>();

    /// <summary>De codes van de hoofdactivititeiten volgens het verenigingsloket</summary>
    [DataMember]
    public string[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<string>();

    public RegistreerFeitelijkeVerenigingCommand ToCommand()
        => new(
            VerenigingsNaam.Create(Naam),
            KorteNaam,
            KorteBeschrijving,
            AssociationRegistry.Vereniging.Startdatum.Create(Startdatum),
            Contactgegevens.Select(ToeTeVoegenContactgegeven.Map).ToArray(),
            Locaties.Select(ToeTeVoegenLocatie.Map).ToArray(),
            Vertegenwoordigers.Select(ToeTeVoegenVertegenwoordiger.Map).ToArray(),
            HoofdactiviteitenVerenigingsloket.Select(HoofdactiviteitVerenigingsloket.Create).ToArray());
}
