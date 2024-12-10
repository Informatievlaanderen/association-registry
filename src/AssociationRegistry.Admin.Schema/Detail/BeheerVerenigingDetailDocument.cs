namespace AssociationRegistry.Admin.Schema.Detail;

using Marten.Metadata;
using Marten.Schema;

public record BeheerVerenigingDetailDocument : IVCode, ISoftDeleted, IMetadata
{
    public string[] CorresponderendeVCodes { get; set; } = Array.Empty<string>();
    public string JsonLdMetadataType { get; set; }
    public string Naam { get; set; } = null!;
    public VerenigingsType Verenigingstype { get; set; } = null!;
    public string Roepnaam { get; set; } = null!;
    public string? KorteNaam { get; set; }
    public string? KorteBeschrijving { get; set; }
    public string? Startdatum { get; set; }
    public string? Einddatum { get; set; }
    public Doelgroep Doelgroep { get; set; } = null!;
    public string? Rechtsvorm { get; set; }
    public string Status { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = Array.Empty<Locatie>();
    public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();
    public Vertegenwoordiger[] Vertegenwoordigers { get; set; } = Array.Empty<Vertegenwoordiger>();

    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } =
        Array.Empty<HoofdactiviteitVerenigingsloket>();

    public Werkingsgebied[] Werkingsgebieden { get; set; } =
        Array.Empty<Werkingsgebied>();

    public Lidmaatschap[] Lidmaatschappen { get; set; } = Array.Empty<Lidmaatschap>();

    public Sleutel[] Sleutels { get; set; } = Array.Empty<Sleutel>();
    public Relatie[] Relaties { get; set; } = Array.Empty<Relatie>();
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public string Bron { get; set; } = null!;
    public Metadata Metadata { get; set; } = null!;
    [Identity] public string VCode { get; init; } = null!;
    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string IsDubbelVan { get; set; } = string.Empty;
}
