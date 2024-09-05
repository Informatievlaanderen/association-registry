namespace AssociationRegistry.Admin.Schema.PowerBiExport;

using Detail;
using Formats;
using Framework;
using Marten.Schema;
using IEvent = Marten.Events.IEvent;

public record Gebeurtenis(string Datum, string Tijdstip, string EventType, string Initiator, long Sequence)
{
    public static Gebeurtenis FromEvent(IEvent @event)
    {
        var instant = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip);

        return new Gebeurtenis(
            instant.ToBelgianDate(),
            instant.ToBelgianTime(),
            @event.EventType.Name,
            @event.GetHeaderString(MetadataHeaderNames.Initiator),
            @event.Sequence
        );
    }
};

public record PowerBiExportDocument : IVCode
{
    public Gebeurtenis[] Historiek { get; set; } = Array.Empty<Gebeurtenis>();
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

    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } =
        Array.Empty<HoofdactiviteitVerenigingsloket>();

    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public string Bron { get; set; } = null!;
    [Identity] public string VCode { get; init; } = null!;
    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string KboNummer { get; set; }
    public string DatumLaatsteAanpassing { get; set; }
}
