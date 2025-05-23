namespace AssociationRegistry.Public.Schema.Search;

using Detail;
using Vereniging;

public class VerenigingZoekUpdateDocument
{
    public string? VCode { get; set; } = null!;
    public string? Naam { get; set; } = null!;
    public string? Roepnaam { get; set; } = null!;
    public string? KorteNaam { get; set; } = null!;
    public string? KorteBeschrijving { get; set; } = null!;
    public VerenigingZoekDocument.Types.Doelgroep? Doelgroep { get; set; } = null!;
    public VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket[]? HoofdactiviteitenVerenigingsloket { get; set; } = null!;

    public VerenigingZoekDocument.Types.Verenigingstype? Verenigingstype { get; set; } = null!;

    public VerenigingZoekDocument.Types.Verenigingssubtype? Verenigingssubtype { get; set; }
    public VerenigingZoekDocument.Types.SubverenigingVan? SubverenigingVan { get; set; }
    public VerenigingZoekDocument.Types.Werkingsgebied[]? Werkingsgebieden { get; set; } = null!;
    public VerenigingZoekDocument.Types.Geotag[]? Geotags { get; set; } = null!;
    public bool? IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public string? Status { get; set; } = null!;
    public bool? IsVerwijderd { get; set; } = null;
    public bool? IsDubbel { get; set; } = null;
}
