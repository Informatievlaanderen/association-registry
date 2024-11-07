namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Events;
using Formats;
using Infrastructure.Extensions;
using JsonLdContext;
using Schema.Constants;
using Schema.Detail;
using Schema.Search;
using Vereniging;
using Doelgroep = Schema.Search.Doelgroep;

public class PubliekZoekProjectionHandler
{
    private readonly IElasticRepository _elasticRepository;

    public PubliekZoekProjectionHandler(IElasticRepository elasticRepository)
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
                KorteBeschrijving = message.Data.KorteBeschrijving,
                Status = VerenigingStatus.Actief,
                IsUitgeschrevenUitPubliekeDatastroom = message.Data.IsUitgeschrevenUitPubliekeDatastroom,
                Doelgroep = Map(message.Data.Doelgroep, message.Data.VCode),
                Locaties = message.Data.Locaties.Select(locatie => Map(locatie, message.VCode)).ToArray(),
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
                Werkingsgebieden = message.Data.Werkingsgebieden?
                                                           .Select(
                                                                werkingsgebied =>
                                                                    new VerenigingZoekDocument.Werkingsgebied()
                                                                    {
                                                                        JsonLdMetadata =
                                                                            CreateJsonLdMetadata(
                                                                                JsonLdType.Werkingsgebied,
                                                                                werkingsgebied.Code),
                                                                        Code = werkingsgebied.Code,
                                                                        Naam = werkingsgebied.Naam,
                                                                    })
                                                           .ToArray() ?? [],
                Lidmaatschappen = [],
                Sleutels =
                    new[]
                    {
                        new VerenigingZoekDocument.Sleutel
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.Data.VCode, Sleutelbron.VR),
                            Bron = Sleutelbron.VR,
                            Waarde = message.Data.VCode,
                            CodeerSysteem = CodeerSysteem.VR,
                            GestructureerdeIdentificator = new VerenigingZoekDocument.GestructureerdeIdentificator
                            {
                                JsonLdMetadata =
                                    CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.Data.VCode, Sleutelbron.VR),
                                Nummer = message.Data.VCode,
                            },
                        },
                    },
                Relaties = Array.Empty<Relatie>(),
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
                KorteBeschrijving = string.Empty,
                Status = VerenigingStatus.Actief,
                Doelgroep = new Doelgroep
                {
                    JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Doelgroep, message.VCode),
                    Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                    Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
                },
                Locaties = Array.Empty<VerenigingZoekDocument.Locatie>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<VerenigingZoekDocument.HoofdactiviteitVerenigingsloket>(),
                Werkingsgebieden = Array.Empty<VerenigingZoekDocument.Werkingsgebied>(),
                Lidmaatschappen = [],
                Sleutels = new[]
                {
                    new VerenigingZoekDocument.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.Data.VCode, Sleutelbron.VR),
                        Bron = Sleutelbron.VR,
                        Waarde = message.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.Data.VCode, Sleutelbron.VR),
                            Nummer = message.Data.VCode,
                        },
                    },
                    new VerenigingZoekDocument.Sleutel
                    {
                        JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Sleutel, message.Data.VCode, Sleutelbron.KBO),
                        Bron = Sleutelbron.KBO,
                        Waarde = message.Data.KboNummer,
                        CodeerSysteem = CodeerSysteem.KBO,
                        GestructureerdeIdentificator = new VerenigingZoekDocument.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.GestructureerdeSleutel, message.Data.VCode, Sleutelbron.KBO),
                            Nummer = message.Data.KboNummer,
                        },
                    },
                },
                Relaties = Array.Empty<Relatie>(),
            }
        );

    public async Task Handle(EventEnvelope<NaamWerdGewijzigd> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Naam = message.Data.Naam,
            }
        );
    }

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

    public async Task Handle(EventEnvelope<KorteBeschrijvingWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new VerenigingZoekDocument
            {
                KorteBeschrijving = message.Data.KorteBeschrijving,
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

    public async Task Handle(EventEnvelope<WerkingsgebiedenWerdenGewijzigd> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Werkingsgebieden = message.Data.Werkingsgebieden
                                                           .Select(
                                                                werkingsgebied =>
                                                                    new VerenigingZoekDocument.Werkingsgebied()
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
        await _elasticRepository.AppendLocatie(
            message.VCode,
            Map(message.Data.Locatie, message.VCode));
    }

    public async Task Handle(EventEnvelope<LocatieWerdGewijzigd> message)
    {
        await _elasticRepository.UpdateLocatie(
            message.VCode,
            Map(message.Data.Locatie, message.VCode));
    }

    public async Task Handle(EventEnvelope<LocatieWerdVerwijderd> message)
    {
        await _elasticRepository.RemoveLocatie(
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
            Adresvoorstelling = locatie.Adres.ToAdresString(),
            IsPrimair = locatie.IsPrimair,
            Postcode = locatie.Adres?.Postcode ?? string.Empty,
            Gemeente = locatie.Adres?.Gemeente ?? string.Empty,
        };

    private static VerenigingZoekDocument.Locatie Map(
        VerenigingZoekDocument.Locatie locatie,
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
        await _elasticRepository.AppendLocatie(
            message.VCode,
            Map(message.Data.Locatie, message.VCode));
    }

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> message)
    {
        await _elasticRepository.UpdateLocatie(
            message.VCode,
            new VerenigingZoekDocument.Locatie
            {
                LocatieId = message.Data.LocatieId,
                Naam = message.Data.Naam,
                IsPrimair = message.Data.IsPrimair,
            });
    }

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelWerdGewijzigdInKbo> message)
    {
        await _elasticRepository.UpdateLocatie(
            message.VCode,
            Map(message.Data.Locatie, message.VCode));
    }

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelWerdVerwijderdUitKbo> message)
    {
        await _elasticRepository.RemoveLocatie(
            message.VCode,
            message.Data.Locatie.LocatieId);
    }

    public async Task Handle(EventEnvelope<VerenigingWerdGestopt> message)
    {
        await _elasticRepository.UpdateAsync(message.VCode, new VerenigingZoekDocument
                                                 { Status = VerenigingStatus.Gestopt });
    }

    public async Task Handle(EventEnvelope<VerenigingWerdGestoptInKBO> message)
    {
        await _elasticRepository.UpdateAsync(message.VCode, new VerenigingZoekDocument
                                                 { Status = VerenigingStatus.Gestopt });
    }

    public async Task Handle(EventEnvelope<VerenigingWerdVerwijderd> message)
    {
        await _elasticRepository.UpdateAsync(message.VCode, new VerenigingZoekDocument
                                                 { IsVerwijderd = true });
    }

    public async Task Handle(EventEnvelope<NaamWerdGewijzigdInKbo> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Naam = message.Data.Naam,
            }
        );
    }

    public async Task Handle(EventEnvelope<KorteNaamWerdGewijzigdInKbo> message)
    {
        await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                KorteNaam = message.Data.KorteNaam,
            }
        );
    }

    public async Task Handle(EventEnvelope<RechtsvormWerdGewijzigdInKBO> message)
    {
        await _elasticRepository.UpdateAsync(
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
    }

    public async Task Handle(EventEnvelope<AdresWerdOvergenomenUitAdressenregister> message)
    {
        await _elasticRepository.UpdateAdres(
            message.VCode,
            message.Data.LocatieId,
            message.Data.Adres.ToAdresString(),
            message.Data.Adres.Postcode,
            message.Data.Adres.Gemeente);
    }

    public async Task Handle(EventEnvelope<AdresWerdGewijzigdInAdressenregister> message)
    {
        await _elasticRepository.UpdateAdres(
            message.VCode,
            message.Data.LocatieId,
            message.Data.Adres.ToAdresString(),
            message.Data.Adres.Postcode,
            message.Data.Adres.Gemeente);
    }

    public async Task Handle(EventEnvelope<LocatieDuplicaatWerdVerwijderdNaAdresMatch> message)
    {
        await _elasticRepository.RemoveLocatie(
            message.VCode,
            message.Data.VerwijderdeLocatieId);
    }

    public async Task Handle(EventEnvelope<LidmaatschapWerdToegevoegd> message)
    {
        await _elasticRepository.AppendLidmaatschap(
            message.VCode,
            Map(message.Data.Lidmaatschap, message.VCode));
    }

    public async Task Handle(EventEnvelope<LidmaatschapWerdGewijzigd> message)
    {
        await _elasticRepository.UpdateLidmaatschap(
            message.VCode,
            Map(message.Data.Lidmaatschap, message.VCode));
    }

    public async Task Handle(EventEnvelope<LidmaatschapWerdVerwijderd> message)
    {
        await _elasticRepository.RemoveLidmaatschap(
            message.VCode,
            message.Data.Lidmaatschap.LidmaatschapId);
    }


    private static VerenigingZoekDocument.Lidmaatschap Map(Registratiedata.Lidmaatschap lidmaatschap, string vCode)
        => new()
        {
            JsonLdMetadata = CreateJsonLdMetadata(JsonLdType.Lidmaatschap, vCode, lidmaatschap.LidmaatschapId.ToString()),

            LidmaatschapId = lidmaatschap.LidmaatschapId,
            AndereVereniging = lidmaatschap.AndereVereniging,
            DatumVan = lidmaatschap.DatumVan.ToBelgianDate(),
            DatumTot = lidmaatschap.DatumTot.ToBelgianDate(),
            Beschrijving = lidmaatschap.Beschrijving,
            Identificatie = lidmaatschap.Identificatie,
        };

    private static JsonLdMetadata CreateJsonLdMetadata(JsonLdType jsonLdType, params string[] values)
        => new()
        {
            Id = jsonLdType.CreateWithIdValues(values),
            Type = jsonLdType.Type,
        };
}
