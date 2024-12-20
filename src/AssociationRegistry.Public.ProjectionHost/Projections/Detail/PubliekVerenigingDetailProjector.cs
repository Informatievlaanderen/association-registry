namespace AssociationRegistry.Public.ProjectionHost.Projections.Detail;

using Events;
using Formats;
using Framework;
using JsonLdContext;
using Marten.Events;
using Schema.Constants;
using Schema.Detail;
using Vereniging;
using AdresFormatter = Formats.AdresFormatter;
using Doelgroep = Schema.Detail.Doelgroep;
using IEvent = Marten.Events.IEvent;
using VerenigingStatus = Schema.Constants.VerenigingStatus;

public static class PubliekVerenigingDetailProjector
{
    public static PubliekVerenigingDetailDocument Create(
        IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type,
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
            Doelgroep =
                MapDoelgroep(feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep, feitelijkeVerenigingWerdGeregistreerd.Data.VCode),
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                          .FormatAsBelgianDate(),
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

            Werkingsgebieden = [],

            Sleutels =
            [
                new()
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Sleutel.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                                                              Sleutelbron.VR.Waarde),
                        JsonLdType.Sleutel.Type),
                    Bron = Sleutelbron.VR.Waarde,
                    GestructureerdeIdentificator = new PubliekVerenigingDetailDocument.GestructureerdeIdentificator
                    {
                        JsonLdMetadata = new JsonLdMetadata(
                            JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                                Sleutelbron.VR.Waarde),
                            JsonLdType.GestructureerdeSleutel.Type),
                        Nummer = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                    },
                    Waarde = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                    CodeerSysteem = CodeerSysteem.VR,
                },
            ],
        };

    public static PubliekVerenigingDetailDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
        => new()
        {
            JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
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
                JsonLdMetadata = new JsonLdMetadata(
                    JsonLdType.Doelgroep.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
                    JsonLdType.Doelgroep.Type),
                Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
            },
            Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
            DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                                        .FormatAsBelgianDate(),
            IsUitgeschrevenUitPubliekeDatastroom = false,
            Status = VerenigingStatus.Actief,
            Contactgegevens = [],
            Locaties = [],
            HoofdactiviteitenVerenigingsloket = [],
            Sleutels =
            [
                new()
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Sleutel.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                                                              Sleutelbron.VR.Waarde),
                        JsonLdType.Sleutel.Type),
                    Bron = Sleutelbron.VR.Waarde,
                    GestructureerdeIdentificator = new PubliekVerenigingDetailDocument.GestructureerdeIdentificator
                    {
                        JsonLdMetadata = new JsonLdMetadata(
                            JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                                Sleutelbron.VR.Waarde),
                            JsonLdType.GestructureerdeSleutel.Type),
                        Nummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                    },
                    Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                    CodeerSysteem = CodeerSysteem.VR,
                },
                new()
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Sleutel.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                                                              Sleutelbron.KBO.Waarde),
                        JsonLdType.Sleutel.Type),
                    Bron = Sleutelbron.KBO.Waarde,
                    GestructureerdeIdentificator = new PubliekVerenigingDetailDocument.GestructureerdeIdentificator
                    {
                        JsonLdMetadata = new JsonLdMetadata(
                            JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                                Sleutelbron.KBO.Waarde),
                            JsonLdType.GestructureerdeSleutel.Type),
                        Nummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                    },
                    Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                    CodeerSysteem = CodeerSysteem.KBO,
                },
            ],
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

    private static PubliekVerenigingDetailDocument.Werkingsgebied MapWerkingsgebied(
        Registratiedata.Werkingsgebied werkingsgebied)
        => new()
        {
            JsonLdMetadata = new JsonLdMetadata(
                JsonLdType.Werkingsgebied.CreateWithIdValues(werkingsgebied.Code),
                JsonLdType.Werkingsgebied.Type),
            Code = werkingsgebied.Code,
            Naam = werkingsgebied.Naam,
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

    public static void Apply(IEvent<StartdatumWerdGewijzigdInKbo> startdatumWerdGewijzigdInKbo, PubliekVerenigingDetailDocument document)
    {
        document.Startdatum = startdatumWerdGewijzigdInKbo.Data.Startdatum;
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
            JsonLdMetadata = new JsonLdMetadata(
                JsonLdType.Doelgroep.CreateWithIdValues(doelgroepWerdGewijzigd.StreamKey!),
                JsonLdType.Doelgroep.Type),
            Minimumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd,
        };
    }

    public static void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = Enumerable.Append(document.Contactgegevens, new PubliekVerenigingDetailDocument.Contactgegeven
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
        document.Locaties = Enumerable.Append(document.Locaties, MapLocatie(document.VCode, locatieWerdToegevoegd.Data.Locatie))
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
        IEvent<WerkingsgebiedenWerdenNietBepaald> werkingsgebiedenWerdenNietBepaald,
        PubliekVerenigingDetailDocument document)
    {
        document.Werkingsgebieden = [];
    }

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenBepaald> werkingsgebiedenWerdenBepaald,
        PubliekVerenigingDetailDocument document)
    {
        document.Werkingsgebieden = werkingsgebiedenWerdenBepaald.Data.Werkingsgebieden
                                                                 .Select(
                                                                      h => new PubliekVerenigingDetailDocument.Werkingsgebied
                                                                      {
                                                                          JsonLdMetadata = new JsonLdMetadata(
                                                                              JsonLdType.Werkingsgebied.CreateWithIdValues(h.Code),
                                                                              JsonLdType.Werkingsgebied.Type),
                                                                          Code = h.Code,
                                                                          Naam = h.Naam,
                                                                      }).ToArray();
    }

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenGewijzigd> werkingsgebiedenWerdenGewijzigd,
        PubliekVerenigingDetailDocument document)
    {
        document.Werkingsgebieden = werkingsgebiedenWerdenGewijzigd.Data.Werkingsgebieden
                                                                   .Select(
                                                                        h => new PubliekVerenigingDetailDocument.Werkingsgebied
                                                                        {
                                                                            JsonLdMetadata = new JsonLdMetadata(
                                                                                JsonLdType.Werkingsgebied.CreateWithIdValues(h.Code),
                                                                                JsonLdType.Werkingsgebied.Type),
                                                                            Code = h.Code,
                                                                            Naam = h.Naam,
                                                                        }).ToArray();
    }

    public static void Apply(
        IEvent<WerkingsgebiedenWerdenNietVanToepassing> werkingsgebiedenWerdenNietVanToepassing,
        PubliekVerenigingDetailDocument document)
    {
        document.Werkingsgebieden =
        [
            new PubliekVerenigingDetailDocument.Werkingsgebied
            {
                JsonLdMetadata = new JsonLdMetadata(
                    JsonLdType.Werkingsgebied.CreateWithIdValues(Werkingsgebied.NietVanToepassing.Code),
                    JsonLdType.Werkingsgebied.Type),
                Code = Werkingsgebied.NietVanToepassing.Code,
                Naam = Werkingsgebied.NietVanToepassing.Naam,
            }
        ];
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

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> maatschappelijkeZetelWerdOvergenomenUitKbo,
        PubliekVerenigingDetailDocument document)
    {
        document.Locaties = Enumerable.Append(document.Locaties,
                                              MapLocatie(document.VCode, maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie))
                                      .OrderBy(l => l.LocatieId)
                                      .ToArray();
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo> maatschappelijkeZetelWerdGewijzigdInKbo,
        PubliekVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.LocatieId)
                                    .Append(MapLocatie(document.VCode, maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo> maatschappelijkeZetelWerdVerwijderdUitKbo,
        PubliekVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != maatschappelijkeZetelWerdVerwijderdUitKbo.Data.Locatie.LocatieId)
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
        document.Contactgegevens = Enumerable.Append(document.Contactgegevens, new PubliekVerenigingDetailDocument.Contactgegeven
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

    public static void Apply(IEvent<VerenigingWerdGestoptInKBO> verenigingWerdGestoptInKbo, PubliekVerenigingDetailDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
    }

    public static void UpdateMetadata(IEvent @event, PubliekVerenigingDetailDocument document)
    {
        document.DatumLaatsteAanpassing = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).FormatAsBelgianDate();
    }

    public static void Apply(IEvent<NaamWerdGewijzigdInKbo> naamWerdGewijzigdInKbo, PubliekVerenigingDetailDocument document)
    {
        document.Naam = naamWerdGewijzigdInKbo.Data.Naam;
    }

    public static void Apply(IEvent<KorteNaamWerdGewijzigdInKbo> korteNaamWerdGewijzigdInKbo, PubliekVerenigingDetailDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigdInKbo.Data.KorteNaam;
    }

    public static void Apply(
        IEvent<ContactgegevenWerdGewijzigdInKbo> contactgegevenWerdGewijzigdUitKbo,
        PubliekVerenigingDetailDocument document)
    {
        var contactgegeven =
            document.Contactgegevens.Single(c => c.ContactgegevenId == contactgegevenWerdGewijzigdUitKbo.Data.ContactgegevenId);

        contactgegeven.Waarde = contactgegevenWerdGewijzigdUitKbo.Data.Waarde;

        document.Contactgegevens = document.Contactgegevens
                                           .Where(c => c.ContactgegevenId != contactgegevenWerdGewijzigdUitKbo.Data.ContactgegevenId)
                                           .Append(contactgegeven)
                                           .OrderBy(l => l.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(
        IEvent<ContactgegevenWerdVerwijderdUitKBO> contactgegevenWerdVerwijderdUitKbo,
        PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(c => c.ContactgegevenId != contactgegevenWerdVerwijderdUitKbo.Data.ContactgegevenId)
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
    }

    public static void Apply(IEvent<RechtsvormWerdGewijzigdInKBO> rechtsvormWerdGewijzigdInKbo, PubliekVerenigingDetailDocument document)
    {
        document.Rechtsvorm = rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm;

        document.Verenigingstype = new PubliekVerenigingDetailDocument.VerenigingsType
        {
            Code = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Code,
            Naam = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Naam,
        };
    }

    public static void Apply(
        IEvent<AdresWerdOvergenomenUitAdressenregister> adresWerdOvergenomenUitAdressenregister,
        PubliekVerenigingDetailDocument document)
    {
        var @event = adresWerdOvergenomenUitAdressenregister.Data;
        var locatie = document.Locaties.Single(s => s.LocatieId == @event.LocatieId);

        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != adresWerdOvergenomenUitAdressenregister.Data.LocatieId)
                                    .Append(locatie with
                                     {
                                         JsonLdMetadata = new JsonLdMetadata(
                                             JsonLdType.Locatie.CreateWithIdValues(adresWerdOvergenomenUitAdressenregister.Data.VCode,
                                                                                   adresWerdOvergenomenUitAdressenregister.Data.LocatieId
                                                                                      .ToString()),
                                             JsonLdType.Locatie.Type),
                                         Adres = Map(adresWerdOvergenomenUitAdressenregister.Data.VCode,
                                                     adresWerdOvergenomenUitAdressenregister.Data.LocatieId,
                                                     adresWerdOvergenomenUitAdressenregister.Data.Adres),
                                         Adresvoorstelling =
                                         AdresFormatter.ToAdresString(adresWerdOvergenomenUitAdressenregister.Data.Adres),
                                         AdresId = Map(adresWerdOvergenomenUitAdressenregister.Data.AdresId),
                                         VerwijstNaar =
                                         MapVerwijstNaar(adresWerdOvergenomenUitAdressenregister.Data.AdresId),
                                     })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<AdresWerdGewijzigdInAdressenregister> adresWerdGewijzigdInAdressenregister,
        PubliekVerenigingDetailDocument document)
    {
        var @event = adresWerdGewijzigdInAdressenregister.Data;
        var locatie = document.Locaties.Single(s => s.LocatieId == @event.LocatieId);

        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != adresWerdGewijzigdInAdressenregister.Data.LocatieId)
                                    .Append(locatie with
                                     {
                                         JsonLdMetadata = new JsonLdMetadata(
                                             JsonLdType.Locatie.CreateWithIdValues(adresWerdGewijzigdInAdressenregister.Data.VCode,
                                                                                   adresWerdGewijzigdInAdressenregister.Data.LocatieId
                                                                                      .ToString()),
                                             JsonLdType.Locatie.Type),
                                         Adres = Map(adresWerdGewijzigdInAdressenregister.Data.VCode,
                                                     adresWerdGewijzigdInAdressenregister.Data.LocatieId,
                                                     adresWerdGewijzigdInAdressenregister.Data.Adres),
                                         Adresvoorstelling = AdresFormatter.ToAdresString(adresWerdGewijzigdInAdressenregister.Data.Adres),
                                         AdresId = Map(adresWerdGewijzigdInAdressenregister.Data.AdresId),
                                         VerwijstNaar =
                                         MapVerwijstNaar(adresWerdGewijzigdInAdressenregister.Data.AdresId),
                                     })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<AdresWerdNietGevondenInAdressenregister> adresWerdNietGevondenInAdressenregister,
        PubliekVerenigingDetailDocument document)
    {
        var @event = adresWerdNietGevondenInAdressenregister.Data;
        var locatie = document.Locaties.Single(s => s.LocatieId == @event.LocatieId);

        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != adresWerdNietGevondenInAdressenregister.Data.LocatieId)
                                    .Append(locatie with
                                     {
                                         AdresId = null,
                                         VerwijstNaar = null,
                                     })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<AdresNietUniekInAdressenregister> adresNietUniekInAdressenregister,
        PubliekVerenigingDetailDocument document)
    {
        var @event = adresNietUniekInAdressenregister.Data;
        var locatie = document.Locaties.Single(s => s.LocatieId == @event.LocatieId);

        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != adresNietUniekInAdressenregister.Data.LocatieId)
                                    .Append(locatie with
                                     {
                                         AdresId = null,
                                         VerwijstNaar = null,
                                     })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(
        IEvent<AdresWerdOntkoppeldVanAdressenregister> adresWerdOntkoppeldVanAdressenregister,
        PubliekVerenigingDetailDocument document)
    {
        var @event = adresWerdOntkoppeldVanAdressenregister.Data;
        var locatie = document.Locaties.Single(s => s.LocatieId == @event.LocatieId);

        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != adresWerdOntkoppeldVanAdressenregister.Data.LocatieId)
                                    .Append(locatie with
                                     {
                                         AdresId = null,
                                         VerwijstNaar = null,
                                     })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
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
            Locatietype = loc.Locatietype,
            Adres = Map(vCode, loc.LocatieId, loc.Adres),
            Adresvoorstelling = AdresFormatter.ToAdresString(loc.Adres),
            AdresId = Map(loc.AdresId),
            VerwijstNaar = MapVerwijstNaar(loc.AdresId),
        };

    private static PubliekVerenigingDetailDocument.Locatie.AdresVerwijzing? MapVerwijstNaar(Registratiedata.AdresId? adresid)
    {
        if (adresid is null) return null;

        return new PubliekVerenigingDetailDocument.Locatie.AdresVerwijzing
        {
            JsonLdMetadata = new JsonLdMetadata(
                JsonLdType.AdresVerwijzing.CreateWithIdValues(adresid.Bronwaarde.Split('/').Last()),
                JsonLdType.AdresVerwijzing.Type),
        };
    }

    private static PubliekVerenigingDetailDocument.Adres? Map(
        string vCode,
        int locatieId,
        Registratiedata.AdresUitAdressenregister? adresUitAdressenregister)
        => adresUitAdressenregister is null
            ? null
            : new PubliekVerenigingDetailDocument.Adres
            {
                JsonLdMetadata = new JsonLdMetadata(
                    JsonLdType.Adres.CreateWithIdValues(vCode, locatieId.ToString()),
                    JsonLdType.Adres.Type),
                Straatnaam = adresUitAdressenregister.Straatnaam,
                Huisnummer = adresUitAdressenregister.Huisnummer,
                Busnummer = adresUitAdressenregister.Busnummer,
                Postcode = adresUitAdressenregister.Postcode,
                Gemeente = adresUitAdressenregister.Gemeente,
                Land = Adres.België,
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

    private static PubliekVerenigingDetailDocument.AdresId? Map(string broncode, string bronwaarde)
        => string.IsNullOrEmpty(bronwaarde)
            ? null
            : new PubliekVerenigingDetailDocument.AdresId
            {
                Bronwaarde = bronwaarde,
                Broncode = broncode,
            };

    private static Doelgroep MapDoelgroep(Registratiedata.Doelgroep doelgroep, string vCode)
        => new()
        {
            JsonLdMetadata = new JsonLdMetadata(
                JsonLdType.Doelgroep.CreateWithIdValues(vCode),
                JsonLdType.Doelgroep.Type),
            Minimumleeftijd = doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroep.Maximumleeftijd,
        };

    public static void Apply(
        IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch> locatieWerdVerwijderd,
        PubliekVerenigingDetailDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieWerdVerwijderd.Data.VerwijderdeLocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
    }

    public static void Apply(IEvent<LidmaatschapWerdToegevoegd> lidmaatschapWerdToegevoegd, PubliekVerenigingDetailDocument document)
    {
        document.Lidmaatschappen = Enumerable.Append(document.Lidmaatschappen,
                                                     MapLidmaatschap(lidmaatschapWerdToegevoegd.Data.Lidmaatschap, document.VCode))
                                             .OrderBy(l => l.LidmaatschapId)
                                             .ToArray();
    }

    private static PubliekVerenigingDetailDocument.Lidmaatschap MapLidmaatschap(Registratiedata.Lidmaatschap lidmaatschap, string vCode)
        => new(new JsonLdMetadata(
                   JsonLdType.Lidmaatschap.CreateWithIdValues(vCode, lidmaatschap.LidmaatschapId.ToString()),
                   JsonLdType.Lidmaatschap.Type),
               lidmaatschap.LidmaatschapId,
               lidmaatschap.AndereVereniging,
               lidmaatschap.DatumVan,
               lidmaatschap.DatumTot,
               lidmaatschap.Identificatie,
               lidmaatschap.Beschrijving);

    public static void Apply(IEvent<LidmaatschapWerdGewijzigd> lidmaatschapWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                           .Where(l => l.LidmaatschapId != lidmaatschapWerdGewijzigd.Data.Lidmaatschap.LidmaatschapId)
                                           .Append(MapLidmaatschap(lidmaatschapWerdGewijzigd.Data.Lidmaatschap, document.VCode))
                                           .OrderBy(l => l.LidmaatschapId)
                                           .ToArray();
    }

    public static void Apply(IEvent<LidmaatschapWerdVerwijderd> lidmaatschapWerdVerwijderd, PubliekVerenigingDetailDocument document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                           .Where(l => l.LidmaatschapId != lidmaatschapWerdVerwijderd.Data.Lidmaatschap.LidmaatschapId)
                                           .OrderBy(l => l.LidmaatschapId)
                                           .ToArray();
    }

    public static void Apply(IEvent<VerenigingWerdGemarkeerdAlsDubbelVan> verenigingWerdGemarkeerdAlsDubbel, PubliekVerenigingDetailDocument document)
    {
        document.Status = VerenigingStatus.Dubbel;
    }
}
