namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Events;
using Formatters;
using Schema.Constants;
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
                VCode = message.Data.VCode,
                Type = new VerenigingZoekDocument.VerenigingsType
                {
                    Code = Verenigingstype.FeitelijkeVereniging.Code,
                    Beschrijving = Verenigingstype.FeitelijkeVereniging.Beschrijving,
                },
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Status = VerenigingStatus.Actief,
                IsUitgeschrevenUitPubliekeDatastroom = message.Data.IsUitgeschrevenUitPubliekeDatastroom,
                Doelgroep = Map(message.Data.Doelgroep),
                Locaties = message.Data.Locaties.Select(Map).ToArray(),
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                                                           .Select(
                                                                hoofdactiviteitVerenigingsloket =>
                                                                    new VerenigingZoekDocument.HoofdactiviteitVerenigingsloket
                                                                    {
                                                                        Code = hoofdactiviteitVerenigingsloket.Code,
                                                                        Naam = hoofdactiviteitVerenigingsloket.Beschrijving,
                                                                    })
                                                           .ToArray(),
                Sleutels = Array.Empty<VerenigingZoekDocument.Sleutel>(),
                Relaties = Array.Empty<Relatie>(),
            }
        );

    public async Task Handle(EventEnvelope<AfdelingWerdGeregistreerd> message)
    {
        await _elasticRepository.IndexAsync(
            new VerenigingZoekDocument
            {
                VCode = message.Data.VCode,
                Type = new VerenigingZoekDocument.VerenigingsType
                {
                    Code = Verenigingstype.Afdeling.Code,
                    Beschrijving = Verenigingstype.Afdeling.Beschrijving,
                },
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Status = VerenigingStatus.Actief,
                IsUitgeschrevenUitPubliekeDatastroom = false,
                Doelgroep = Map(message.Data.Doelgroep),
                Locaties = message.Data.Locaties.Select(Map).ToArray(),
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                                                           .Select(
                                                                hoofdactiviteitVerenigingsloket =>
                                                                    new VerenigingZoekDocument.HoofdactiviteitVerenigingsloket
                                                                    {
                                                                        Code = hoofdactiviteitVerenigingsloket.Code,
                                                                        Naam = hoofdactiviteitVerenigingsloket.Beschrijving,
                                                                    })
                                                           .ToArray(),
                Sleutels = Array.Empty<VerenigingZoekDocument.Sleutel>(),
                Relaties = new[]
                {
                    new Relatie
                    {
                        Type = RelatieType.IsAfdelingVan.Beschrijving,
                        AndereVereniging = new GerelateerdeVereniging
                        {
                            KboNummer = message.Data.Moedervereniging.KboNummer,
                            VCode = message.Data.Moedervereniging.VCode,
                            Naam = message.Data.Moedervereniging.Naam,
                        },
                    },
                },
            }
        );

        if (!string.IsNullOrEmpty(message.Data.Moedervereniging.VCode))
            await _elasticRepository.AppendRelatie(message.Data.Moedervereniging.VCode, new Relatie
            {
                Type = RelatieType.IsAfdelingVan.InverseBeschrijving,
                AndereVereniging = new GerelateerdeVereniging
                {
                    KboNummer = string.Empty,
                    VCode = message.Data.VCode,
                    Naam = message.Data.Naam,
                },
            });
    }

    public async Task Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingZoekDocument
            {
                VCode = message.Data.VCode,
                Type = new VerenigingZoekDocument.VerenigingsType
                {
                    Code = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
                    Beschrijving = Verenigingstype.Parse(message.Data.Rechtsvorm).Beschrijving,
                },
                Naam = message.Data.Naam,
                Roepnaam = string.Empty,
                KorteNaam = message.Data.KorteNaam,
                Status = VerenigingStatus.Actief,
                Doelgroep = new Doelgroep
                {
                    Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                    Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
                },
                Locaties = Array.Empty<VerenigingZoekDocument.Locatie>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<VerenigingZoekDocument.HoofdactiviteitVerenigingsloket>(),
                Sleutels = new[]
                {
                    new VerenigingZoekDocument.Sleutel
                    {
                        Bron = Sleutelbron.Kbo,
                        Waarde = message.Data.KboNummer,
                    },
                },
                Relaties = Array.Empty<Relatie>(),
            }
        );

    public async Task Handle(EventEnvelope<NaamWerdGewijzigd> message)
    {
        await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new VerenigingZoekDocument
            {
                Naam = message.Data.Naam,
            }
        );
        await _elasticRepository.UpdateNaamInRelaties(
            new VerenigingZoekDocument
            {
                VCode = message.Data.VCode,
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

    public async Task Handle(EventEnvelope<DoelgroepWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                Doelgroep = new Doelgroep
                {
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
                                                                        Code = hoofdactiviteitVerenigingsloket.Code,
                                                                        Naam = hoofdactiviteitVerenigingsloket.Beschrijving,
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
            Map(message.Data.Locatie));
    }

    public async Task Handle(EventEnvelope<LocatieWerdGewijzigd> message)
    {
        await _elasticRepository.UpdateLocatie(
            message.VCode,
            Map(message.Data.Locatie));
    }

    public async Task Handle(EventEnvelope<LocatieWerdVerwijderd> message)
    {
        await _elasticRepository.RemoveLocatie(
            message.VCode,
            message.Data.Locatie.LocatieId);
    }

    private static VerenigingZoekDocument.Locatie Map(Registratiedata.Locatie locatie)
        => new()
        {
            LocatieId = locatie.LocatieId,
            Locatietype = locatie.Locatietype,
            Naam = locatie.Naam,
            Adresvoorstelling = locatie.Adres.ToAdresString(),
            IsPrimair = locatie.IsPrimair,
            Postcode = locatie.Adres?.Postcode ?? string.Empty,
            Gemeente = locatie.Adres?.Gemeente ?? string.Empty,
        };

    private static Doelgroep Map(Registratiedata.Doelgroep doelgroep)
        => new()
        {
            Minimumleeftijd = doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroep.Maximumleeftijd,
        };

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelWerdOvergenomenUitKbo> message)
    {
        await _elasticRepository.AppendLocatie(
            message.VCode,
            Map(message.Data.Locatie));
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

    public async Task Handle(EventEnvelope<VerenigingWerdGestopt> message)
    {
        await _elasticRepository.UpdateAsync(message.VCode, new VerenigingZoekDocument
                                                 { Status = VerenigingStatus.Gestopt });
    }
}
