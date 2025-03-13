namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Events;
using Formats;
using Framework;
using JsonLdContext;
using Marten.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Schema;
using Schema.Detail;
using Vereniging;

using Contactgegeven = Schema.Detail.Contactgegeven;
using Doelgroep = Schema.Detail.Doelgroep;
using IEvent = Marten.Events.IEvent;
using SubverenigingVan = Schema.Detail.SubverenigingVan;
using Verenigingssubtype = Vereniging.Verenigingssubtype;
using VerenigingStatus = Schema.Constants.VerenigingStatus;
using Verenigingstype = Schema.Detail.Verenigingstype;
using Vertegenwoordiger = Schema.Detail.Vertegenwoordiger;
using WellknownFormats = Constants.WellknownFormats;
using Werkingsgebied = Schema.Detail.Werkingsgebied;

public class BeheerVerenigingDetailProjector
{
    public static BeheerVerenigingDetailDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type,
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Verenigingstype = BeheerVerenigingDetailMapper.MapVerenigingstype(AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging),
            Verenigingssubtype = null,
            Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            Doelgroep = BeheerVerenigingDetailMapper.MapDoelgroep(feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep,
                                                                  feitelijkeVerenigingWerdGeregistreerd.Data.VCode),
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                          .FormatAsBelgianDate(),
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = feitelijkeVerenigingWerdGeregistreerd.Data.IsUitgeschrevenUitPubliekeDatastroom,
            IsDubbelVan = "",
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
            Werkingsgebieden = [],

