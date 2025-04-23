namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;
using Vereniging;

[DataContract]
public class VerenigingDetail
{
    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>De unieke identificatie code van deze vereniging</summary>
    [DataMember(Name = "VCode")]
    public string VCode { get; init; } = null!;

    // <summary>De unieke identificatie codes van de corresponderende verenigingen</summary>
    [DataMember(Name = "CorresponderendeVCodes")]
    public string[] CorresponderendeVCodes { get; init; } = [];

    /// <summary>Het type van deze vereniging</summary>
    [DataMember(Name = "Verenigingstype")]
    public Verenigingstype Verenigingstype { get; init; } = null!;

    /// <summary>Het subtype van deze vereniging</summary>
    [DataMember(Name = "Verenigingssubtype", EmitDefaultValue = false)]
    public Verenigingssubtype? Verenigingssubtype { get; init; } = null!;

    /// <summary>Extra informatie over het subtype van deze vereniging</summary>
    [DataMember(Name = "SubverenigingVan", EmitDefaultValue = false)]
    public SubverenigingVan? SubverenigingVan { get; init; } = null!;


    /// <summary>Naam van de vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;

    /// <summary>Roepnaam van de vereniging. Dit veld is enkel aanwezig bij verenigingen met rechtspersoonlijkheid</summary>
    [DataMember(Name = "Roepnaam", EmitDefaultValue = false)]
    public string Roepnaam { get; init; } = null!;

    /// <summary>Korte naam van de vereniging</summary>
    [DataMember(Name = "KorteNaam")]
    public string? KorteNaam { get; init; }

    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember(Name = "KorteBeschrijving")]
    public string? KorteBeschrijving { get; init; }

    /// <summary>Datum waarop de vereniging gestart is</summary>
    [DataMember(Name = "Startdatum")]
    public string? Startdatum { get; init; }

    /// <summary>Datum waarop de vereniging gestopt is</summary>
    [DataMember(Name = "Einddatum")]
    public string? Einddatum { get; init; }

    /// <summary>De doelgroep waar de activiteiten van deze vereniging zich op concentreert</summary>
    [DataMember(Name = "Doelgroep")]
    public DoelgroepResponse Doelgroep { get; init; } = null!;

    /// <summary>Status van de vereniging<br />
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Actief<br />
    ///     - Gestopt<br />
    ///     - Dubbel<br />
    /// </summary>
    [DataMember(Name = "Status")]
    public string Status { get; init; } = null!;

    /// <summary>Is deze vereniging uitgeschreven uit de publieke datastroom</summary>
    [DataMember(Name = "IsUitgeschrevenUitPubliekeDatastroom")]
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; init; }

    /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
    [DataMember(Name = "hoofdactiviteitenVerenigingsloket")]
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; } = null!;

    /// <summary>De werkingsgebieden van deze vereniging</summary>
    [DataMember(Name = "werkingsgebieden")]
    public Werkingsgebied[] Werkingsgebieden { get; set; } = null!;

    /// <summary>De contactgegevens van deze vereniging</summary>
    [DataMember(Name = "Contactgegevens")]
    public Contactgegeven[] Contactgegevens { get; init; } = null!;

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    [DataMember(Name = "Locaties")]
    public Locatie[] Locaties { get; init; } = null!;

    /// <summary>Alle vertegenwoordigers van deze vereniging</summary>
    [DataMember(Name = "Vertegenwoordigers")]
    public Vertegenwoordiger[] Vertegenwoordigers { get; init; } = null!;

    /// <summary>De sleutels van deze vereniging</summary>
    [DataMember(Name = "Sleutels")]
    public Sleutel[] Sleutels { get; init; } = null!;

    /// <summary>De relaties van deze vereniging</summary>
    [DataMember(Name = "Relaties")]
    public Relatie[] Relaties { get; init; } = null!;

    /// <summary>De lidmaatschappen van deze vereniging</summary>
    [DataMember(Name = "Lidmaatschappen")]
    public Lidmaatschap[] Lidmaatschappen { get; init; } = null!;

    /// <summary>De bron die deze vereniging beheert.
    ///      <br />
    ///     Mogelijke waarden:<br />
    ///     - Initiator<br />
    ///     - KBO
    /// </summary>
    [DataMember(Name = "Bron")]
    public string Bron { get; set; } = null!;

    /// <summary>De VCode van de vereniging waarvan deze vereniging een dubbel is</summary>
    [DataMember(Name = "IsDubbelVan")]
    public string IsDubbelVan { get; set; }
}
