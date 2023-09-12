namespace AssociationRegistry.Public.ProjectionHost.Projections.Detail;

using Events;
using Formatters;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Schema.Constants;
using Schema.Detail;
using Vereniging;
using Doelgroep = Schema.Detail.Doelgroep;

public static class PubliekVerenigingDetailProjector
{
    public static PubliekVerenigingDetailDocument Create(
        IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Type = new PubliekVerenigingDetailDocument.VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Beschrijving = Verenigingstype.FeitelijkeVereniging.Beschrijving,
            },
            Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
            IsUitgeschrevenUitPubliekeDatastroom = feitelijkeVerenigingWerdGeregistreerd.Data.IsUitgeschrevenUitPubliekeDatastroom,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum,
            Doelgroep = MapDoelgroep(feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep),
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = VerenigingStatus.Actief,
            Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Data.Contactgegevens.Select(
                c => new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = c.ContactgegevenId,
                    Type = c.Type.ToString(),
                    Waarde = c.Waarde,
                    Beschrijving = c.Beschrijving,
                    IsPrimair = c.IsPrimair,
                }).ToArray(),
            Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties.Select(MapLocatie).ToArray(),
            HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket
                                                                                     .Select(MapHoofdactiviteit).ToArray(),
        };

    public static PubliekVerenigingDetailDocument Create(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd)
        => new()
        {
            VCode = afdelingWerdGeregistreerd.Data.VCode,
            Type = new PubliekVerenigingDetailDocument.VerenigingsType
            {
                Code = Verenigingstype.Afdeling.Code,
                Beschrijving = Verenigingstype.Afdeling.Beschrijving,
            },
            Naam = afdelingWerdGeregistreerd.Data.Naam,
            KorteNaam = afdelingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = afdelingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = afdelingWerdGeregistreerd.Data.Startdatum,
            Doelgroep = MapDoelgroep(afdelingWerdGeregistreerd.Data.Doelgroep),
            IsUitgeschrevenUitPubliekeDatastroom = false,
            DatumLaatsteAanpassing = afdelingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = VerenigingStatus.Actief,
            Contactgegevens = afdelingWerdGeregistreerd.Data.Contactgegevens.Select(
                c => new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = c.ContactgegevenId,
                    Type = c.Type.ToString(),
                    Waarde = c.Waarde,
                    Beschrijving = c.Beschrijving,
                    IsPrimair = c.IsPrimair,
                }).ToArray(),
            Locaties = afdelingWerdGeregistreerd.Data.Locaties.Select(MapLocatie).ToArray(),
            Relaties = new[]
            {
                new PubliekVerenigingDetailDocument.Relatie
                {
                    Type = RelatieType.IsAfdelingVan.Beschrijving,
                    AndereVereniging = new PubliekVerenigingDetailDocument.Relatie.GerelateerdeVereniging
                    {
                        KboNummer = afdelingWerdGeregistreerd.Data.Moedervereniging.KboNummer,
                        VCode = afdelingWerdGeregistreerd.Data.Moedervereniging.VCode,
                        Naam = afdelingWerdGeregistreerd.Data.Moedervereniging.Naam,
                    },
                },
            },
            HoofdactiviteitenVerenigingsloket =
                afdelingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(MapHoofdactiviteit).ToArray(),
        };

    public static PubliekVerenigingDetailDocument Apply(
        IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd,
        PubliekVerenigingDetailDocument moeder)
    {
        moeder.Relaties = moeder.Relaties.Append(
            new PubliekVerenigingDetailDocument.Relatie
            {
                Type = RelatieType.IsAfdelingVan.InverseBeschrijving,
                AndereVereniging = new PubliekVerenigingDetailDocument.Relatie.GerelateerdeVereniging
                {
                    KboNummer = string.Empty,
                    Naam = afdelingWerdGeregistreerd.Data.Naam,
                    VCode = afdelingWerdGeregistreerd.Data.VCode,
                },
            }).ToArray();

        return moeder;
    }

    public static PubliekVerenigingDetailDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
        => new()
        {
            VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
            Type = new PubliekVerenigingDetailDocument.VerenigingsType
            {
                Code = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Code,
                Beschrijving = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Beschrijving,
            },
            Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
            Roepnaam = string.Empty,
            KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = string.Empty,
            Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum,
            Doelgroep = new Doelgroep
            {
                Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
            },
            Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
            DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                                        .ToBelgianDate(),
            Status = VerenigingStatus.Actief,
            Contactgegevens = Array.Empty<PubliekVerenigingDetailDocument.Contactgegeven>(),
            Locaties = Array.Empty<PubliekVerenigingDetailDocument.Locatie>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket>(),
            Sleutels = new PubliekVerenigingDetailDocument.Sleutel[]
            {
                new()
                {
                    Bron = Sleutelbron.Kbo.Waarde,
                    Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                },
            },
        };

    private static PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket MapHoofdactiviteit(
        Registratiedata.HoofdactiviteitVerenigingsloket arg)
        => new()
        {
            Code = arg.Code,
            Beschrijving = arg.Beschrijving,
        };

    public static void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
        document.DatumLaatsteAanpassing = naamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<RoepnaamWerdGewijzigd> roepnaamWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Roepnaam = roepnaamWerdGewijzigd.Data.Roepnaam;
        document.DatumLaatsteAanpassing = roepnaamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Startdatum = startdatumWerdGewijzigd.Data.Startdatum;
        document.DatumLaatsteAanpassing = startdatumWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;
        document.DatumLaatsteAanpassing = korteNaamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(
        IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd,
        PubliekVerenigingDetailDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
        document.DatumLaatsteAanpassing = korteBeschrijvingWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Doelgroep = new Doelgroep
        {
            Minimumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd,
        };

        document.DatumLaatsteAanpassing = doelgroepWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Append(
                                                new PubliekVerenigingDetailDocument.Contactgegeven
                                                {
                                                    ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                                                    Type = contactgegevenWerdToegevoegd.Data.Type,
                                                    Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                                                    Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                                                    IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();

        document.DatumLaatsteAanpassing = contactgegevenWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(c => c.ContactgegevenId != contactgegevenWerdGewijzigd.Data.ContactgegevenId)
                                           .Append(
                                                new PubliekVerenigingDetailDocument.Contactgegeven
                                                {
                                                    ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                                                    Type = contactgegevenWerdGewijzigd.Data.Type,
                                                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                                                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                                                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();

        document.DatumLaatsteAanpassing = contactgegevenWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(c => c.ContactgegevenId != contactgegevenWerdVerwijderd.Data.ContactgegevenId)
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();

        document.DatumLaatsteAanpassing = contactgegevenWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<LocatieWerdToegevoegd> locatieWerdToegevoegd, PubliekVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Append(MapLocatie(locatieWerdToegevoegd.Data.Locatie))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing = locatieWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<LocatieWerdGewijzigd> locatieWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieWerdGewijzigd.Data.Locatie.LocatieId)
                                    .Append(MapLocatie(locatieWerdGewijzigd.Data.Locatie))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing = locatieWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<LocatieWerdVerwijderd> locatieWerdVerwijderd, PubliekVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieWerdVerwijderd.Data.Locatie.LocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing = locatieWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofactiviteitenVerenigingloketWerdenGewijzigd,
        PubliekVerenigingDetailDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = hoofactiviteitenVerenigingloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket
           .Select(
                h => new PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
                {
                    Code = h.Code,
                    Beschrijving = h.Beschrijving,
                }).ToArray();

        document.DatumLaatsteAanpassing = hoofactiviteitenVerenigingloketWerdenGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                                        .ToBelgianDate();
    }

    public static void Apply(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdVerwijderdUitPubliekeDatastroom,
        PubliekVerenigingDetailDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = true;

        document.DatumLaatsteAanpassing =
            verenigingWerdVerwijderdUitPubliekeDatastroom.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdToegevoegdAanPubliekeDatastroom,
        PubliekVerenigingDetailDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = false;

        document.DatumLaatsteAanpassing =
            verenigingWerdToegevoegdAanPubliekeDatastroom.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    private static PubliekVerenigingDetailDocument.Locatie MapLocatie(Registratiedata.Locatie loc)
        => new()
        {
            LocatieId = loc.LocatieId,
            IsPrimair = loc.IsPrimair,
            Naam = loc.Naam,
            Locatietype = loc.Locatietype,
            Adres = Map(loc.Adres),
            Adresvoorstelling = loc.Adres.ToAdresString(),
            AdresId = Map(loc.AdresId),
        };

    private static PubliekVerenigingDetailDocument.Adres? Map(Registratiedata.Adres? adres)
        => adres is null
            ? null
            : new PubliekVerenigingDetailDocument.Adres
            {
                Straatnaam = adres.Straatnaam,
                Huisnummer = adres.Huisnummer,
                Busnummer = adres.Busnummer,
                Postcode = adres.Postcode,
                Gemeente = adres.Gemeente,
                Land = adres.Land,
            };

    private static PubliekVerenigingDetailDocument.AdresId? Map(Registratiedata.AdresId? locAdresId)
        => locAdresId is null
            ? null
            : new PubliekVerenigingDetailDocument.AdresId
            {
                Bronwaarde = locAdresId.Bronwaarde,
                Broncode = locAdresId.Broncode,
            };

    private static Doelgroep MapDoelgroep(Registratiedata.Doelgroep doelgroep)
        => new()
        {
            Minimumleeftijd = doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroep.Maximumleeftijd,
        };

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> maatschappelijkeZetelWerdOvergenomenUitKbo,
        PubliekVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Append(MapLocatie(maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing =
            maatschappelijkeZetelWerdOvergenomenUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> maatschappelijkeZetelVolgensKboWerdGewijzigd,
        PubliekVerenigingDetailDocument document)
    {
        var maatschappelijkeZetel =
            document.Locaties.Single(l => l.LocatieId == maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId);

        maatschappelijkeZetel.Naam = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.Naam;
        maatschappelijkeZetel.IsPrimair = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.IsPrimair;

        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId)
                                    .Append(maatschappelijkeZetel)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing =
            maatschappelijkeZetelVolgensKboWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(
        IEvent<ContactgegevenWerdOvergenomenUitKBO> contactgegevenWerdOvergenomenUitKBO,
        PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Append(
                                                new PubliekVerenigingDetailDocument.Contactgegeven
                                                {
                                                    ContactgegevenId = contactgegevenWerdOvergenomenUitKBO.Data.ContactgegevenId,
                                                    Type = contactgegevenWerdOvergenomenUitKBO.Data.Type,
                                                    Beschrijving = string.Empty,
                                                    Waarde = contactgegevenWerdOvergenomenUitKBO.Data.Waarde,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();

        document.DatumLaatsteAanpassing =
            contactgegevenWerdOvergenomenUitKBO.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<VerenigingWerdGestopt> verenigingWerdGestopt, PubliekVerenigingDetailDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;

        document.DatumLaatsteAanpassing = verenigingWerdGestopt.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }
}
