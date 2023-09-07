namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling.RequestModels;

using Acties.RegistreerAfdeling;
using Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Vereniging;

[DataContract]
public class RegistreerAfdelingRequest
{
    /// <summary>Naam van de afdeling</summary>
    [DataMember]
    [Required]
    public string Naam { get; init; } = null!;

    /// <summary>Kbo nummer van de moedervereniging</summary>
    [DataMember]
    [Required]
    public string KboNummerMoedervereniging { get; init; } = null!;

    /// <summary>Korte naam van de afdeling</summary>
    [DataMember]
    public string? KorteNaam { get; init; }

    /// <summary>Korte beschrijving van de afdeling</summary>
    [DataMember]
    public string? KorteBeschrijving { get; init; }

    /// <summary>Datum waarop de afdeling gestart is. Deze datum mag niet later zijn dan vandaag</summary>
    [DataMember]
    public DateOnly? Startdatum { get; init; }

    /// <summary>
    /// De doelgroep waar de activiteiten van deze afdeling zich op concentreert
    /// </summary>
    [DataMember]
    public DoelgroepRequest? Doelgroep { get; set; }

    /// <summary>De contactgegevens van deze afdeling</summary>
    [DataMember]
    public ToeTeVoegenContactgegeven[] Contactgegevens { get; set; } = Array.Empty<ToeTeVoegenContactgegeven>();

    /// <summary>Alle locaties waar deze afdeling actief is</summary>
    [DataMember]
    public ToeTeVoegenLocatie[] Locaties { get; set; } = Array.Empty<ToeTeVoegenLocatie>();

    /// <summary>De vertegenwoordigers van deze afdeling</summary>
    [DataMember]
    public ToeTeVoegenVertegenwoordiger[] Vertegenwoordigers { get; set; } = Array.Empty<ToeTeVoegenVertegenwoordiger>();

    /// <summary>De codes van de hoofdactivititeiten volgens het verenigingsloket</summary>
    [DataMember]
    public string[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<string>();

    public RegistreerAfdelingCommand ToCommand()
        => new(
            VerenigingsNaam.Create(Naam),
            KboNummer.Create(KboNummerMoedervereniging),
            KorteNaam,
            KorteBeschrijving,
            Datum.CreateOptional(Startdatum),
            DoelgroepRequest.Map(Doelgroep),
            Contactgegevens.Select(ToeTeVoegenContactgegeven.Map).ToArray(),
            Locaties.Select(ToeTeVoegenLocatie.Map).ToArray(),
            Vertegenwoordigers.Select(ToeTeVoegenVertegenwoordiger.Map).ToArray(),
            HoofdactiviteitenVerenigingsloket.Select(HoofdactiviteitVerenigingsloket.Create).ToArray());
}
