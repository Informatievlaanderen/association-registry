namespace AssociationRegistry.Admin.Schema.Search;

public class VerenigingZoekUpdateDocument
{
    public string VCode { get; set; } = null!;
    public string? Naam { get; set; } = null!;
    public string? Roepnaam { get; set; } = null!;
    public string? KorteNaam { get; set; } = null!;
    public string? KorteBeschrijving { get; set; } = null!;
    public string? Startdatum { get; set; } = null!;
    public string? Einddatum { get; set; } = null!;

    public VerenigingZoekDocument.Types.Doelgroep? Doelgroep { get; set; } = null!;
    public VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket[]? HoofdactiviteitenVerenigingsloket { get; set; } = null!;

    public VerenigingZoekDocument.Types.VerenigingsType Verenigingstype { get; set; } = null!;
    public VerenigingZoekDocument.Types.Verenigingssubtype? Verenigingssubtype { get; set; }
    public VerenigingZoekDocument.Types.SubverenigingVan? SubverenigingVan { get; set; }

    public VerenigingZoekDocument.Types.Werkingsgebied[]? Werkingsgebieden { get; set; } = null!;

    public bool? IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public string? Status { get; set; } = null!;
    public bool? IsVerwijderd { get; set; }
    public bool? IsDubbel { get; set; }
}
