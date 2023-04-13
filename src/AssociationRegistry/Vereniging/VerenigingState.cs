namespace AssociationRegistry.Vereniging;

using Events;

public record VerenigingState
{
    internal VerenigingState()
    {
    }

    public VCode VCode { get; init; } = null!;
    public VerenigingsNaam Naam { get; private init; } = null!;
    public string? KorteNaam { get; private init; }
    public string? KorteBeschrijving { get; private init; }
    public Startdatum? Startdatum { get; private init; }
    public Contactgegevens Contactgegevens { get; private init; } = Contactgegevens.Empty;

    public VerenigingState Apply(VerenigingWerdGeregistreerd @event)
        => new()
        {
            VCode = VCode.Create(@event.VCode),
            Naam = new VerenigingsNaam(@event.Naam),
            KorteNaam = @event.KorteNaam,
            KorteBeschrijving = @event.KorteBeschrijving,
            Startdatum = new Startdatum(@event.Startdatum),
            Contactgegevens = @event.Contactgegevens.Aggregate(
                Contactgegevens.Empty,
                (lijst, c) => lijst.Append(
                    Contactgegeven.FromEvent(
                        c.ContactgegevenId,
                        ContactgegevenType.Parse(c.Type),
                        c.Waarde,
                        c.Omschrijving,
                        c.IsPrimair)
                )
            ),
        };

    public VerenigingState Apply(NaamWerdGewijzigd @event)
        => this with { Naam = new VerenigingsNaam(@event.Naam) };

    public VerenigingState Apply(KorteNaamWerdGewijzigd @event)
        => this with { KorteNaam = @event.KorteNaam };

    public VerenigingState Apply(KorteBeschrijvingWerdGewijzigd @event)
        => this with { KorteBeschrijving = @event.KorteBeschrijving };

    public VerenigingState Apply(StartdatumWerdGewijzigd @event)
        => this with { Startdatum = Startdatum.Hydrate(@event.Startdatum) };

    public VerenigingState Apply(ContactgegevenWerdToegevoegd @event)
        => this with
        {
            Contactgegevens = Contactgegevens.Append(
                Contactgegeven.FromEvent(
                    @event.ContactgegevenId,
                    ContactgegevenType.Parse(@event.Type),
                    @event.Waarde,
                    @event.Omschrijving,
                    @event.IsPrimair)),
        };

    public VerenigingState Apply(ContactgegevenWerdVerwijderd @event)
        => this with
        {
            Contactgegevens = Contactgegevens.Remove(@event.ContactgegevenId),
        };

    public VerenigingState Apply(ContactgegevenWerdGewijzigd @event)
        => this with
        {
            Contactgegevens = Contactgegevens.Replace(
                Contactgegeven.FromEvent(
                    @event.ContactgegevenId,
                    ContactgegevenType.Parse(@event.Type),
                    @event.Waarde,
                    @event.Omschrijving,
                    @event.IsPrimair)),
        };
}
