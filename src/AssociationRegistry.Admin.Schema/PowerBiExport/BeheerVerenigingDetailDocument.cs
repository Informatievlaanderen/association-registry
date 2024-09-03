namespace AssociationRegistry.Admin.Schema.PowerBiExport;

using Detail;
using Marten.Metadata;
using Marten.Schema;

public record PowerBiExportDocument : IVCode
{
    public string[] CorresponderendeVCodes { get; set; } = Array.Empty<string>();
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
    public Locatie[] Locaties { get; set; } = Array.Empty<Locatie>();
    public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();
    public int AantalVertegenwoordigers { get; set; }

    public PowerBiExport.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } =
        Array.Empty<HoofdactiviteitVerenigingsloket>();

    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public string Bron { get; set; } = null!;
    [Identity] public string VCode { get; init; } = null!;
    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string KboNummer { get; set; }
    public string DatumLaatsteAanpassing { get; set; }
}
