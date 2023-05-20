namespace AssociationRegistry.Vereniging;

using Emails;
using Events;
using SocialMedias;
using TelefoonNummers;

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
    public Vertegenwoordigers Vertegenwoordigers { get; private init; } = Vertegenwoordigers.Empty;
    public HoofdactiviteitenVerenigingsloket HoofdactiviteitenVerenigingsloket { get; private init; } = HoofdactiviteitenVerenigingsloket.Empty;

    public VerenigingState Apply(FeitelijkeVerenigingWerdGeregistreerd @event)
        => new()
        {
            VCode = VCode.Hydrate(@event.VCode),
            Naam = VerenigingsNaam.Hydrate(@event.Naam),
            KorteNaam = @event.KorteNaam,
            KorteBeschrijving = @event.KorteBeschrijving,
            Startdatum = Startdatum.Hydrate(@event.Startdatum),
            Contactgegevens = @event.Contactgegevens.Aggregate(
                Contactgegevens.Empty,
                (lijst, c) => lijst.Append(
                    Contactgegeven.Hydrate(
                        c.ContactgegevenId,
                        ContactgegevenType.Parse(c.Type),
                        c.Waarde,
                        c.Beschrijving,
                        c.IsPrimair)
                )
            ),
            Vertegenwoordigers = @event.Vertegenwoordigers.Aggregate(
                Vertegenwoordigers.Empty,
                (lijst, v) => lijst.Append(
                    Vertegenwoordiger.Hydrate(
                        v.VertegenwoordigerId,
                        Insz.Hydrate(v.Insz),
                        v.Rol,
                        v.Roepnaam,
                        v.Voornaam,
                        v.Achternaam,
                        v.IsPrimair,
                        Email.Hydrate(v.Email),
                        TelefoonNummer.Hydrate(v.Telefoon),
                        TelefoonNummer.Hydrate(v.Mobiel),
                        SocialMedia.Hydrate(v.SocialMedia)
                    ))),
            HoofdactiviteitenVerenigingsloket = HoofdactiviteitenVerenigingsloket.Hydrate(
                @event.HoofdactiviteitenVerenigingsloket.Select(
                        h => HoofdactiviteitVerenigingsloket.Create(h.Code))
                    .ToArray()),
        };

    public VerenigingState Apply(NaamWerdGewijzigd @event)
        => this with { Naam = VerenigingsNaam.Hydrate(@event.Naam) };

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
                Contactgegeven.Hydrate(
                    @event.ContactgegevenId,
                    ContactgegevenType.Parse(@event.Type),
                    @event.Waarde,
                    @event.Beschrijving,
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
                Contactgegeven.Hydrate(
                    @event.ContactgegevenId,
                    ContactgegevenType.Parse(@event.Type),
                    @event.Waarde,
                    @event.Beschrijving,
                    @event.IsPrimair)),
        };

    public VerenigingState Apply(HoofdactiviteitenVerenigingsloketWerdenGewijzigd @event)
        => this with
        {
            HoofdactiviteitenVerenigingsloket = HoofdactiviteitenVerenigingsloket.Hydrate(
                @event.HoofdactiviteitenVerenigingsloket.Select(
                        h => HoofdactiviteitVerenigingsloket.Create(h.Code))
                    .ToArray()),
        };

    public VerenigingState Apply(VertegenwoordigerWerdToegevoegd @event)
        => this with
        {
            Vertegenwoordigers = Vertegenwoordigers.Append(
                Vertegenwoordiger.Hydrate(
                    @event.VertegenwoordigerId,
                    Insz.Hydrate(@event.Insz),
                    @event.Rol,
                    @event.Roepnaam,
                    @event.Voornaam,
                    @event.Achternaam,
                    @event.IsPrimair,
                    Email.Hydrate(@event.Email),
                    TelefoonNummer.Hydrate(@event.Telefoon),
                    TelefoonNummer.Hydrate(@event.Mobiel),
                    SocialMedia.Hydrate(@event.SocialMedia)
                )),
        };

    public VerenigingState Apply(VertegenwoordigerWerdGewijzigd @event)
    {
        var vertegenwoordiger = Vertegenwoordigers[@event.VertegenwoordigerId];
        return this with
        {
            Vertegenwoordigers = Vertegenwoordigers.Replace(
                Vertegenwoordiger.Hydrate(
                    @event.VertegenwoordigerId,
                    Insz.Hydrate(vertegenwoordiger.Insz),
                    @event.Rol,
                    @event.Roepnaam,
                    vertegenwoordiger.Voornaam,
                    vertegenwoordiger.Achternaam,
                    @event.IsPrimair,
                    Email.Hydrate(@event.Email),
                    TelefoonNummer.Hydrate(@event.Telefoon),
                    TelefoonNummer.Hydrate(@event.Mobiel),
                    SocialMedia.Hydrate(@event.SocialMedia)
                )),
        };
    }

    public VerenigingState Apply(VertegenwoordigerWerdVerwijderd @event)
        => this with
        {
            Vertegenwoordigers = Vertegenwoordigers.Remove(@event.VertegenwoordigerId),
        };
}
