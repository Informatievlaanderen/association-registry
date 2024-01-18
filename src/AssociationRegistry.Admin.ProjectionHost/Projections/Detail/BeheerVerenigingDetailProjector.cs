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
using Doelgroep = Schema.Detail.Doelgroep;
using IEvent = Marten.Events.IEvent;

public class BeheerVerenigingDetailProjector
{
    public static BeheerVerenigingDetailDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            JsonLdMetadata =
                BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Vereniging, feitelijkeVerenigingWerdGeregistreerd.Data.VCode),
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Verenigingstype = BeheerVerenigingDetailMapper.MapVerenigingsType(Verenigingstype.FeitelijkeVereniging),
            Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            Doelgroep = BeheerVerenigingDetailMapper.MapDoelgroep(feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep),
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
                                                                                     .Select(h => BeheerVerenigingDetailMapper
                                                                                             .MapHoofdactiviteitVerenigingsloket(
                                                                                                  h,
                                                                                                  feitelijkeVerenigingWerdGeregistreerd.Data
                                                                                                     .VCode))
                                                                                     .ToArray(),
            Bron = feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
            Metadata = new Metadata(feitelijkeVerenigingWerdGeregistreerd.Sequence, feitelijkeVerenigingWerdGeregistreerd.Version),
        };

    public static BeheerVerenigingDetailDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
        => new()
        {
            JsonLdMetadata =
                BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Vereniging,
                                                                  verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
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
                Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
            },
            Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
            DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                                        .ToBelgianDate(),
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            Contactgegevens = Array.Empty<Schema.Detail.Contactgegeven>(),
            Locaties = Array.Empty<Schema.Detail.Locatie>(),
            Vertegenwoordigers = Array.Empty<Schema.Detail.Vertegenwoordiger>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<Schema.Detail.HoofdactiviteitVerenigingsloket>(),
            Sleutels = new[]
            {
                BeheerVerenigingDetailMapper.MapKboSleutel(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                                                           verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
            },
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
        document.Startdatum = startdatumWerdGewijzigd.Data.Startdatum?.ToString(WellknownFormats.DateOnly);
    }

    public static void Apply(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Doelgroep = new Doelgroep
        {
            Minimumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd,
        };
    }

    public static void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, BeheerVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.Append(
                                                new Schema.Detail.Contactgegeven
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
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofactiviteitenVerenigingloketWerdenGewijzigd,
        BeheerVerenigingDetailDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = hoofactiviteitenVerenigingloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket
           .Select(
                h => new Schema.Detail.HoofdactiviteitVerenigingsloket
                {
                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Hoofdactiviteit, document.VCode, h.Code),
                    Code = h.Code,
                    Naam = h.Naam,
                }).ToArray();
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd,
        BeheerVerenigingDetailDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers.Append(
                                                   new Schema.Detail.Vertegenwoordiger
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

                                                       VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens()
                                                       {
                                                           JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                                               JsonLdType.VertegenwoordigerContactgegeven, document.VCode,
                                                               vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId.ToString()),
                                                           IsPrimair = vertegenwoordigerWerdToegevoegd.Data.IsPrimair,
                                                           Email = vertegenwoordigerWerdToegevoegd.Data.Email,
                                                           Telefoon = vertegenwoordigerWerdToegevoegd.Data.Telefoon,
                                                           Mobiel = vertegenwoordigerWerdToegevoegd.Data.Mobiel,
                                                           SocialMedia = vertegenwoordigerWerdToegevoegd.Data.SocialMedia,
                                                       }
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
                                                       }
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
                                                                                    locatieWerdToegevoegd.Data.Bron, document.VCode))
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
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(locatieWerdGewijzigd.Data.Locatie.AdresId),
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
                                                new Schema.Detail.Contactgegeven
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

    public static void Apply(IEvent<EinddatumWerdGewijzigd> einddatumWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Einddatum = einddatumWerdGewijzigd.Data.Einddatum.ToString(WellknownFormats.DateOnly);
    }

    public static void Apply(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> vertegenwoordigerWerdOvergenomenUitKbo,
        BeheerVerenigingDetailDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers.Append(
                                                   new Schema.Detail.Vertegenwoordiger
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
                                                       VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens()
                                                       {
                                                           JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                                               JsonLdType.VertegenwoordigerContactgegeven, document.VCode,
                                                               vertegenwoordigerWerdOvergenomenUitKbo.Data.VertegenwoordigerId.ToString()),
                                                           IsPrimair = false,
                                                           Email = string.Empty,
                                                           Telefoon = string.Empty,
                                                           Mobiel = string.Empty,
                                                           SocialMedia = string.Empty,
                                                       }

                                                   })
                                              .OrderBy(v => v.VertegenwoordigerId)
                                              .ToArray();
    }

    public static void UpdateMetadata(IEvent e, BeheerVerenigingDetailDocument document)
    {
        document.DatumLaatsteAanpassing = e.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(e.Sequence, e.Version);
    }
}
