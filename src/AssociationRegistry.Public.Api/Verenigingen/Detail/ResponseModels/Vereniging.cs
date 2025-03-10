namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using AssociationRegistry.Vereniging;
using System.ComponentModel;
using System.Runtime.Serialization;

[DataContract]
public class Vereniging
{
    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>De unieke identificatie code van deze vereniging</summary>
    [DataMember(Name = "VCode")]
    public string VCode { get; init; } = null!;

    /// <summary>Type van de vereniging</summary>
    [DataMember(Name = "Verenigingstype")]
    public VerenigingsType Verenigingstype { get; init; } = null!;

    /// <summary>Het subtype van deze vereniging</summary>
    [DataMember(Name = "Verenigingssubtype", EmitDefaultValue = false)]
    public Verenigingssubtype? Verenigingssubtype { get; init; } = null!;

    /// <summary>Naam van de vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;

    /// <summary>Roepnaam van de vereniging. Dit veld is enkel aanwezig bij verenigingen met rechtspersoonlijkheid</summary>
    [DataMember(Name = "Roepnaam", EmitDefaultValue = false)]
    [DefaultValue(null)]
    public string? Roepnaam { get; init; }

    /// <summary>Korte naam van de vereniging</summary>
    [DataMember(Name = "KorteNaam")]
    public string? KorteNaam { get; init; }

    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember(Name = "KorteBeschrijving")]
    public string? KorteBeschrijving { get; init; }

    /// <summary>Datum waarop de vereniging gestart is. Deze datum mag niet later zijn dan vandaag</summary>
    [DataMember(Name = "Startdatum")]
    public DateOnly? Startdatum { get; init; }

    /// <summary>De doelgroep waar de activiteiten van deze vereniging zich op concentreert</summary>
    [DataMember(Name = "Doelgroep")]
    public DoelgroepResponse Doelgroep { get; init; } = null!;

    /// <summary>Status van de vereniging</summary>
    [DataMember(Name = "Status")]
    public string Status { get; init; } = null!;

    /// <summary>De contactgegevens van deze vereniging</summary>
    [DataMember(Name = "Contactgegevens")]
    public Contactgegeven[] Contactgegevens { get; init; } = [];

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    [DataMember(Name = "Locaties")]
    public Locatie[] Locaties { get; init; } = [];

    /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
    [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; } = [];

    /// <summary>De werkingsgebieden van deze vereniging</summary>
    [DataMember(Name = "Werkingsgebieden")]
    public Werkingsgebied[] Werkingsgebieden { get; init; } = [];

    /// <summary>De sleutels die deze vereniging beheren</summary>
    [DataMember(Name = "Sleutels")]
    public Sleutel[] Sleutels { get; init; } = [];

    /// <summary>De relaties van deze vereniging</summary>
    [DataMember(Name = "Relaties")]
    public Relatie[] Relaties { get; init; } = [];

    /// <summary>De lidmaatschappen van deze vereniging</summary>
    [DataMember(Name = "Lidmaatschappen")]
    public Lidmaatschap[] Lidmaatschappen { get; init; } = [];
}