            Sleutels = [BeheerVerenigingDetailMapper.MapVrSleutel(feitelijkeVerenigingWerdGeregistreerd.Data.VCode)],
            Bron = feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
            Metadata = new Metadata(feitelijkeVerenigingWerdGeregistreerd.Sequence, feitelijkeVerenigingWerdGeregistreerd.Version),
        };

    public static BeheerVerenigingDetailDocument Create(IEvent<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type,
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Verenigingstype = BeheerVerenigingDetailMapper.MapVerenigingstype(AssociationRegistry.Vereniging.Verenigingstype.VZER),
            Verenigingssubtype = BeheerVerenigingDetailMapper.MapSubtype(Verenigingssubtype.NietBepaald),
            Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            Doelgroep = BeheerVerenigingDetailMapper.MapDoelgroep(feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep,
                                                                  feitelijkeVerenigingWerdGeregistreerd.Data.VCode),
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                          .FormatAsBelgianDate(),
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = feitelijkeVerenigingWerdGeregistreerd.Data.IsUitgeschrevenUitPubliekeDatastroom,
            IsDubbelVan = "",
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
            Werkingsgebieden = [],

            Sleutels = [BeheerVerenigingDetailMapper.MapVrSleutel(feitelijkeVerenigingWerdGeregistreerd.Data.VCode)],
            Bron = feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
            Metadata = new Metadata(feitelijkeVerenigingWerdGeregistreerd.Sequence, feitelijkeVerenigingWerdGeregistreerd.Version),
        };

    public static BeheerVerenigingDetailDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
        => new()
        {
            JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
            VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
            Verenigingstype = new Verenigingstype
            {
                Code = AssociationRegistry.Vereniging.Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Naam,
            },
            Verenigingssubtype = null,
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
            DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).FormatAsBelgianDate(),
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            IsDubbelVan = "",
            Contactgegevens = [],
            Locaties = [],
            Vertegenwoordigers = [],
            HoofdactiviteitenVerenigingsloket = [],
            Werkingsgebieden = [],
            Sleutels =
            [
                BeheerVerenigingDetailMapper.MapVrSleutel(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
                BeheerVerenigingDetailMapper.MapKboSleutel(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                                                           verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
            ],
            Bron = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Bron,
            Metadata = new Metadata(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence,
                                    verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version),
        };

    public static void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
    }

    public static void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;
    }

    public static void Apply(IEvent<RoepnaamWerdGewijzigd> roepnaamWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Roepnaam = roepnaamWerdGewijzigd.Data.Roepnaam;
    }

    public static void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
    }

    public static void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Startdatum = !string.IsNullOrEmpty(startdatumWerdGewijzigd.Data.Startdatum?.ToString())
            ? startdatumWerdGewijzigd.Data.Startdatum?.ToString(WellknownFormats.DateOnly)
            : null;
    }

    public static void Apply(IEvent<StartdatumWerdGewijzigdInKbo> startdatumWerdGewijzigdInKbo, BeheerVerenigingDetailDocument document)
    {
        document.Startdatum = !string.IsNullOrEmpty(startdatumWerdGewijzigdInKbo.Data.Startdatum?.ToString())
            ? startdatumWerdGewijzigdInKbo.Data.Startdatum?.ToString(WellknownFormats.DateOnly)
            : null;
    }

    public static void Apply(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Doelgroep = new Doelgroep
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Doelgroep, doelgroepWerdGewijzigd.StreamKey!),
            Minimumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd,
        };
    }

    public static void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, BeheerVerenigingDetailDocument document)
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

    public static void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, BeheerVerenigingDetailDocument document)
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

    public static void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, BeheerVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(
                                                c => c.ContactgegevenId != contactgegevenWerdVerwijderd.Data.ContactgegevenId)
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofdactiviteitenVerenigingsloketWerdenGewijzigd,
        BeheerVerenigingDetailDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket
           .Select(BeheerVerenigingDetailMapper.MapHoofdactiviteitVerenigingsloket).ToArray();
    }

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenNietBepaald> werkingsgebiedenWerdNietBepaald,
        BeheerVerenigingDetailDocument document)
    {
        document.Werkingsgebieden = [];
    }

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenBepaald> werkingsgebiedenWerdenBepaald,
        BeheerVerenigingDetailDocument document)
    {
        document.Werkingsgebieden = werkingsgebiedenWerdenBepaald.Data.Werkingsgebieden
                                                                   .Select(BeheerVerenigingDetailMapper.MapWerkingsgebied).ToArray();
    }

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenGewijzigd> werkingsgebiedenWerdenGewijzigd,
        BeheerVerenigingDetailDocument document)
    {
        document.Werkingsgebieden = werkingsgebiedenWerdenGewijzigd.Data.Werkingsgebieden
                                                                   .Select(BeheerVerenigingDetailMapper.MapWerkingsgebied).ToArray();
    }

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenNietVanToepassing> werkingsgebiedenWerdenNietVanToepasssing,
        BeheerVerenigingDetailDocument document)
    {
        document.Werkingsgebieden = [
            new Werkingsgebied
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                    JsonLdType.Werkingsgebied,
                    AssociationRegistry.Vereniging.Werkingsgebied.NietVanToepassing.Code),

                Code = AssociationRegistry.Vereniging.Werkingsgebied.NietVanToepassing.Code,
                Naam = AssociationRegistry.Vereniging.Werkingsgebied.NietVanToepassing.Naam,
            },
        ];
    }


    public static void Apply(
        IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd,
        BeheerVerenigingDetailDocument document)
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

    public static void Apply(IEvent<VertegenwoordigerWerdGewijzigd> vertegenwoordigerWerdGewijzigd, BeheerVerenigingDetailDocument document)
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
        BeheerVerenigingDetailDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers
                                              .Where(
                                                   c => c.VertegenwoordigerId != vertegenwoordigerWerdVerwijderd.Data.VertegenwoordigerId)
                                              .OrderBy(v => v.VertegenwoordigerId)
                                              .ToArray();
    }

    public static void Apply(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdVerwijderdUitPubliekeDatastroom,
        BeheerVerenigingDetailDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = true;
    }

    public static void Apply(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdToegevoegdAanPubliekeDatastroom,
        BeheerVerenigingDetailDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = false;
    }

    public static void Apply(IEvent<LocatieWerdToegevoegd> locatieWerdToegevoegd, BeheerVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Append(BeheerVerenigingDetailMapper.MapLocatie(locatieWerdToegevoegd.Data.Locatie,
                                                                                    locatieWerdToegevoegd.Data.Bron,
                                                                                    document.VCode))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(IEvent<LocatieWerdGewijzigd> locatieWerdGewijzigd, BeheerVerenigingDetailDocument document)
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

    public static void Apply(IEvent<LocatieWerdVerwijderd> locatieWerdVerwijderd, BeheerVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieWerdVerwijderd.Data.Locatie.LocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> maatschappelijkeZetelWerdOvergenomenUitKbo,
        BeheerVerenigingDetailDocument document)
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
        BeheerVerenigingDetailDocument document)
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
        BeheerVerenigingDetailDocument document)
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
        BeheerVerenigingDetailDocument document)
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

    public static void Apply(IEvent<VerenigingWerdGestopt> verenigingWerdGestopt, BeheerVerenigingDetailDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
        document.Einddatum = verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly);
    }

    public static void Apply(IEvent<VerenigingWerdGestoptInKBO> verenigingWerdGestoptInKbo, BeheerVerenigingDetailDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
        document.Einddatum = verenigingWerdGestoptInKbo.Data.Einddatum.ToString(WellknownFormats.DateOnly);
    }

    public static void Apply(IEvent<EinddatumWerdGewijzigd> einddatumWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Einddatum = einddatumWerdGewijzigd.Data.Einddatum.ToString(WellknownFormats.DateOnly);
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> vertegenwoordigerWerdOvergenomenUitKbo,
        BeheerVerenigingDetailDocument document)
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

    public static void UpdateMetadata(IEvent e, BeheerVerenigingDetailDocument document)
    {
        document.DatumLaatsteAanpassing = e.GetHeaderInstant(MetadataHeaderNames.Tijdstip).FormatAsBelgianDate();
        document.Metadata = new Metadata(e.Sequence, e.Version);
    }

    public static void Apply(IEvent<NaamWerdGewijzigdInKbo> naamWerdGewijzigdInKbo, BeheerVerenigingDetailDocument document)
    {
        document.Naam = naamWerdGewijzigdInKbo.Data.Naam;
    }

    public static void Apply(IEvent<KorteNaamWerdGewijzigdInKbo> korteNaamWerdGewijzigdInKbo, BeheerVerenigingDetailDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigdInKbo.Data.KorteNaam;
    }

    public static void Apply(
        IEvent<ContactgegevenWerdGewijzigdInKbo> contactgegevenWerdGewijzigdUitKbo,
        BeheerVerenigingDetailDocument document)
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
        BeheerVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(
                                                c => c.ContactgegevenId != contactgegevenWerdVerwijderdUitKbo.Data.ContactgegevenId)
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(
        IEvent<ContactgegevenWerdInBeheerGenomenDoorKbo> contactgegevenWerdInBeheerGenomenDoorKbo,
        BeheerVerenigingDetailDocument document)
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
        BeheerVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.LocatieId,
                                         update: l => l with
                                         {
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres, document.VCode, l.LocatieId),
                                             Adresvoorstelling =
                                             maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres.ToAdresString(),
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
        BeheerVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != maatschappelijkeZetelWerdVerwijderdUitKbo.Data.Locatie.LocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<AdresWerdOvergenomenUitAdressenregister> adresWerdOvergenomenUitAdressenregister,
        BeheerVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresWerdOvergenomenUitAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 adresWerdOvergenomenUitAdressenregister.Data.Adres, document.VCode,
                                                 l.LocatieId),
                                             Adresvoorstelling =
                                             adresWerdOvergenomenUitAdressenregister.Data.Adres.ToAdresString(),
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
        BeheerVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresWerdGewijzigdInAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 adresWerdGewijzigdInAdressenregister.Data.Adres, document.VCode,
                                                 l.LocatieId),
                                             Adresvoorstelling =
                                             adresWerdGewijzigdInAdressenregister.Data.Adres.ToAdresString(),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 adresWerdGewijzigdInAdressenregister.Data.AdresId),
                                             VerwijstNaar = BeheerVerenigingDetailMapper.MapAdresVerwijzing(
                                                 adresWerdGewijzigdInAdressenregister.Data.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(IEvent<RechtsvormWerdGewijzigdInKBO> rechtsvormWerdGewijzigdInKbo, BeheerVerenigingDetailDocument document)
    {
        document.Verenigingstype = new Verenigingstype
        {
            Code = AssociationRegistry.Vereniging.Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Code,
            Naam = AssociationRegistry.Vereniging.Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Naam,
        };

        document.Rechtsvorm = rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm;
    }

    public static void Apply(
        IEvent<AdresWerdNietGevondenInAdressenregister> adresWerdNietGevondenInAdressenregister,
        BeheerVerenigingDetailDocument document)
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
        BeheerVerenigingDetailDocument document)
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
        BeheerVerenigingDetailDocument document)
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
        BeheerVerenigingDetailDocument document)
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
        BeheerVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieDuplicaatWerdVerwijderdNaAdresMatch.Data.VerwijderdeLocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(IEvent<LidmaatschapWerdToegevoegd> lidmaatschapWerdToegevoegd, BeheerVerenigingDetailDocument document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                           .Append(BeheerVerenigingDetailMapper.MapLidmaatschap(
                                                       lidmaatschapWerdToegevoegd.Data.Lidmaatschap,
                                                       document.VCode))
                                           .OrderBy(l => l.LidmaatschapId)
                                           .ToArray();
    }

    public static void Apply(IEvent<LidmaatschapWerdGewijzigd> lidmaatschapWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                           .UpdateSingle(
                                                identityFunc: l
                                                    => l.LidmaatschapId == lidmaatschapWerdGewijzigd.Data.Lidmaatschap.LidmaatschapId,
                                                update: l => BeheerVerenigingDetailMapper.MapLidmaatschap(
                                                    lidmaatschapWerdGewijzigd.Data.Lidmaatschap,
                                                    document.VCode))
                                           .OrderBy(l => l.LidmaatschapId)
                                           .ToArray();
    }

    public static void Apply(IEvent<LidmaatschapWerdVerwijderd> lidmaatschapWerdToegevoegd, BeheerVerenigingDetailDocument document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                           .Where(l => l.LidmaatschapId != lidmaatschapWerdToegevoegd.Data.Lidmaatschap.LidmaatschapId)
                                           .OrderBy(l => l.LidmaatschapId)
                                           .ToArray();
    }


    public static void Apply(IEvent<VerenigingWerdGemarkeerdAlsDubbelVan> verenigingWerdGemarkeerdAlsDubbel, BeheerVerenigingDetailDocument document)
    {
        document.Status = VerenigingStatus.Dubbel;
        document.IsDubbelVan = verenigingWerdGemarkeerdAlsDubbel.Data.VCodeAuthentiekeVereniging;
    }

    public static void Apply(IEvent<VerenigingAanvaarddeDubbeleVereniging> verenigingAanvaardeDubbeleVereniging, BeheerVerenigingDetailDocument document)
    {
        document.CorresponderendeVCodes =
            document.CorresponderendeVCodes
                    .Append(verenigingAanvaardeDubbeleVereniging.Data.VCodeDubbeleVereniging)
                    .ToArray();
    }

    public static void Apply(IEvent<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> weigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt, BeheerVerenigingDetailDocument document)
    {
        document.Status = weigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt.Data.VorigeStatus;
        document.IsDubbelVan = string.Empty;
    }

    public static void Apply(IEvent<MarkeringDubbeleVerengingWerdGecorrigeerd> markeringDubbeleVerengingWerdGecorrigeerd, BeheerVerenigingDetailDocument document)
    {
        document.Status = markeringDubbeleVerengingWerdGecorrigeerd.Data.VorigeStatus;
        document.IsDubbelVan = string.Empty;
    }

    public static void Apply(IEvent<VerenigingAanvaarddeCorrectieDubbeleVereniging> verenigingAanvaarddeCorrectieDubbeleVereniging, BeheerVerenigingDetailDocument document)
    {
        document.CorresponderendeVCodes =
            document.CorresponderendeVCodes
                    .Where(x => x != verenigingAanvaarddeCorrectieDubbeleVereniging.Data.VCodeDubbeleVereniging)
                    .ToArray();
    }

    public static void Apply(IEvent<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> @event, BeheerVerenigingDetailDocument document)
    {
        document.Verenigingstype = BeheerVerenigingDetailMapper.MapVerenigingstype(AssociationRegistry.Vereniging.Verenigingstype.VZER);
        document.Verenigingssubtype = BeheerVerenigingDetailMapper.MapSubtype(Verenigingssubtype.NietBepaald);
    }

    public static void Apply(IEvent<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> @event, BeheerVerenigingDetailDocument document)
    {
        document.Verenigingssubtype = BeheerVerenigingDetailMapper.MapSubtype(Verenigingssubtype.FeitelijkeVereniging);
        document.SubverenigingVan = null;
    }

    public static void Apply(IEvent<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> @event, BeheerVerenigingDetailDocument document)
    {
        document.Verenigingssubtype = BeheerVerenigingDetailMapper.MapSubtype(Verenigingssubtype.NietBepaald);
        document.SubverenigingVan = null;
    }

    public static void Apply(IEvent<VerenigingssubtypeWerdVerfijndNaarSubvereniging> @event, BeheerVerenigingDetailDocument document)
    {
        document.Verenigingssubtype = BeheerVerenigingDetailMapper.MapSubtype(Verenigingssubtype.Subvereniging);

        document.SubverenigingVan = new SubverenigingVan()
        {
            AndereVereniging = @event.Data.SubverenigingVan.AndereVereniging,
            AndereVerenigingNaam = @event.Data.SubverenigingVan.AndereVerenigingNaam,
            Identificatie = @event.Data.SubverenigingVan.Identificatie,
            Beschrijving = @event.Data.SubverenigingVan.Beschrijving,
        };
    }

    public static void Apply(IEvent<SubverenigingRelatieWerdGewijzigd> @event, BeheerVerenigingDetailDocument document)
    {
        document.SubverenigingVan = document.SubverenigingVan! with
        {
            AndereVereniging = @event.Data.AndereVereniging,
            AndereVerenigingNaam = @event.Data.AndereVerenigingNaam,
        };
    }

    public static void Apply(IEvent<SubverenigingDetailsWerdenGewijzigd> @event, BeheerVerenigingDetailDocument document)
    {
        document.SubverenigingVan = document.SubverenigingVan! with
        {
            Identificatie = @event.Data.Identificatie,
            Beschrijving = @event.Data.Beschrijving,
        };
    }
}
