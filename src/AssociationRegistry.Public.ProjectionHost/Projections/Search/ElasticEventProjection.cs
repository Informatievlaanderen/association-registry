namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System.Linq;
using System.Threading.Tasks;
using Events;
using Formatters;
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
            }
        );

    public async Task Handle(EventEnvelope<AfdelingWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
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
            }
        );

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
                KorteNaam = message.Data.KorteNaam,
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

    public void Handle(EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message)
    {
        _elasticRepository.UpdateAsync(
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

    public void Handle(EventEnvelope<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> message)
    {
        _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                IsUitgeschrevenUitPubliekeDatastroom = true,
            });
    }

    public void Handle(EventEnvelope<VerenigingWerdIngeschrevenInPubliekeDatastroom> message)
    {
        _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                IsUitgeschrevenUitPubliekeDatastroom = false,
            });
    }

    public void Handle(EventEnvelope<LocatieWerdToegevoegd> message)
    {
        _elasticRepository.AppendLocatie(
            message.VCode,
            Map(message.Data.Locatie));
    }

    public void Handle(EventEnvelope<LocatieWerdGewijzigd> message)
    {
        _elasticRepository.ReplaceLocatie(
            message.VCode,
            Map(message.Data.Locatie));
    }

    public void Handle(EventEnvelope<LocatieWerdVerwijderd> message)
    {
        _elasticRepository.RemoveLocatie(
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

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdOvergenomenUitKbo> message)
    {
        _elasticRepository.AppendLocatie(
            message.VCode,
            Map(message.Data.Locatie));
    }
}
