namespace AssociationRegistry.Public.ProjectionHost.Projections.Detail;

using Events;
using Formatters;
using Framework;
using Infrastructure.Extensions;
using JsonLdContext;
using Marten.Events;
using Schema.Constants;
using Schema.Detail;
using Vereniging;
using Doelgroep = Schema.Detail.Doelgroep;
using IEvent = Marten.Events.IEvent;

public static class PubliekVerenigingDetailProjector
{
    public static PubliekVerenigingDetailDocument Create(
        IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            JsonLdMetadata = new JsonLdMetadata(
                JsonLdType.Vereniging.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.Data.VCode),
                JsonLdType.Vereniging.Type),
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Verenigingstype = new PubliekVerenigingDetailDocument.VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
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
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Contactgegeven.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                                                                     c.ContactgegevenId.ToString()),
                        JsonLdType.Contactgegeven.Type),
                    ContactgegevenId = c.ContactgegevenId,
                    Contactgegeventype = c.Contactgegeventype.ToString(),
                    Waarde = c.Waarde,
                    Beschrijving = c.Beschrijving,
                    IsPrimair = c.IsPrimair,
                }).ToArray(),
            Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties
                                                            .Select(
                                                                 loc => MapLocatie(feitelijkeVerenigingWerdGeregistreerd.Data.VCode, loc))
                                                            .ToArray(),
            HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket
                                                                                     .Select(MapHoofdactiviteit).ToArray(),
        };

    public static PubliekVerenigingDetailDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
        => new()
        {
            JsonLdMetadata = new JsonLdMetadata(
                JsonLdType.Vereniging.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
                JsonLdType.Vereniging.Type),

            VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
            Verenigingstype = new PubliekVerenigingDetailDocument.VerenigingsType
            {
                Code = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Code,
                Naam = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Naam,
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
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Sleutel.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                                                              Sleutelbron.Kbo.Waarde),
                        JsonLdType.Sleutel.Type),
                    Bron = Sleutelbron.Kbo.Waarde,
                    GestructureerdeIdentificator = new PubliekVerenigingDetailDocument.GestructureerdeIdentificator
                    {
                        JsonLdMetadata = new JsonLdMetadata(
                            JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                                Sleutelbron.Kbo.Waarde),
                            JsonLdType.GestructureerdeSleutel.Type),
                        Nummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                    },
                    Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                },
            },
        };

    private static PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket MapHoofdactiviteit(
        Registratiedata.HoofdactiviteitVerenigingsloket arg)
        => new()
        {
            JsonLdMetadata = new JsonLdMetadata(
                JsonLdType.Hoofdactiviteit.CreateWithIdValues(arg.Code),
                JsonLdType.Hoofdactiviteit.Type),
            Code = arg.Code,
            Naam = arg.Naam,
        };

    public static void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
    }

    public static void Apply(IEvent<RoepnaamWerdGewijzigd> roepnaamWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Roepnaam = roepnaamWerdGewijzigd.Data.Roepnaam;
    }

    public static void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Startdatum = startdatumWerdGewijzigd.Data.Startdatum;
    }

    public static void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;
    }

    public static void Apply(
        IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd,
        PubliekVerenigingDetailDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
    }

    public static void Apply(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Doelgroep = new Doelgroep
        {
            Minimumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd,
        };
    }

    public static void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Append(
                                                new PubliekVerenigingDetailDocument.Contactgegeven
                                                {
                                                    JsonLdMetadata = new JsonLdMetadata(
                                                        JsonLdType.Contactgegeven.CreateWithIdValues(
                                                            contactgegevenWerdToegevoegd.StreamKey!,
                                                            contactgegevenWerdToegevoegd.Data.ContactgegevenId.ToString()),
                                                        JsonLdType.Contactgegeven.Type),
                                                    ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                                                    Contactgegeventype = contactgegevenWerdToegevoegd.Data.Contactgegeventype,
                                                    Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                                                    Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                                                    IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(c => c.ContactgegevenId != contactgegevenWerdGewijzigd.Data.ContactgegevenId)
                                           .Append(
                                                new PubliekVerenigingDetailDocument.Contactgegeven
                                                {
                                                    JsonLdMetadata = new JsonLdMetadata(
                                                        JsonLdType.Contactgegeven.CreateWithIdValues(
                                                            contactgegevenWerdGewijzigd.StreamKey!,
                                                            contactgegevenWerdGewijzigd.Data.ContactgegevenId.ToString()),
                                                        JsonLdType.Contactgegeven.Type),
                                                    ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                                                    Contactgegeventype = contactgegevenWerdGewijzigd.Data.Contactgegeventype,
                                                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                                                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                                                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(c => c.ContactgegevenId != contactgegevenWerdVerwijderd.Data.ContactgegevenId)
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(IEvent<LocatieWerdToegevoegd> locatieWerdToegevoegd, PubliekVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Append(MapLocatie(document.VCode, locatieWerdToegevoegd.Data.Locatie))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(IEvent<LocatieWerdGewijzigd> locatieWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieWerdGewijzigd.Data.Locatie.LocatieId)
                                    .Append(MapLocatie(document.VCode, locatieWerdGewijzigd.Data.Locatie))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(IEvent<LocatieWerdVerwijderd> locatieWerdVerwijderd, PubliekVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieWerdVerwijderd.Data.Locatie.LocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofactiviteitenVerenigingloketWerdenGewijzigd,
        PubliekVerenigingDetailDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = hoofactiviteitenVerenigingloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket
           .Select(
                h => new PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Hoofdactiviteit.CreateWithIdValues(h.Code),
                        JsonLdType.Hoofdactiviteit.Type),
                    Code = h.Code,
                    Naam = h.Naam,
                }).ToArray();
    }

    public static void Apply(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdVerwijderdUitPubliekeDatastroom,
        PubliekVerenigingDetailDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = true;
    }

    public static void Apply(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdToegevoegdAanPubliekeDatastroom,
        PubliekVerenigingDetailDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = false;
    }

    private static PubliekVerenigingDetailDocument.Locatie MapLocatie(string vCode, Registratiedata.Locatie loc)
        => new()
        {
            JsonLdMetadata = new JsonLdMetadata(
                JsonLdType.Locatie.CreateWithIdValues(vCode, loc.LocatieId.ToString()),
                JsonLdType.Locatie.Type),
            LocatieId = loc.LocatieId,
            IsPrimair = loc.IsPrimair,
            Naam = loc.Naam,
            Locatietype = new PubliekVerenigingDetailDocument.Locatie.LocatieType()
            {
                JsonLdMetadata = new JsonLdMetadata(
                    JsonLdType.LocatieType.CreateWithIdValues(loc.Locatietype),
                    JsonLdType.LocatieType.Type),
                Naam = loc.Locatietype,
            },
            Adres = Map(vCode, loc.LocatieId, loc.Adres),
            Adresvoorstelling = loc.Adres.ToAdresString(),
            AdresId = Map(loc.AdresId),
        };

    private static PubliekVerenigingDetailDocument.Adres? Map(string vCode, int locatieId, Registratiedata.Adres? adres)
        => adres is null
            ? null
            : new PubliekVerenigingDetailDocument.Adres
            {
                JsonLdMetadata = new JsonLdMetadata(
                    JsonLdType.Adres.CreateWithIdValues(vCode, locatieId.ToString()),
                    JsonLdType.Adres.Type),
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
                                    .Append(MapLocatie(document.VCode, maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
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
    }

    public static void Apply(
        IEvent<ContactgegevenWerdOvergenomenUitKBO> contactgegevenWerdOvergenomenUitKBO,
        PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Append(
                                                new PubliekVerenigingDetailDocument.Contactgegeven
                                                {
                                                    JsonLdMetadata = new JsonLdMetadata(
                                                        JsonLdType.Contactgegeven.CreateWithIdValues(
                                                            contactgegevenWerdOvergenomenUitKBO.StreamKey!,
                                                            contactgegevenWerdOvergenomenUitKBO.Data.ContactgegevenId.ToString()),
                                                        JsonLdType.Contactgegeven.Type),
                                                    ContactgegevenId = contactgegevenWerdOvergenomenUitKBO.Data.ContactgegevenId,
                                                    Contactgegeventype = contactgegevenWerdOvergenomenUitKBO.Data.Contactgegeventype,
                                                    Beschrijving = string.Empty,
                                                    Waarde = contactgegevenWerdOvergenomenUitKBO.Data.Waarde,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(
        IEvent<ContactgegevenUitKBOWerdGewijzigd> contactgegevenUitKboWerdGewijzigd,
        PubliekVerenigingDetailDocument document)
    {
        var contactgegeven =
            document.Contactgegevens.Single(c => c.ContactgegevenId == contactgegevenUitKboWerdGewijzigd.Data.ContactgegevenId);

        contactgegeven.Beschrijving = contactgegevenUitKboWerdGewijzigd.Data.Beschrijving;
        contactgegeven.IsPrimair = contactgegevenUitKboWerdGewijzigd.Data.IsPrimair;

        document.Contactgegevens = document.Contactgegevens
                                           .Where(c => c.ContactgegevenId != contactgegevenUitKboWerdGewijzigd.Data.ContactgegevenId)
                                           .Append(contactgegeven)
                                           .OrderBy(l => l.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(IEvent<VerenigingWerdGestopt> verenigingWerdGestopt, PubliekVerenigingDetailDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
    }

    public static void UpdateMetadata(IEvent @event, PubliekVerenigingDetailDocument document)
    {
        document.DatumLaatsteAanpassing = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }
}
