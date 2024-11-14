namespace AssociationRegistry.Vereniging;

using Bronnen;
using Emails;
using Events;
using Framework;
using Marten.Schema;
using SocialMedias;
using TelefoonNummers;

public record VerenigingState : IHasVersion
{
    [Identity]
    public string Identity
    {
        get => VCode;
        set => VCode = VCode.Create(value);
    }

    public Verenigingstype Verenigingstype { get; init; } = null!;
    public VCode VCode { get; private set; } = null!;
    public KboNummer? KboNummer { get; private init; }
    public VerenigingsNaam Naam { get; private init; } = null!;
    public string? Roepnaam { get; private init; }
    public string? KorteNaam { get; private init; }
    public string? KorteBeschrijving { get; private init; }
    public Datum? Startdatum { get; private init; }
    public Datum? Einddatum { get; private init; }
    public Doelgroep? Doelgroep { get; private init; }
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; private init; }
    public Contactgegevens Contactgegevens { get; private init; } = Contactgegevens.Empty;
    public Vertegenwoordigers Vertegenwoordigers { get; private init; } = Vertegenwoordigers.Empty;
    public Locaties Locaties { get; init; } = Locaties.Empty;
    public Lidmaatschappen Lidmaatschappen { get; init; } = Lidmaatschappen.Empty;

    public HoofdactiviteitenVerenigingsloket HoofdactiviteitenVerenigingsloket { get; private init; } =
        HoofdactiviteitenVerenigingsloket.Empty;

    public Werkingsgebieden Werkingsgebieden { get; private init; } = Werkingsgebieden.Empty;
    public bool IsGestopt => Einddatum is not null;
    public bool IsIngeschrevenOpWijzigingenUitKbo { get; private init; }
    public List<string> HandledIdempotenceKeys { get; set; } = new();
    public bool IsVerwijderd { get; set; }
    public long Version { get; set; }

    public VerenigingState Apply(FeitelijkeVerenigingWerdGeregistreerd @event)
        => new()
        {
            Verenigingstype = Verenigingstype.FeitelijkeVereniging,
            VCode = VCode.Hydrate(@event.VCode),
            Naam = VerenigingsNaam.Hydrate(@event.Naam),
            KorteNaam = @event.KorteNaam,
            KorteBeschrijving = @event.KorteBeschrijving,
            Startdatum = Datum.Hydrate(@event.Startdatum),
            Doelgroep = Doelgroep.Hydrate(@event.Doelgroep.Minimumleeftijd, @event.Doelgroep.Maximumleeftijd),
            IsUitgeschrevenUitPubliekeDatastroom = @event.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = @event.Contactgegevens.Aggregate(
                Contactgegevens.Empty,
                func: (lijst, c) => Contactgegevens.Hydrate(
                    lijst.Append(
                        Contactgegeven.Hydrate(
                            c.ContactgegevenId,
                            Contactgegeventype.Parse(c.Contactgegeventype),
                            c.Waarde,
                            c.Beschrijving,
                            c.IsPrimair,
                            Bron.Initiator)))),
            Vertegenwoordigers = @event.Vertegenwoordigers.Aggregate(
                Vertegenwoordigers.Empty,
                func: (lijst, v) => Vertegenwoordigers.Hydrate(
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
                func: (lijst, l) => Locaties.Hydrate(
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
            Werkingsgebieden = Werkingsgebieden.Hydrate(
                @event.Werkingsgebieden?.Select(wg => Werkingsgebied.Hydrate(wg.Code, wg.Naam)).ToArray() ?? []),
        };

    public VerenigingState Apply(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd @event)
        => new()
        {
            Verenigingstype = Verenigingstype.Parse(@event.Rechtsvorm),
            VCode = VCode.Hydrate(@event.VCode),
            KboNummer = KboNummer.Hydrate(@event.KboNummer),
            Naam = VerenigingsNaam.Hydrate(@event.Naam),
            KorteNaam = @event.KorteNaam,
            Startdatum = Datum.Hydrate(@event.Startdatum),
        };

    public VerenigingState Apply(VerenigingWerdIngeschrevenOpWijzigingenUitKbo _)
        => this with { IsIngeschrevenOpWijzigingenUitKbo = true };

    public VerenigingState Apply(NaamWerdGewijzigd @event)
        => this with { Naam = VerenigingsNaam.Hydrate(@event.Naam) };

    public VerenigingState Apply(KorteNaamWerdGewijzigd @event)
        => this with { KorteNaam = @event.KorteNaam };

    public VerenigingState Apply(KorteBeschrijvingWerdGewijzigd @event)
        => this with { KorteBeschrijving = @event.KorteBeschrijving };

    public VerenigingState Apply(StartdatumWerdGewijzigd @event)
        => this with { Startdatum = Datum.Hydrate(@event.Startdatum) };

    public VerenigingState Apply(VerenigingWerdGestopt @event)
        => this with { Einddatum = Datum.Hydrate(@event.Einddatum) };

    public VerenigingState Apply(VerenigingWerdGestoptInKBO @event)
        => this with { Einddatum = Datum.Hydrate(@event.Einddatum) };

    public VerenigingState Apply(VerenigingWerdVerwijderd @event)
        => this with { IsVerwijderd = true };

    public VerenigingState Apply(EinddatumWerdGewijzigd @event)
        => this with { Einddatum = Datum.Hydrate(@event.Einddatum) };

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
                        Contactgegeventype.Parse(@event.Contactgegeventype),
                        @event.Waarde,
                        @event.Beschrijving,
                        @event.IsPrimair,
                        Bron.Initiator))),
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
                            Contactgegeventype.Parse(@event.Contactgegeventype),
                            @event.Waarde,
                            @event.Beschrijving,
                            @event.IsPrimair,
                            Bron.Initiator))),
        };

    public VerenigingState Apply(ContactgegevenUitKBOWerdGewijzigd @event)
    {
        var contactgegeven = Contactgegevens.Single(c => c.ContactgegevenId == @event.ContactgegevenId);

        return this with
        {
            Contactgegevens = Contactgegevens.Hydrate(
                Contactgegevens
                   .Without(@event.ContactgegevenId)
                   .Append(
                        contactgegeven with
                        {
                            Beschrijving = @event.Beschrijving,
                            IsPrimair = @event.IsPrimair,
                        })),
        };
    }

    public VerenigingState Apply(HoofdactiviteitenVerenigingsloketWerdenGewijzigd @event)
        => this with
        {
            HoofdactiviteitenVerenigingsloket = HoofdactiviteitenVerenigingsloket.Hydrate(
                @event.HoofdactiviteitenVerenigingsloket.Select(
                           h => HoofdactiviteitVerenigingsloket.Create(h.Code))
                      .ToArray()),
        };

    public VerenigingState Apply(WerkingsgebiedenWerdenGewijzigd @event)
        => this with
        {
            Werkingsgebieden = Werkingsgebieden.Hydrate(
                @event.Werkingsgebieden.Select(
                           h => Werkingsgebied.Create(h.Code))
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
                Locaties
                   .AppendFromEventData(@event.Locatie)
            ),
        };

    public VerenigingState Apply(LidmaatschapWerdToegevoegd @event)
        => this with
        {
            Lidmaatschappen = Lidmaatschappen.Hydrate(
                Lidmaatschappen
                   .AppendFromEventData(@event.Lidmaatschap)
            ),
        };

    public VerenigingState Apply(LidmaatschapWerdGewijzigd @event)
        => this with
        {
            Lidmaatschappen = Lidmaatschappen.Hydrate(
                Lidmaatschappen
                   .Without(@event.Lidmaatschap.LidmaatschapId)
                   .AppendFromEventData(@event.Lidmaatschap)
            ),
        };

    public VerenigingState Apply(LidmaatschapWerdVerwijderd @event)
        => this with
        {
            Lidmaatschappen = Lidmaatschappen.Hydrate(
                Lidmaatschappen
                   .Without(@event.Lidmaatschap.LidmaatschapId)
            ),
        };

    public VerenigingState Apply(LocatieWerdGewijzigd @event)
        => this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.Locatie.LocatieId)
                   .AppendFromEventData(@event.Locatie)
            ),
        };

    public VerenigingState Apply(MaatschappelijkeZetelVolgensKBOWerdGewijzigd @event)
    {
        var maatschappelijkeZetel = Locaties[@event.LocatieId];

        return this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.LocatieId)
                   .Append(maatschappelijkeZetel with
                    {
                        Naam = @event.Naam,
                        IsPrimair = @event.IsPrimair,
                    })
            ),
        };
    }

    public VerenigingState Apply(LocatieWerdVerwijderd @event)
        => this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.Locatie.LocatieId)),
        };

    public VerenigingState Apply(MaatschappelijkeZetelWerdOvergenomenUitKbo @event)
        => this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .AppendFromEventData(@event.Locatie)),
        };

    public VerenigingState Apply(MaatschappelijkeZetelWerdGewijzigdInKbo @event)
        => this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.Locatie.LocatieId)
                   .AppendFromEventData(@event.Locatie)),
        };

    public VerenigingState Apply(MaatschappelijkeZetelWerdVerwijderdUitKbo @event)
        => this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.Locatie.LocatieId)),
        };

    public VerenigingState Apply(VertegenwoordigerWerdOvergenomenUitKBO @event)
        => this with
        {
            Vertegenwoordigers = Vertegenwoordigers.Hydrate(
                Vertegenwoordigers.AppendFromEventData(@event)),
        };

    public VerenigingState Apply(ContactgegevenWerdOvergenomenUitKBO @event)
        => this with
        {
            Contactgegevens = Contactgegevens.Hydrate(
                Contactgegevens.Append(
                    Contactgegeven.Hydrate(
                        @event.ContactgegevenId,
                        Contactgegeventype.Parse(@event.Contactgegeventype),
                        @event.Waarde,
                        string.Empty,
                        isPrimair: false,
                        Bron.KBO,
                        ContactgegeventypeVolgensKbo.Parse(@event.TypeVolgensKbo)))),
        };

    public VerenigingState Apply(ContactgegevenWerdGewijzigdInKbo @event)
        => this with
        {
            Contactgegevens = Contactgegevens.Hydrate(
                Contactgegevens
                   .Without(@event.ContactgegevenId)
                   .Append(Contactgegeven.Hydrate(
                               @event.ContactgegevenId,
                               Contactgegeventype.Parse(@event.Contactgegeventype),
                               @event.Waarde,
                               string.Empty,
                               isPrimair: false,
                               Bron.KBO,
                               ContactgegeventypeVolgensKbo.Parse(@event.TypeVolgensKbo)))),
        };

    public VerenigingState Apply(ContactgegevenKonNietOvergenomenWordenUitKBO @event)
        => this;

    public VerenigingState Apply(MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo @event)
        => this;

    public VerenigingState Apply(RoepnaamWerdGewijzigd @event)
        => this with { Roepnaam = @event.Roepnaam };

    public VerenigingState Apply(SynchronisatieMetKboWasSuccesvol @event)
        => this;

    public VerenigingState Apply(NaamWerdGewijzigdInKbo @event)
        => this with
        {
            Naam = VerenigingsNaam.Hydrate(@event.Naam),
        };

    public VerenigingState Apply(RechtsvormWerdGewijzigdInKBO @event)
        => this with
        {
            Verenigingstype = Verenigingstype.Parse(@event.Rechtsvorm),
        };

    public VerenigingState Apply(KorteNaamWerdGewijzigdInKbo @event)
        => this with
        {
            KorteNaam = @event.KorteNaam,
        };

    public VerenigingState Apply(StartdatumWerdGewijzigdInKbo @event)
        => this with
        {
            Startdatum = Datum.Hydrate(@event.Startdatum),
        };

    public VerenigingState Apply(ContactgegevenWerdVerwijderdUitKBO @event)
        => this with
        {
            Contactgegevens = Contactgegevens.Hydrate(
                Contactgegevens.Without(@event.ContactgegevenId)),
        };

    public VerenigingState Apply(ContactgegevenWerdInBeheerGenomenDoorKbo @event)
    {
        var contactgegeven = Contactgegevens.Single(c => c.ContactgegevenId == @event.ContactgegevenId);

        return this with
        {
            Contactgegevens = Contactgegevens.Hydrate(
                Contactgegevens.Without(@event.ContactgegevenId)
                               .Append(contactgegeven with
                                {
                                    Bron = Bron.KBO,
                                })),
        };
    }

    public VerenigingState Apply(AdresWerdOvergenomenUitAdressenregister @event)
    {
        var locatie = Locaties.SingleOrDefault(x => x.LocatieId == @event.LocatieId);

        if (locatie is null)
        {
            return this;
        }

        return this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.LocatieId)
                   .Append(locatie with
                    {
                        AdresId = AdresId.Hydrate(@event.AdresId.Broncode, @event.AdresId.Bronwaarde),
                        Adres = Adres.Hydrate(@event.Adres),
                    })),
        };
    }

    public VerenigingState Apply(AdresWerdNietGevondenInAdressenregister @event)
    {
        var locatie = Locaties.SingleOrDefault(x => x.LocatieId == @event.LocatieId);

        if (locatie is null)
        {
            return this;
        }

        return this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.LocatieId)
                   .Append(locatie with
                    {
                        AdresId = null,
                    })
            ),
        };
    }

    public VerenigingState Apply(AdresKonNietOvergenomenWordenUitAdressenregister @event)
    {
        var locatie = Locaties.SingleOrDefault(x => x.LocatieId == @event.LocatieId);

        if (locatie is null)
        {
            return this;
        }

        return this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.LocatieId)
                   .Append(locatie with
                    {
                        AdresId = null,
                    })
            ),
        };
    }

    public VerenigingState Apply(AdresNietUniekInAdressenregister @event)
    {
        var locatie = Locaties.SingleOrDefault(x => x.LocatieId == @event.LocatieId);

        if (locatie is null)
        {
            return this;
        }

        return this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.LocatieId)
                   .Append(locatie with
                    {
                        AdresId = null,
                    })
            ),
        };
    }

    public VerenigingState Apply(AdresWerdGewijzigdInAdressenregister @event)
    {
        if (!HandledIdempotenceKeys.Contains(@event.IdempotenceKey))
            HandledIdempotenceKeys.Add(@event.IdempotenceKey);

        var locatie = Locaties.SingleOrDefault(x => x.LocatieId == @event.LocatieId);

        if (locatie is null)
        {
            return this;
        }

        return this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.LocatieId)
                   .Append(locatie with
                    {
                        AdresId = AdresId.Hydrate(@event.AdresId.Broncode, @event.AdresId.Bronwaarde),
                        Adres = Adres.Hydrate(@event.Adres),
                    })),
        };
    }

    public VerenigingState Apply(AdresWerdOntkoppeldVanAdressenregister @event)
    {
        var locatie = Locaties.SingleOrDefault(x => x.LocatieId == @event.LocatieId);

        if (locatie is null)
        {
            return this;
        }

        return this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.LocatieId)
                   .Append(locatie with
                    {
                        AdresId = null,
                    })
            ),
        };
    }

    public VerenigingState Apply(LocatieDuplicaatWerdVerwijderdNaAdresMatch @event)
    {
        var locatie = Locaties.SingleOrDefault(x => x.LocatieId == @event.VerwijderdeLocatieId);

        if (locatie is null)
        {
            return this;
        }

        return this with
        {
            Locaties = Locaties.Hydrate(
                Locaties
                   .Without(@event.VerwijderdeLocatieId)),
        };
    }

    public VerenigingState Apply(AdresHeeftGeenVerschillenMetAdressenregister @event)
        => this;
}
