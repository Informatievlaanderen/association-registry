namespace AssociationRegistry.Public.ProjectionHost.Projections.Detail;

using Events;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Schema.Detail;
using Vereniging;

public static class PubliekVerenigingDetailProjector
{
    public static PubliekVerenigingDetailDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
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
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = "Actief",
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
            HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(MapHoofdactiviteit).ToArray(),
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
            IsUitgeschrevenUitPubliekeDatastroom = false,
            DatumLaatsteAanpassing = afdelingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = "Actief",
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
            HoofdactiviteitenVerenigingsloket = afdelingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(MapHoofdactiviteit).ToArray(),
        };

    public static PubliekVerenigingDetailDocument Create(IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
        => new()
        {
            VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
            Type = new PubliekVerenigingDetailDocument.VerenigingsType
            {
                Code = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Code,
                Beschrijving = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Beschrijving,
            },
            Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
            KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = string.Empty,
            Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum,
            Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
            DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = "Actief",
            Contactgegevens = Array.Empty<PubliekVerenigingDetailDocument.Contactgegeven>(),
            Locaties = Array.Empty<PubliekVerenigingDetailDocument.Locatie>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket>(),
            Sleutels = new PubliekVerenigingDetailDocument.Sleutel[]
            {
                new()
                {
                    Bron = Verenigingsbron.Kbo.Waarde,
                    Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                },
            },
        };

    private static PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket MapHoofdactiviteit(Registratiedata.HoofdactiviteitVerenigingsloket arg)
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

    public static void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
        document.DatumLaatsteAanpassing = korteBeschrijvingWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
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
            .ToArray();

        document.DatumLaatsteAanpassing = contactgegevenWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
            .Where(c => c.ContactgegevenId != contactgegevenWerdVerwijderd.Data.ContactgegevenId)
            .ToArray()
            .ToArray();

        document.DatumLaatsteAanpassing = contactgegevenWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofactiviteitenVerenigingloketWerdenGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = hoofactiviteitenVerenigingloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket.Select(
            h => new PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
            {
                Code = h.Code,
                Beschrijving = h.Beschrijving,
            }).ToArray();
        document.DatumLaatsteAanpassing = hoofactiviteitenVerenigingloketWerdenGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    private static PubliekVerenigingDetailDocument.Locatie MapLocatie(Registratiedata.Locatie loc)
        => new()
        {
            LocatieId = loc.LocatieId,
            Hoofdlocatie = loc.Hoofdlocatie,
            Naam = loc.Naam,
            Locatietype = loc.Locatietype,
            Straatnaam = loc.Adres.Straatnaam,
            Huisnummer = loc.Adres.Huisnummer,
            Busnummer = loc.Adres.Busnummer,
            Postcode = loc.Adres.Postcode,
            Gemeente = loc.Adres.Gemeente,
            Land = loc.Adres.Land,
            Adres = loc.ToAdresString(),
            AdresId = loc.Adres.AdresId?.BronWaarde,
            Adresbron = loc.Adres.AdresId?.Broncode,
        };

    public static PubliekVerenigingDetailDocument Apply(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd, PubliekVerenigingDetailDocument moeder)
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

    public static void Apply(IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdVerwijderdUitPubliekeDatastroom, PubliekVerenigingDetailDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = true;
        document.DatumLaatsteAanpassing = verenigingWerdVerwijderdUitPubliekeDatastroom.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public static void Apply(IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdToegevoegdAanPubliekeDatastroom, PubliekVerenigingDetailDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = false;
        document.DatumLaatsteAanpassing = verenigingWerdToegevoegdAanPubliekeDatastroom.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }
}
