namespace AssociationRegistry.Vereniging;

using Emails;
using Events;
using Framework;
using Marten.Schema;
using SocialMedias;
using TelefoonNummers;

public record VerenigingState : IHasVersion
{
    public long Version { get; set; }

    [Identity]
    public string Identity
    {
        get => VCode;
        set => VCode = VCode.Create(value);
    }

    public Verenigingstype Verenigingstype { get; init; } = null!;
    public VCode VCode { get; private set; } = null!;
    public VerenigingsNaam Naam { get; private init; } = null!;
    public string? KorteNaam { get; private init; }
    public string? KorteBeschrijving { get; private init; }
    public Startdatum? Startdatum { get; private init; }
    public Doelgroep? Doelgroep { get; private init; }
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; private init; }
    public Contactgegevens Contactgegevens { get; private init; } = Contactgegevens.Empty;
    public Vertegenwoordigers Vertegenwoordigers { get; private init; } = Vertegenwoordigers.Empty;
    public Locaties Locaties { get; init; } = Locaties.Empty;
    public HoofdactiviteitenVerenigingsloket HoofdactiviteitenVerenigingsloket { get; private init; } =
        HoofdactiviteitenVerenigingsloket.Empty;

    public VerenigingState Apply(FeitelijkeVerenigingWerdGeregistreerd @event)
        => new()
        {
            Verenigingstype = Verenigingstype.FeitelijkeVereniging,
            VCode = VCode.Hydrate(@event.VCode),
            Naam = VerenigingsNaam.Hydrate(@event.Naam),
            KorteNaam = @event.KorteNaam,
            KorteBeschrijving = @event.KorteBeschrijving,
            Startdatum = Startdatum.Hydrate(@event.Startdatum),
            Doelgroep = Doelgroep.Hydrate(@event.Doelgroep.Minimumleeftijd, @event.Doelgroep.Maximumleeftijd),
            IsUitgeschrevenUitPubliekeDatastroom = @event.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = @event.Contactgegevens.Aggregate(
                Contactgegevens.Empty,
                (lijst, c) => Contactgegevens.Hydrate(
                    lijst.Append(
                        Contactgegeven.Hydrate(
                            c.ContactgegevenId,
                            ContactgegevenType.Parse(c.Type),
                            c.Waarde,
                            c.Beschrijving,
                            c.IsPrimair)))),
            Vertegenwoordigers = @event.Vertegenwoordigers.Aggregate(
                Vertegenwoordigers.Empty,
                (lijst, v) => Vertegenwoordigers.Hydrate(
                    lijst.Append(
                        Vertegenwoordiger.Hydrate(
                            v.VertegenwoordigerId,
                            Insz.Hydrate(v.Insz),
                            v.Rol,
                            v.Roepnaam,
                            Voornaam.Hydrate(v.Voornaam),
                            Achternaam.Hydrate(v.Achternaam),
                            v.IsPrimair,
                            Email.Hydrate(v.Email),
                            TelefoonNummer.Hydrate(v.Telefoon),
                            TelefoonNummer.Hydrate(v.Mobiel),
                            SocialMedia.Hydrate(v.SocialMedia)
                        )))),
            Locaties = @event.Locaties.Aggregate(
                Locaties.Empty,
                (lijst, l) => Locaties.Hydrate(
                    lijst.Append(
                        Locatie.Hydrate(
                            l.LocatieId,
                            l.Naam,
                            l.IsPrimair,
                            l.Locatietype,
                            l.Adres is null
                                ? null
                                : Adres.Hydrate(
                                    l.Adres.Straatnaam,
                                    l.Adres.Huisnummer,
                                    l.Adres.Busnummer,
                                    l.Adres.Postcode,
                                    l.Adres.Gemeente,
                                    l.Adres.Land),
                            l.AdresId is null
                                ? null
                                : AdresId.Hydrate(
                                    Adresbron.Parse(l.AdresId.Broncode),
                                    l.AdresId.Bronwaarde))))),
            HoofdactiviteitenVerenigingsloket = HoofdactiviteitenVerenigingsloket.Hydrate(
                @event.HoofdactiviteitenVerenigingsloket.Select(
                        h => HoofdactiviteitVerenigingsloket.Create(h.Code))
                    .ToArray()),
        };

    public VerenigingState Apply(AfdelingWerdGeregistreerd @event)
        => new()
        {
            Verenigingstype = Verenigingstype.Afdeling,
            VCode = VCode.Hydrate(@event.VCode),
            Naam = VerenigingsNaam.Hydrate(@event.Naam),
            KorteNaam = @event.KorteNaam,
            KorteBeschrijving = @event.KorteBeschrijving,
            Startdatum = Startdatum.Hydrate(@event.Startdatum),
            Doelgroep = Doelgroep.Hydrate(@event.Doelgroep.Minimumleeftijd, @event.Doelgroep.Maximumleeftijd),
            Contactgegevens = @event.Contactgegevens.Aggregate(
                Contactgegevens.Empty,
                (lijst, c) => Contactgegevens.Hydrate(
                    lijst.Append(
                        Contactgegeven.Hydrate(
                            c.ContactgegevenId,
                            ContactgegevenType.Parse(c.Type),
                            c.Waarde,
                            c.Beschrijving,
                            c.IsPrimair)))),
            Vertegenwoordigers = @event.Vertegenwoordigers.Aggregate(
                Vertegenwoordigers.Empty,
                (lijst, v) => Vertegenwoordigers.Hydrate(
                    lijst.Append(
                        Vertegenwoordiger.Hydrate(
                            v.VertegenwoordigerId,
                            Insz.Hydrate(v.Insz),
                            v.Rol,
                            v.Roepnaam,
                            Voornaam.Hydrate(v.Voornaam),
                            Achternaam.Hydrate(v.Achternaam),
                            v.IsPrimair,
                            Email.Hydrate(v.Email),
                            TelefoonNummer.Hydrate(v.Telefoon),
                            TelefoonNummer.Hydrate(v.Mobiel),
                            SocialMedia.Hydrate(v.SocialMedia)
                        )))),
            Locaties = @event.Locaties.Aggregate(
                Locaties.Empty,
                (lijst, l) => Locaties.Hydrate(
                    lijst.Append(
                        Locatie.Hydrate(
                            l.LocatieId,
                            l.Naam,
                            l.IsPrimair,
                            l.Locatietype,
                            l.Adres is null
                                ? null
                                : Adres.Hydrate(
                                    l.Adres.Straatnaam,
                                    l.Adres.Huisnummer,
                                    l.Adres.Busnummer,
                                    l.Adres.Postcode,
                                    l.Adres.Gemeente,
                                    l.Adres.Land),
                            l.AdresId is null
                                ? null
                                : AdresId.Hydrate(
                                    Adresbron.Parse(l.AdresId.Broncode),
                                    l.AdresId.Bronwaarde))))),
            HoofdactiviteitenVerenigingsloket = HoofdactiviteitenVerenigingsloket.Hydrate(
                @event.HoofdactiviteitenVerenigingsloket.Select(
                        h => HoofdactiviteitVerenigingsloket.Create(h.Code))
                    .ToArray()),
        };

    public VerenigingState Apply(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd @event)
        => new()
        {
            Verenigingstype = Verenigingstype.VerenigingMetRechtspersoonlijkheid,
            VCode = VCode.Hydrate(@event.VCode),
            Naam = VerenigingsNaam.Hydrate(@event.Naam),
            KorteNaam = @event.KorteNaam,
        };

    public VerenigingState Apply(NaamWerdGewijzigd @event)
        => this with { Naam = VerenigingsNaam.Hydrate(@event.Naam) };

    public VerenigingState Apply(KorteNaamWerdGewijzigd @event)
        => this with { KorteNaam = @event.KorteNaam };

    public VerenigingState Apply(KorteBeschrijvingWerdGewijzigd @event)
        => this with { KorteBeschrijving = @event.KorteBeschrijving };

    public VerenigingState Apply(StartdatumWerdGewijzigd @event)
        => this with { Startdatum = Startdatum.Hydrate(@event.Startdatum) };

    public VerenigingState Apply(DoelgroepWerdGewijzigd @event)
        => this with
        {
            Doelgroep = Doelgroep.Hydrate(
                @event.Doelgroep.Minimumleeftijd,
                @event.Doelgroep.Maximumleeftijd),
        };

    public VerenigingState Apply(ContactgegevenWerdToegevoegd @event)
        => this with
        {
            Contactgegevens = Contactgegevens.Hydrate(
                Contactgegevens.Append(
                    Contactgegeven.Hydrate(
                        @event.ContactgegevenId,
                        ContactgegevenType.Parse(@event.Type),
                        @event.Waarde,
                        @event.Beschrijving,
                        @event.IsPrimair))),
        };

    public VerenigingState Apply(ContactgegevenWerdVerwijderd @event)
        => this with
        {
            Contactgegevens = Contactgegevens.Hydrate(
                Contactgegevens.Without(@event.ContactgegevenId)),
        };

    public VerenigingState Apply(ContactgegevenWerdGewijzigd @event)
        => this with
        {
            Contactgegevens = Contactgegevens.Hydrate(
                Contactgegevens
                    .Without(@event.ContactgegevenId)
                    .Append(
                        Contactgegeven.Hydrate(
                            @event.ContactgegevenId,
                            ContactgegevenType.Parse(@event.Type),
                            @event.Waarde,
                            @event.Beschrijving,
                            @event.IsPrimair))),
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
            Vertegenwoordigers = Vertegenwoordigers.Hydrate(
                Vertegenwoordigers
                    .Append(
                        Vertegenwoordiger.Hydrate(
                            @event.VertegenwoordigerId,
                            Insz.Hydrate(@event.Insz),
                            @event.Rol,
                            @event.Roepnaam,
                            Voornaam.Hydrate(@event.Voornaam),
                            Achternaam.Hydrate(@event.Achternaam),
                            @event.IsPrimair,
                            Email.Hydrate(@event.Email),
                            TelefoonNummer.Hydrate(@event.Telefoon),
                            TelefoonNummer.Hydrate(@event.Mobiel),
                            SocialMedia.Hydrate(@event.SocialMedia)
                        ))),
        };

    public VerenigingState Apply(VertegenwoordigerWerdGewijzigd @event)
    {
        var vertegenwoordiger = Vertegenwoordigers[@event.VertegenwoordigerId];
        return this with
        {
            Vertegenwoordigers = Vertegenwoordigers.Hydrate(
                Vertegenwoordigers
                    .Without(@event.VertegenwoordigerId)
                    .Append(
                        Vertegenwoordiger.Hydrate(
                            @event.VertegenwoordigerId,
                            Insz.Hydrate(vertegenwoordiger.Insz),
                            @event.Rol,
                            @event.Roepnaam,
                            Voornaam.Hydrate(@event.Voornaam),
                            Achternaam.Hydrate(@event.Achternaam),
                            @event.IsPrimair,
                            Email.Hydrate(@event.Email),
                            TelefoonNummer.Hydrate(@event.Telefoon),
                            TelefoonNummer.Hydrate(@event.Mobiel),
                            SocialMedia.Hydrate(@event.SocialMedia)
                        ))),
        };
    }

    public VerenigingState Apply(VertegenwoordigerWerdVerwijderd @event)
        => this with
        {
            Vertegenwoordigers = Vertegenwoordigers.Hydrate(
                Vertegenwoordigers
                    .Without(@event.VertegenwoordigerId)),
        };

    public VerenigingState Apply(VerenigingWerdUitgeschrevenUitPubliekeDatastroom @event)
        => this with
        {
            IsUitgeschrevenUitPubliekeDatastroom = true,
        };

    public VerenigingState Apply(VerenigingWerdIngeschrevenInPubliekeDatastroom @event)
        => this with
        {
            IsUitgeschrevenUitPubliekeDatastroom = false,
        };

    public VerenigingState Apply(LocatieWerdToegevoegd @event)
        => this with
        {
            Locaties = Locaties.Hydrate(
                Locaties.ToArray().Append(
                    Locatie.Hydrate(
                        @event.Locatie.LocatieId,
                        @event.Locatie.Naam,
                        @event.Locatie.IsPrimair,
                        @event.Locatie.Locatietype,
                        @event.Locatie.Adres is null
                            ? null
                            : Adres.Hydrate(
                                @event.Locatie.Adres.Straatnaam,
                                @event.Locatie.Adres.Huisnummer,
                                @event.Locatie.Adres.Busnummer,
                                @event.Locatie.Adres.Postcode,
                                @event.Locatie.Adres.Gemeente,
                                @event.Locatie.Adres.Land),
                        @event.Locatie.AdresId is null ? null : AdresId.Hydrate(@event.Locatie.AdresId.Broncode, @event.Locatie.AdresId.Bronwaarde))
                ).ToArray()),
        };

    public VerenigingState Apply(LocatieWerdGewijzigd @event)
        => this;

    public VerenigingState Apply(LocatieWerdVerwijderd @event)
        => this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                    .Without(@event.Locatie.LocatieId)),
        };
}
