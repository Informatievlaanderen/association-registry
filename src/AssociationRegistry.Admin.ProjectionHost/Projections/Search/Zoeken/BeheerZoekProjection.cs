namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search.Zoeken;

using Constants;
using Events;
using JsonLdContext;
using Schema;
using Schema.Constants;
using Schema.Search;
using Vereniging;
using AdresFormatter = Formats.AdresFormatter;
using Doelgroep = Schema.Search.Doelgroep;

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
                Verenigingstype = new VerenigingZoekDocument.VerenigingsType
                {
                    Code = Verenigingstype.FeitelijkeVereniging.Code,
                    Naam = Verenigingstype.FeitelijkeVereniging.Naam,
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
                                                                    new VerenigingZoekDocument.HoofdactiviteitVerenigingsloket
                                                                    {
                                                                        JsonLdMetadata =
                                                                            CreateJsonLdMetadata(
                                                                                JsonLdType.Hoofdactiviteit,
                                                                                hoofdactiviteitVerenigingsloket.Code),
                                                                        Code = hoofdactiviteitVerenigingsloket.Code,
                                                                        Naam = hoofdactiviteitVerenigingsloket.Naam,
                                                                    })
                                                           .ToArray(),
                Sleutels = new[]
                {
                    new VerenigingZoekDocument.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                        Bron = Sleutelbron.VR,
                        Waarde = message.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                            Nummer = message.Data.VCode,
                        },
                    },
                },
            }
        );

    public async Task Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingZoekDocument
            {
                JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
                VCode = message.Data.VCode,
                Verenigingstype = new VerenigingZoekDocument.VerenigingsType
                {
                    Code = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
                    Naam = Verenigingstype.Parse(message.Data.Rechtsvorm).Naam,
                },
                Naam = message.Data.Naam,
                Roepnaam = string.Empty,
                KorteNaam = message.Data.KorteNaam,
                Status = VerenigingStatus.Actief,
                Locaties = Array.Empty<VerenigingZoekDocument.Locatie>(),
                Startdatum = message.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                Einddatum = null,
                Doelgroep = new Doelgroep
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, message.VCode),
                    Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                    Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
                },
                IsUitgeschrevenUitPubliekeDatastroom = false,
                HoofdactiviteitenVerenigingsloket = Array.Empty<VerenigingZoekDocument.HoofdactiviteitVerenigingsloket>(),
                Sleutels = new[]
                {
                    new VerenigingZoekDocument.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.VR.Waarde),
                        Bron = Sleutelbron.VR,
                        Waarde = message.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.VR.Waarde),
                            Nummer = message.Data.VCode,
                        },
                    },
                    new VerenigingZoekDocument.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.VCode, Sleutelbron.KBO.Waarde),
                        Bron = Sleutelbron.KBO,
                        Waarde = message.Data.KboNummer,
                        CodeerSysteem = CodeerSysteem.KBO,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.VCode, Sleutelbron.KBO.Waarde),
                            Nummer = message.Data.KboNummer,
                        },
                    },
                },
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
                                                                    new VerenigingZoekDocument.HoofdactiviteitVerenigingsloket
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

    private static VerenigingZoekDocument.Locatie Map(Registratiedata.Locatie locatie, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, vCode, locatie.LocatieId.ToString()),

            LocatieId = locatie.LocatieId,
            Locatietype = locatie.Locatietype,
            Naam = locatie.Naam,
            Adresvoorstelling = AdresFormatter.ToAdresString(locatie.Adres),
            IsPrimair = locatie.IsPrimair,
            Postcode = locatie.Adres?.Postcode ?? string.Empty,
            Gemeente = locatie.Adres?.Gemeente ?? string.Empty,
        };

    private static VerenigingZoekDocument.Locatie Map(VerenigingZoekDocument.Locatie locatie, Registratiedata.AdresUitAdressenregister adresUitAdressenregister, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Locatie, vCode, locatie.LocatieId.ToString()),
            LocatieId = locatie.LocatieId,
            Locatietype = locatie.Locatietype,
            Naam = locatie.Naam,
            Adresvoorstelling = AdresFormatter.ToAdresString(adresUitAdressenregister),
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
            new VerenigingZoekDocument.Locatie
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
                Verenigingstype = new VerenigingZoekDocument.VerenigingsType
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
            AdresFormatter.ToAdresString(message.Data.Adres),
            message.Data.Adres.Postcode,
            message.Data.Adres.Gemeente);
    }

    public async Task Handle(EventEnvelope<AdresWerdGewijzigdInAdressenregister> message)
    {
        await _elasticRepository.UpdateAdres<VerenigingZoekDocument>(
            message.VCode,
            message.Data.LocatieId,
            AdresFormatter.ToAdresString(message.Data.Adres),
            message.Data.Adres.Postcode,
            message.Data.Adres.Gemeente);
    }

    public async Task Handle(EventEnvelope<LocatieDuplicaatWerdVerwijderdNaAdresMatch> message)
    {
        await _elasticRepository.RemoveLocatie<VerenigingZoekDocument>(
            message.VCode,
            message.Data.VerwijderdeLocatieId);
    }

    private static JsonLdMetadata CreateJsonLdMetadata(JsonLdType jsonLdType, params string[] values)
        => new()
        {
            Id = jsonLdType.CreateWithIdValues(values),
            Type = jsonLdType.Type,
        };
}
