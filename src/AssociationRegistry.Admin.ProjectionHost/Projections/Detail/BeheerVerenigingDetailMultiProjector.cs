namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Constants;
using Events;
using Formatters;
using Framework;
using Infrastructure.Extensions;
using JsonLdContext;
using Marten.Events;
using Schema;
using Schema.Constants;
using Schema.Detail;
using Vereniging;
using Contactgegeven = Schema.Detail.Contactgegeven;
using Doelgroep = Schema.Detail.Doelgroep;
using HoofdactiviteitVerenigingsloket = Schema.Detail.HoofdactiviteitVerenigingsloket;
using IEvent = Marten.Events.IEvent;
using Locatie = Schema.Detail.Locatie;
using Vertegenwoordiger = Schema.Detail.Vertegenwoordiger;

public class BeheerVerenigingDetailMultiProjector
{
    public static BeheerVerenigingDetailMultiDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type,
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Verenigingstype = BeheerVerenigingDetailMapper.MapVerenigingsType(Verenigingstype.FeitelijkeVereniging),
            Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            Doelgroep = BeheerVerenigingDetailMapper.MapDoelgroep(feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep,
                                                                  feitelijkeVerenigingWerdGeregistreerd.Data.VCode),
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = feitelijkeVerenigingWerdGeregistreerd.Data.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Data.Contactgegevens
                                                                   .Select(c => BeheerVerenigingDetailMapper.MapContactgegeven(
                                                                               c, feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
                                                                               feitelijkeVerenigingWerdGeregistreerd.Data.VCode))
                                                                   .ToArray(),
            Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties
                                                            .Select(loc => BeheerVerenigingDetailMapper.MapLocatie(
                                                                        loc, feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
                                                                        feitelijkeVerenigingWerdGeregistreerd.Data.VCode)).ToArray(),
            Vertegenwoordigers = feitelijkeVerenigingWerdGeregistreerd.Data.Vertegenwoordigers
                                                                      .Select(v => BeheerVerenigingDetailMapper.MapVertegenwoordiger(
                                                                                  v, feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
                                                                                  feitelijkeVerenigingWerdGeregistreerd.Data.VCode))
                                                                      .ToArray(),
            HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data
                                                                                     .HoofdactiviteitenVerenigingsloket
                                                                                     .Select(BeheerVerenigingDetailMapper
                                                                                         .MapHoofdactiviteitVerenigingsloket)
                                                                                     .ToArray(),
            Sleutels = new[] { BeheerVerenigingDetailMapper.MapVrSleutel(feitelijkeVerenigingWerdGeregistreerd.Data.VCode) },
            Bron = feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
            Metadata = new Metadata(feitelijkeVerenigingWerdGeregistreerd.Sequence, feitelijkeVerenigingWerdGeregistreerd.Version),
        };

    public static BeheerVerenigingDetailMultiDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
        => new()
        {
            JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
            VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Code,
                Naam = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Naam,
            },
            Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
            Roepnaam = string.Empty,
            KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = string.Empty,
            Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            Doelgroep = new Doelgroep
            {
                JsonLdMetadata =
                    BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Doelgroep,
                                                                      verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
                Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
            },
            Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
            DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                                        .ToBelgianDate(),
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            Contactgegevens = Array.Empty<Contactgegeven>(),
            Locaties = Array.Empty<Locatie>(),
            Vertegenwoordigers = Array.Empty<Vertegenwoordiger>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<HoofdactiviteitVerenigingsloket>(),
            Sleutels = new[]
            {
                BeheerVerenigingDetailMapper.MapVrSleutel(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
                BeheerVerenigingDetailMapper.MapKboSleutel(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                                                           verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
            },
            Bron = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Bron,
            Metadata = new Metadata(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence,
                                    verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version),
        };

    public static void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
    }

    public static void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingDetailMultiDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;
    }

    public static void Apply(IEvent<RoepnaamWerdGewijzigd> roepnaamWerdGewijzigd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Roepnaam = roepnaamWerdGewijzigd.Data.Roepnaam;
    }

    public static void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingDetailMultiDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
    }

    public static void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Startdatum = !string.IsNullOrEmpty(startdatumWerdGewijzigd.Data.Startdatum?.ToString())
            ? startdatumWerdGewijzigd.Data.Startdatum?.ToString(WellknownFormats.DateOnly)
            : null;
    }

    public static void Apply(IEvent<StartdatumWerdGewijzigdInKbo> startdatumWerdGewijzigdInKbo, BeheerVerenigingDetailMultiDocument document)
    {
        document.Startdatum = !string.IsNullOrEmpty(startdatumWerdGewijzigdInKbo.Data.Startdatum?.ToString())
            ? startdatumWerdGewijzigdInKbo.Data.Startdatum?.ToString(WellknownFormats.DateOnly)
            : null;
    }

    public static void Apply(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Doelgroep = new Doelgroep
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Doelgroep, doelgroepWerdGewijzigd.StreamKey!),
            Minimumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd,
        };
    }

    public static void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.Append(
                                                new Contactgegeven
                                                {
                                                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                                        JsonLdType.Contactgegeven, document.VCode,
                                                        contactgegevenWerdToegevoegd.Data.ContactgegevenId.ToString()),
                                                    ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                                                    Contactgegeventype = contactgegevenWerdToegevoegd.Data.Contactgegeventype,
                                                    Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                                                    Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                                                    Bron = contactgegevenWerdToegevoegd.Data.Bron,
                                                    IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .UpdateSingle(
                                                identityFunc: c => c.ContactgegevenId == contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                                                update: c => c with
                                                {
                                                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                                                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                                                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(
                                                c => c.ContactgegevenId != contactgegevenWerdVerwijderd.Data.ContactgegevenId)
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofdactiviteitenVerenigingsloketWerdenGewijzigd,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket
           .Select(BeheerVerenigingDetailMapper.MapHoofdactiviteitVerenigingsloket).ToArray();
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers.Append(
                                                   new Vertegenwoordiger
                                                   {
                                                       JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                                           JsonLdType.Vertegenwoordiger, document.VCode,
                                                           vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId.ToString()),
                                                       VertegenwoordigerId = vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId,
                                                       Insz = vertegenwoordigerWerdToegevoegd.Data.Insz,
                                                       Achternaam = vertegenwoordigerWerdToegevoegd.Data.Achternaam,
                                                       Voornaam = vertegenwoordigerWerdToegevoegd.Data.Voornaam,
                                                       Roepnaam = vertegenwoordigerWerdToegevoegd.Data.Roepnaam,
                                                       Rol = vertegenwoordigerWerdToegevoegd.Data.Rol,
                                                       IsPrimair = vertegenwoordigerWerdToegevoegd.Data.IsPrimair,
                                                       Email = vertegenwoordigerWerdToegevoegd.Data.Email,
                                                       Telefoon = vertegenwoordigerWerdToegevoegd.Data.Telefoon,
                                                       Mobiel = vertegenwoordigerWerdToegevoegd.Data.Mobiel,
                                                       SocialMedia = vertegenwoordigerWerdToegevoegd.Data.SocialMedia,
                                                       Bron = vertegenwoordigerWerdToegevoegd.Data.Bron,

                                                       VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
                                                       {
                                                           JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                                               JsonLdType.VertegenwoordigerContactgegeven, document.VCode,
                                                               vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId.ToString()),
                                                           IsPrimair = vertegenwoordigerWerdToegevoegd.Data.IsPrimair,
                                                           Email = vertegenwoordigerWerdToegevoegd.Data.Email,
                                                           Telefoon = vertegenwoordigerWerdToegevoegd.Data.Telefoon,
                                                           Mobiel = vertegenwoordigerWerdToegevoegd.Data.Mobiel,
                                                           SocialMedia = vertegenwoordigerWerdToegevoegd.Data.SocialMedia,
                                                       },
                                                   })
                                              .OrderBy(v => v.VertegenwoordigerId)
                                              .ToArray();
    }

    public static void Apply(IEvent<VertegenwoordigerWerdGewijzigd> vertegenwoordigerWerdGewijzigd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers
                                              .UpdateSingle(
                                                   identityFunc: v
                                                       => v.VertegenwoordigerId == vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId,
                                                   update: v => v with
                                                   {
                                                       Roepnaam = vertegenwoordigerWerdGewijzigd.Data.Roepnaam,
                                                       Rol = vertegenwoordigerWerdGewijzigd.Data.Rol,
                                                       IsPrimair = vertegenwoordigerWerdGewijzigd.Data.IsPrimair,
                                                       Email = vertegenwoordigerWerdGewijzigd.Data.Email,
                                                       Telefoon = vertegenwoordigerWerdGewijzigd.Data.Telefoon,
                                                       Mobiel = vertegenwoordigerWerdGewijzigd.Data.Mobiel,
                                                       SocialMedia = vertegenwoordigerWerdGewijzigd.Data.SocialMedia,
                                                       VertegenwoordigerContactgegevens = v.VertegenwoordigerContactgegevens with
                                                       {
                                                           IsPrimair = vertegenwoordigerWerdGewijzigd.Data.IsPrimair,
                                                           Email = vertegenwoordigerWerdGewijzigd.Data.Email,
                                                           Telefoon = vertegenwoordigerWerdGewijzigd.Data.Telefoon,
                                                           Mobiel = vertegenwoordigerWerdGewijzigd.Data.Mobiel,
                                                           SocialMedia = vertegenwoordigerWerdGewijzigd.Data.SocialMedia,
                                                       },
                                                   })
                                              .OrderBy(v => v.VertegenwoordigerId)
                                              .ToArray();
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers
                                              .Where(
                                                   c => c.VertegenwoordigerId != vertegenwoordigerWerdVerwijderd.Data.VertegenwoordigerId)
                                              .OrderBy(v => v.VertegenwoordigerId)
                                              .ToArray();
    }

    public static void Apply(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdVerwijderdUitPubliekeDatastroom,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = true;
    }

    public static void Apply(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdToegevoegdAanPubliekeDatastroom,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = false;
    }

    public static void Apply(IEvent<LocatieWerdToegevoegd> locatieWerdToegevoegd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .Append(BeheerVerenigingDetailMapper.MapLocatie(locatieWerdToegevoegd.Data.Locatie,
                                                                                    locatieWerdToegevoegd.Data.Bron,
                                                                                    document.VCode))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(IEvent<LocatieWerdGewijzigd> locatieWerdGewijzigd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == locatieWerdGewijzigd.Data.Locatie.LocatieId,
                                         update: l => l with
                                         {
                                             IsPrimair = locatieWerdGewijzigd.Data.Locatie.IsPrimair,
                                             Locatietype = locatieWerdGewijzigd.Data.Locatie.Locatietype,
                                             Naam = locatieWerdGewijzigd.Data.Locatie.Naam,
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 locatieWerdGewijzigd.Data.Locatie.Adres, document.VCode, l.LocatieId),
                                             Adresvoorstelling = locatieWerdGewijzigd.Data.Locatie.Adres.ToAdresString(),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 locatieWerdGewijzigd.Data.Locatie.AdresId),
                                             VerwijstNaar =
                                             BeheerVerenigingDetailMapper.MapAdresVerwijzing(locatieWerdGewijzigd.Data.Locatie.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(IEvent<LocatieWerdVerwijderd> locatieWerdVerwijderd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieWerdVerwijderd.Data.Locatie.LocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> maatschappelijkeZetelWerdOvergenomenUitKbo,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .Append(BeheerVerenigingDetailMapper.MapLocatie(maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie,
                                                                                    maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Bron,
                                                                                    document.VCode))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> maatschappelijkeZetelVolgensKboWerdGewijzigd,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId,
                                         update: l => l with
                                         {
                                             IsPrimair = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.IsPrimair,
                                             Naam = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.Naam,
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<ContactgegevenWerdOvergenomenUitKBO> contactgegevenWerdToegevoegd,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.Append(
                                                new Contactgegeven
                                                {
                                                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                                        JsonLdType.Contactgegeven, document.VCode,
                                                        contactgegevenWerdToegevoegd.Data.ContactgegevenId.ToString()),
                                                    ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                                                    Contactgegeventype = contactgegevenWerdToegevoegd.Data.Contactgegeventype,
                                                    Beschrijving = string.Empty,
                                                    Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                                                    Bron = contactgegevenWerdToegevoegd.Data.Bron,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(
        IEvent<ContactgegevenUitKBOWerdGewijzigd> contactgegevenUitKboWerdGewijzigd,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.UpdateSingle(
                                                identityFunc: c
                                                    => c.ContactgegevenId == contactgegevenUitKboWerdGewijzigd.Data.ContactgegevenId,
                                                update: contactgegeven => contactgegeven with
                                                {
                                                    IsPrimair = contactgegevenUitKboWerdGewijzigd.Data.IsPrimair,
                                                    Beschrijving = contactgegevenUitKboWerdGewijzigd.Data.Beschrijving,
                                                }
                                            ).OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(IEvent<VerenigingWerdGestopt> verenigingWerdGestopt, BeheerVerenigingDetailMultiDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
        document.Einddatum = verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly);
    }

    public static void Apply(IEvent<VerenigingWerdGestoptInKBO> verenigingWerdGestoptInKbo, BeheerVerenigingDetailMultiDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
        document.Einddatum = verenigingWerdGestoptInKbo.Data.Einddatum.ToString(WellknownFormats.DateOnly);
    }

    public static void Apply(IEvent<EinddatumWerdGewijzigd> einddatumWerdGewijzigd, BeheerVerenigingDetailMultiDocument document)
    {
        document.Einddatum = einddatumWerdGewijzigd.Data.Einddatum.ToString(WellknownFormats.DateOnly);
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> vertegenwoordigerWerdOvergenomenUitKbo,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers.Append(
                                                   new Vertegenwoordiger
                                                   {
                                                       JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                                           JsonLdType.Vertegenwoordiger, document.VCode,
                                                           vertegenwoordigerWerdOvergenomenUitKbo.Data.VertegenwoordigerId.ToString()),
                                                       VertegenwoordigerId =
                                                           vertegenwoordigerWerdOvergenomenUitKbo.Data.VertegenwoordigerId,
                                                       Insz = vertegenwoordigerWerdOvergenomenUitKbo.Data.Insz,
                                                       Achternaam = vertegenwoordigerWerdOvergenomenUitKbo.Data.Achternaam,
                                                       Voornaam = vertegenwoordigerWerdOvergenomenUitKbo.Data.Voornaam,
                                                       Roepnaam = string.Empty,
                                                       Rol = string.Empty,
                                                       IsPrimair = false,
                                                       Email = string.Empty,
                                                       Telefoon = string.Empty,
                                                       Mobiel = string.Empty,
                                                       SocialMedia = string.Empty,
                                                       Bron = vertegenwoordigerWerdOvergenomenUitKbo.Data.Bron,
                                                       VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
                                                       {
                                                           JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                                               JsonLdType.VertegenwoordigerContactgegeven, document.VCode,
                                                               vertegenwoordigerWerdOvergenomenUitKbo.Data.VertegenwoordigerId.ToString()),
                                                           IsPrimair = false,
                                                           Email = string.Empty,
                                                           Telefoon = string.Empty,
                                                           Mobiel = string.Empty,
                                                           SocialMedia = string.Empty,
                                                       },
                                                   })
                                              .OrderBy(v => v.VertegenwoordigerId)
                                              .ToArray();
    }

    public static void UpdateMetadata(IEvent e, BeheerVerenigingDetailMultiDocument document)
    {
        document.DatumLaatsteAanpassing = e.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(e.Sequence, e.Version);
    }

    public static void Apply(IEvent<NaamWerdGewijzigdInKbo> naamWerdGewijzigdInKbo, BeheerVerenigingDetailMultiDocument document)
    {
        document.Naam = naamWerdGewijzigdInKbo.Data.Naam;
    }

    public static void Apply(IEvent<KorteNaamWerdGewijzigdInKbo> korteNaamWerdGewijzigdInKbo, BeheerVerenigingDetailMultiDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigdInKbo.Data.KorteNaam;
    }

    public static void Apply(
        IEvent<ContactgegevenWerdGewijzigdInKbo> contactgegevenWerdGewijzigdUitKbo,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.UpdateSingle(
                                                identityFunc: c
                                                    => c.ContactgegevenId == contactgegevenWerdGewijzigdUitKbo.Data.ContactgegevenId,
                                                update: contactgegeven => contactgegeven with
                                                {
                                                    Waarde = contactgegevenWerdGewijzigdUitKbo.Data.Waarde,
                                                }
                                            ).OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(
        IEvent<ContactgegevenWerdVerwijderdUitKBO> contactgegevenWerdVerwijderdUitKbo,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(
                                                c => c.ContactgegevenId != contactgegevenWerdVerwijderdUitKbo.Data.ContactgegevenId)
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(
        IEvent<ContactgegevenWerdInBeheerGenomenDoorKbo> contactgegevenWerdInBeheerGenomenDoorKbo,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .UpdateSingle(
                                                identityFunc: c
                                                    => c.ContactgegevenId == contactgegevenWerdInBeheerGenomenDoorKbo.Data.ContactgegevenId,
                                                update: c => c with
                                                {
                                                    Bron = contactgegevenWerdInBeheerGenomenDoorKbo.Data.Bron,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo> maatschappelijkeZetelWerdGewijzigdInKbo,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.LocatieId,
                                         update: l => l with
                                         {
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres, document.VCode, l.LocatieId),
                                             Adresvoorstelling = maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres.ToAdresString(),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId),
                                             VerwijstNaar =
                                             BeheerVerenigingDetailMapper.MapAdresVerwijzing(
                                                 maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo> maatschappelijkeZetelWerdVerwijderdUitKbo,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != maatschappelijkeZetelWerdVerwijderdUitKbo.Data.Locatie.LocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<AdresWerdOvergenomenUitAdressenregister> adresWerdOvergenomenUitAdressenregister,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresWerdOvergenomenUitAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 adresWerdOvergenomenUitAdressenregister.Data.Adres, document.VCode,
                                                 l.LocatieId),
                                             Adresvoorstelling = adresWerdOvergenomenUitAdressenregister.Data.Adres
                                                .ToAdresString(),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 adresWerdOvergenomenUitAdressenregister.Data.AdresId),
                                             VerwijstNaar = BeheerVerenigingDetailMapper.MapAdresVerwijzing(
                                                 adresWerdOvergenomenUitAdressenregister.Data.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<AdresWerdGewijzigdInAdressenregister> adresWerdGewijzigdInAdressenregister,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresWerdGewijzigdInAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 adresWerdGewijzigdInAdressenregister.Data.Adres, document.VCode,
                                                 l.LocatieId),
                                             Adresvoorstelling = adresWerdGewijzigdInAdressenregister.Data.Adres
                                                .ToAdresString(),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 adresWerdGewijzigdInAdressenregister.Data.AdresId),
                                             VerwijstNaar = BeheerVerenigingDetailMapper.MapAdresVerwijzing(
                                                 adresWerdGewijzigdInAdressenregister.Data.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(IEvent<RechtsvormWerdGewijzigdInKBO> rechtsvormWerdGewijzigdInKbo, BeheerVerenigingDetailMultiDocument document)
    {
        document.Verenigingstype = new VerenigingsType
        {
            Code = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Code,
            Naam = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Naam,
        };

        document.Rechtsvorm = rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm;
    }

    public static void Apply(
        IEvent<AdresWerdNietGevondenInAdressenregister> adresWerdNietGevondenInAdressenregister,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresWerdNietGevondenInAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             AdresId = null,
                                             VerwijstNaar = null,
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<AdresNietUniekInAdressenregister> adresNietUniekInAdressenregister,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresNietUniekInAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             AdresId = null,
                                             VerwijstNaar = null,
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<AdresKonNietOvergenomenWordenUitAdressenregister> adresKonNietOvergenomenWordenUitAdressenregister,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingleOrNothing(
                                         identityFunc: l => l.LocatieId == adresKonNietOvergenomenWordenUitAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             AdresId = null,
                                             VerwijstNaar = null,
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<AdresWerdOntkoppeldVanAdressenregister> adresWerdOntkoppeldVanAdressenregister,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresWerdOntkoppeldVanAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             AdresId = null,
                                             VerwijstNaar = null,
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }


    public static void Apply(
        IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch> locatieDuplicaatWerdVerwijderdNaAdresMatch,
        BeheerVerenigingDetailMultiDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieDuplicaatWerdVerwijderdNaAdresMatch.Data.VerwijderdeLocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }
}