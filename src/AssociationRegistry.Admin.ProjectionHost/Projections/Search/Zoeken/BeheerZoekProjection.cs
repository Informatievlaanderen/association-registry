namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search.Zoeken;

using Events;
using Formats;
using JsonLdContext;
using Schema;
using Schema.Constants;
using Schema.Search;
using Vereniging;

using Doelgroep = Schema.Search.VerenigingZoekDocument.Types.Doelgroep;
using VerenigingStatus = Schema.Constants.VerenigingStatus;
using WellknownFormats = Constants.WellknownFormats;

public class BeheerZoekProjectionHandler
{
    private readonly IElasticRepository _elasticRepository;

    public BeheerZoekProjectionHandler(
        IElasticRepository elasticRepository
    )
    {
        _elasticRepository = elasticRepository;
    }

    public async Task Handle(EventEnvelope<FeitelijkeVerenigingWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingZoekDocument
            {
                JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type,
                VCode = message.Data.VCode,
                Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
                {
                    Code = Verenigingstype.FeitelijkeVereniging.Code,
                    Naam = Verenigingstype.FeitelijkeVereniging.Naam,
                },
                Verenigingssubtype = null,
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Status = VerenigingStatus.Actief,
                Locaties = message.Data.Locaties.Select(locatie => Map(locatie, message.VCode)).ToArray(),
                Startdatum = message.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                Einddatum = null,
                Doelgroep = Map(message.Data.Doelgroep, message.VCode),
                IsUitgeschrevenUitPubliekeDatastroom = message.Data.IsUitgeschrevenUitPubliekeDatastroom,
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                                                           .Select(
                                                                hoofdactiviteitVerenigingsloket =>
                                                                    new VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket
                                                                    {
                                                                        JsonLdMetadata =
                                                                            CreateJsonLdMetadata(
                                                                                JsonLdType.Hoofdactiviteit,
                                                                                hoofdactiviteitVerenigingsloket.Code),
                                                                        Code = hoofdactiviteitVerenigingsloket.Code,
                                                                        Naam = hoofdactiviteitVerenigingsloket.Naam,
                                                                    })
                                                           .ToArray(),

                Werkingsgebieden = [],
                Lidmaatschappen = [],
                CorresponderendeVCodes = [],
                Sleutels =
                [
                    new VerenigingZoekDocument.Types.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                        Bron = Sleutelbron.VR,
                        Waarde = message.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                            Nummer = message.Data.VCode,
                        },
                    },
                ],
            }
        );

    public async Task Handle(EventEnvelope<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingZoekDocument
            {
                JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type,
                VCode = message.Data.VCode,
                Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
                {
                    Code = Verenigingstype.VZER.Code,
                    Naam = Verenigingstype.VZER.Naam,
                },
                Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype
                {
                    Code = Verenigingssubtype.NietBepaald.Code,
                    Naam = Verenigingssubtype.NietBepaald.Naam,
                },
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Status = VerenigingStatus.Actief,
                Locaties = message.Data.Locaties.Select(locatie => Map(locatie, message.VCode)).ToArray(),
                Startdatum = message.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                Einddatum = null,
                Doelgroep = Map(message.Data.Doelgroep, message.VCode),
                IsUitgeschrevenUitPubliekeDatastroom = message.Data.IsUitgeschrevenUitPubliekeDatastroom,
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                                                           .Select(
                                                                hoofdactiviteitVerenigingsloket =>
                                                                    new VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket
                                                                    {
                                                                        JsonLdMetadata =
                                                                            CreateJsonLdMetadata(
                                                                                JsonLdType.Hoofdactiviteit,
                                                                                hoofdactiviteitVerenigingsloket.Code),
                                                                        Code = hoofdactiviteitVerenigingsloket.Code,
                                                                        Naam = hoofdactiviteitVerenigingsloket.Naam,
                                                                    })
                                                           .ToArray(),

                Werkingsgebieden = [],
                Lidmaatschappen = [],
                CorresponderendeVCodes = [],
                Sleutels =
                [
                    new VerenigingZoekDocument.Types.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                        Bron = Sleutelbron.VR,
                        Waarde = message.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                            Nummer = message.Data.VCode,
                        },
                    },
                ],
            }
        );

    public async Task Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingZoekDocument
            {
                JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
                VCode = message.Data.VCode,
                Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
                {
                    Code = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
                    Naam = Verenigingstype.Parse(message.Data.Rechtsvorm).Naam,
                },
                Verenigingssubtype = null,
                Naam = message.Data.Naam,
                Roepnaam = string.Empty,
                KorteNaam = message.Data.KorteNaam,
                Status = VerenigingStatus.Actief,
                Locaties = [],
                Startdatum = message.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                Einddatum = null,
                Doelgroep = new Doelgroep
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, message.VCode),
                    Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                    Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
                },
                IsUitgeschrevenUitPubliekeDatastroom = false,
                HoofdactiviteitenVerenigingsloket = [],
                Werkingsgebieden = [],
                Lidmaatschappen = [],
                CorresponderendeVCodes = [],
                Sleutels =
                [
                    new VerenigingZoekDocument.Types.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                        Bron = Sleutelbron.VR,
                        Waarde = message.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                            Nummer = message.Data.VCode,
                        },
                    },
                    new VerenigingZoekDocument.Types.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.KBO.Waarde),
                        Bron = Sleutelbron.KBO,
                        Waarde = message.Data.KboNummer,
                        CodeerSysteem = CodeerSysteem.KBO,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.Types.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.KBO.Waarde),
                            Nummer = message.Data.KboNummer,
                        },
                    },
                ],
            }
        );

    public async Task Handle(EventEnvelope<NaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new VerenigingZoekDocument
            {
                Naam = message.Data.Naam,
            }
        );

    public async Task Handle(EventEnvelope<RoepnaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Roepnaam = message.Data.Roepnaam,
            }
        );

    public async Task Handle(EventEnvelope<KorteNaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new VerenigingZoekDocument
            {
                KorteNaam = message.Data.KorteNaam,
            }
        );

    public async Task Handle(EventEnvelope<StartdatumWerdGewijzigd> message)
        => await _elasticRepository.UpdateStartdatum<VerenigingZoekDocument>(
            message.VCode,
            message.Data.Startdatum
        );

    public async Task Handle(EventEnvelope<StartdatumWerdGewijzigdInKbo> message)
        => await _elasticRepository.UpdateStartdatum<VerenigingZoekDocument>(
            message.VCode,
            message.Data.Startdatum
        );

    public async Task Handle(EventEnvelope<EinddatumWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Einddatum = message.Data.Einddatum.ToString(WellknownFormats.DateOnly),
            }
        );

    public async Task Handle(EventEnvelope<DoelgroepWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Doelgroep = new Doelgroep
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, message.VCode),
                    Minimumleeftijd = message.Data.Doelgroep.Minimumleeftijd,
                    Maximumleeftijd = message.Data.Doelgroep.Maximumleeftijd,
                },
            }
        );

    public async Task Handle(EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                                                           .Select(
                                                                hoofdactiviteitVerenigingsloket =>
                                                                    new VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket
                                                                    {
                                                                        JsonLdMetadata =
                                                                            CreateJsonLdMetadata(
                                                                                JsonLdType.Hoofdactiviteit,
                                                                                hoofdactiviteitVerenigingsloket.Code),
                                                                        Code = hoofdactiviteitVerenigingsloket.Code,
                                                                        Naam = hoofdactiviteitVerenigingsloket.Naam,
                                                                    })
                                                           .ToArray(),
            });
    }

    public async Task Handle(EventEnvelope<WerkingsgebiedenWerdenNietBepaald> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Werkingsgebieden = [],
            });
    }

    public async Task Handle(EventEnvelope<WerkingsgebiedenWerdenBepaald> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Werkingsgebieden = message.Data.Werkingsgebieden
                                          .Select(
                                               werkingsgebied =>
                                                   new VerenigingZoekDocument.Types.Werkingsgebied
                                                   {
                                                       JsonLdMetadata =
                                                           CreateJsonLdMetadata(
                                                               JsonLdType.Werkingsgebied,
                                                               werkingsgebied.Code),
                                                       Code = werkingsgebied.Code,
                                                       Naam = werkingsgebied.Naam,
                                                   })
                                          .ToArray(),
            });
    }

    public async Task Handle(EventEnvelope<WerkingsgebiedenWerdenGewijzigd> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Werkingsgebieden = message.Data.Werkingsgebieden
                                          .Select(
                                               werkingsgebied =>
                                                   new VerenigingZoekDocument.Types.Werkingsgebied
                                                   {
                                                       JsonLdMetadata =
                                                           CreateJsonLdMetadata(
                                                               JsonLdType.Werkingsgebied,
                                                               werkingsgebied.Code),
                                                       Code = werkingsgebied.Code,
                                                       Naam = werkingsgebied.Naam,
                                                   })
                                          .ToArray(),
            });
    }

    public async Task Handle(EventEnvelope<WerkingsgebiedenWerdenNietVanToepassing> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Werkingsgebieden =
                [
                    new VerenigingZoekDocument.Types.Werkingsgebied
                    {
                        JsonLdMetadata =
                            CreateJsonLdMetadata(
                                JsonLdType.Werkingsgebied,
                                Werkingsgebied.NietVanToepassing.Code),
                        Code = Werkingsgebied.NietVanToepassing.Code,
                        Naam = Werkingsgebied.NietVanToepassing.Naam,
                    },
                ],
            });
    }

    public async Task Handle(EventEnvelope<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                IsUitgeschrevenUitPubliekeDatastroom = true,
            });
    }

    public async Task Handle(EventEnvelope<VerenigingWerdIngeschrevenInPubliekeDatastroom> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                IsUitgeschrevenUitPubliekeDatastroom = false,
            });
    }

    public async Task Handle(EventEnvelope<LocatieWerdToegevoegd> message)
    {
        await _elasticRepository.AppendLocatie<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Locatie, message.VCode));
    }

    public async Task Handle(EventEnvelope<LocatieWerdGewijzigd> message)
    {
        await _elasticRepository.UpdateLocatie<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Locatie, message.VCode));
    }

    public async Task Handle(EventEnvelope<LocatieWerdVerwijderd> message)
    {
        await _elasticRepository.RemoveLocatie<VerenigingZoekDocument>(
            message.VCode,
            message.Data.Locatie.LocatieId);
    }

    public async Task Handle(EventEnvelope<LidmaatschapWerdToegevoegd> message)
    {
        await _elasticRepository.AppendLidmaatschap<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Lidmaatschap, message.VCode));
    }

    public async Task Handle(EventEnvelope<LidmaatschapWerdGewijzigd> message)
    {
        await _elasticRepository.UpdateLidmaatschap<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Lidmaatschap, message.VCode));
    }

    public async Task Handle(EventEnvelope<LidmaatschapWerdVerwijderd> message)
    {
        await _elasticRepository.RemoveLidmaatschap<VerenigingZoekDocument>(
            message.VCode,
            message.Data.Lidmaatschap.LidmaatschapId);
    }

    private static VerenigingZoekDocument.Types.Locatie Map(Registratiedata.Locatie locatie, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, vCode, locatie.LocatieId.ToString()),

            LocatieId = locatie.LocatieId,
            Locatietype = locatie.Locatietype,
            Naam = locatie.Naam,
            Adresvoorstelling = locatie.Adres.ToAdresString(),
            IsPrimair = locatie.IsPrimair,
            Postcode = locatie.Adres?.Postcode ?? string.Empty,
            Gemeente = locatie.Adres?.Gemeente ?? string.Empty,
        };

    private static VerenigingZoekDocument.Types.Lidmaatschap Map(Registratiedata.Lidmaatschap lidmaatschap, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Lidmaatschap, vCode, lidmaatschap.LidmaatschapId.ToString()),

            LidmaatschapId = lidmaatschap.LidmaatschapId,
            AndereVereniging = lidmaatschap.AndereVereniging,
            DatumVan = lidmaatschap.DatumVan.FormatAsBelgianDate(),
            DatumTot = lidmaatschap.DatumTot.FormatAsBelgianDate(),
            Beschrijving = lidmaatschap.Beschrijving,
            Identificatie = lidmaatschap.Identificatie,
        };

    private static VerenigingZoekDocument.Types.Locatie Map(
        VerenigingZoekDocument.Types.Locatie locatie,
        Registratiedata.AdresUitAdressenregister adresUitAdressenregister,
        string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, vCode, locatie.LocatieId.ToString()),
            LocatieId = locatie.LocatieId,
            Locatietype = locatie.Locatietype,
            Naam = locatie.Naam,
            Adresvoorstelling = adresUitAdressenregister.ToAdresString(),
            IsPrimair = locatie.IsPrimair,
            Postcode = adresUitAdressenregister.Postcode ?? string.Empty,
            Gemeente = adresUitAdressenregister.Gemeente ?? string.Empty,
        };

    private static Doelgroep Map(Registratiedata.Doelgroep doelgroep, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, vCode),
            Minimumleeftijd = doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroep.Maximumleeftijd,
        };

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelWerdOvergenomenUitKbo> message)
    {
        await _elasticRepository.AppendLocatie<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Locatie, message.VCode));
    }

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> message)
    {
        await _elasticRepository.UpdateLocatie<VerenigingZoekDocument>(
            message.VCode,
            new VerenigingZoekDocument.Types.Locatie
            {
                JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, message.VCode, message.Data.LocatieId.ToString()),
                LocatieId = message.Data.LocatieId,
                Naam = message.Data.Naam,
                IsPrimair = message.Data.IsPrimair,
            });
    }

    public async Task Handle(EventEnvelope<VerenigingWerdGestopt> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Status = VerenigingStatus.Gestopt,
                Einddatum = message.Data.Einddatum.ToString(WellknownFormats.DateOnly),
            });
    }

    public async Task Handle(EventEnvelope<VerenigingWerdGestoptInKBO> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Status = VerenigingStatus.Gestopt,
                Einddatum = message.Data.Einddatum.ToString(WellknownFormats.DateOnly),
            });
    }

    public async Task Handle(EventEnvelope<VerenigingWerdVerwijderd> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                IsVerwijderd = true,
            });
    }

    public async Task Handle(EventEnvelope<NaamWerdGewijzigdInKbo> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Naam = message.Data.Naam,
            }
        );

    public async Task Handle(EventEnvelope<KorteNaamWerdGewijzigdInKbo> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                KorteNaam = message.Data.KorteNaam,
            }
        );

    public async Task Handle(EventEnvelope<RechtsvormWerdGewijzigdInKBO> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
                {
                    Code = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
                    Naam = Verenigingstype.Parse(message.Data.Rechtsvorm).Naam,
                },
            }
        );

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelWerdGewijzigdInKbo> message)
    {
        await _elasticRepository.UpdateLocatie<VerenigingZoekDocument>(
            message.VCode,
            Map(message.Data.Locatie, message.VCode));
    }

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelWerdVerwijderdUitKbo> message)
    {
        await _elasticRepository.RemoveLocatie<VerenigingZoekDocument>(
            message.VCode,
            message.Data.Locatie.LocatieId);
    }

    public async Task Handle(EventEnvelope<AdresWerdOvergenomenUitAdressenregister> message)
    {
        await _elasticRepository.UpdateAdres<VerenigingZoekDocument>(
            message.VCode,
            message.Data.LocatieId,
            message.Data.Adres.ToAdresString(),
            message.Data.Adres.Postcode,
            message.Data.Adres.Gemeente);
    }

    public async Task Handle(EventEnvelope<AdresWerdGewijzigdInAdressenregister> message)
    {
        await _elasticRepository.UpdateAdres<VerenigingZoekDocument>(
            message.VCode,
            message.Data.LocatieId,
            message.Data.Adres.ToAdresString(),
            message.Data.Adres.Postcode,
            message.Data.Adres.Gemeente);
    }

    public async Task Handle(EventEnvelope<LocatieDuplicaatWerdVerwijderdNaAdresMatch> message)
    {
        await _elasticRepository.RemoveLocatie<VerenigingZoekDocument>(
            message.VCode,
            message.Data.VerwijderdeLocatieId);
    }

    public async Task Handle(EventEnvelope<VerenigingWerdGemarkeerdAlsDubbelVan> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                IsDubbel = true,
            });
    }

    public async Task Handle(EventEnvelope<VerenigingAanvaarddeDubbeleVereniging> message)
    {
        await _elasticRepository.AppendCorresponderendeVCodes<VerenigingZoekDocument>(
            message.VCode,
            message.Data.VCodeDubbeleVereniging);
    }

    public async Task Handle(EventEnvelope<MarkeringDubbeleVerengingWerdGecorrigeerd> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                IsDubbel = false,
            });
    }

    public async Task Handle(EventEnvelope<VerenigingAanvaarddeCorrectieDubbeleVereniging> message)
    {
        await _elasticRepository.RemoveCorresponderendeVCode<VerenigingZoekDocument>(
            message.VCode,
            message.Data.VCodeDubbeleVereniging);
    }

    public async Task Handle(EventEnvelope<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                IsDubbel = false,
            });
    }

    public async Task Handle(EventEnvelope<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
                {
                    Code = Verenigingstype.VZER.Code,
                    Naam = Verenigingstype.VZER.Naam,
                },
                Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype
                {
                    Code = Verenigingssubtype.NietBepaald.Code,
                    Naam = Verenigingssubtype.NietBepaald.Naam,
                },
            });
    }

    public async Task Handle(EventEnvelope<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype
                {
                    Code = Verenigingssubtype.FeitelijkeVereniging.Code,
                    Naam = Verenigingssubtype.FeitelijkeVereniging.Naam,
                },
            });
    }

    public async Task Handle(EventEnvelope<SubtypeWerdTerugGezetNaarNietBepaald> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Verenigingssubtype = new VerenigingZoekDocument.Types.Verenigingssubtype
                {
                    Code = Verenigingssubtype.NietBepaald.Code,
                    Naam = Verenigingssubtype.NietBepaald.Naam,
                },
            });
    }

    private static JsonLdMetadata CreateJsonLdMetadata(JsonLdType jsonLdType, params string[] values)
        => new()
        {
            Id = jsonLdType.CreateWithIdValues(values),
            Type = jsonLdType.Type,
        };
}
