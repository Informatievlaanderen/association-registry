namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Acties.RegistreerVereniging;
using Common;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;

[DataContract]
public class RegistreerVerenigingRequest
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

    /// <summary>
    ///     Ondernemingsnummer van de vereniging. Formaat '##########', '#### ### ###' en '####.###.###" zijn toegelaten
    /// </summary>
    [DataMember]
    public string? KboNummer { get; init; }

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

    public RegistreerVerenigingCommand ToCommand()
        => new(
            VerenigingsNaam.Create(Naam),
            KorteNaam,
            KorteBeschrijving,
            AssociationRegistry.Vereniging.Startdatum.Create(Startdatum),
            AssociationRegistry.Vereniging.KboNummer.Create(KboNummer),
            Contactgegevens.Select(Map).ToArray(),
            Locaties.Select(Map).ToArray(),
            Vertegenwoordigers.Select(Map).ToArray(),
            HoofdactiviteitenVerenigingsloket.Select(HoofdactiviteitVerenigingsloket.Create).ToArray());

    private static Vertegenwoordiger Map(ToeTeVoegenVertegenwoordiger vert)
        => Vertegenwoordiger.Create(
            Insz.Create(vert.Insz),
            vert.IsPrimair,
            vert.Roepnaam,
            vert.Rol,
            Email.Create(vert.Email),
            TelefoonNummer.Create(vert.Telefoon),
            TelefoonNummer.Create(vert.Mobiel),
            SocialMedia.Create(vert.SocialMedia)
        );

    private static Locatie Map(ToeTeVoegenLocatie loc)
        => Locatie.Create(
            loc.Naam,
            loc.Straatnaam,
            loc.Huisnummer,
            loc.Busnummer,
            loc.Postcode,
            loc.Gemeente,
            loc.Land,
            loc.Hoofdlocatie,
            loc.Locatietype);

    public static Contactgegeven Map(ToeTeVoegenContactgegeven toeTeVoegenContactgegeven)
        => Contactgegeven.Create(
            ContactgegevenType.Parse(toeTeVoegenContactgegeven.Type),
            toeTeVoegenContactgegeven.Waarde,
            toeTeVoegenContactgegeven.Beschrijving,
            toeTeVoegenContactgegeven.IsPrimair);
}
