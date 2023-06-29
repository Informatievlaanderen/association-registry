namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System.Linq;
using System.Threading.Tasks;
using Events;
using Formatters;
using Schema.Search;
using Vereniging;

public class ElasticEventHandler
{
    private readonly IElasticRepository _elasticRepository;
    private readonly IVerenigingBrolFeeder _brolFeeder;

    public ElasticEventHandler(IElasticRepository elasticRepository, IVerenigingBrolFeeder brolFeeder)
    {
        _elasticRepository = elasticRepository;
        _brolFeeder = brolFeeder;
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
                Doelgroep = _brolFeeder.Doelgroep,
                Activiteiten = _brolFeeder.Activiteiten.ToArray(),
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
                Doelgroep = _brolFeeder.Doelgroep,
                Activiteiten = _brolFeeder.Activiteiten.ToArray(),
                Sleutels = Array.Empty<VerenigingZoekDocument.Sleutel>(),
            }
        );

    public async Task Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingZoekDocument
            {
                VCode = message.Data.VCode,
                Type = new VerenigingZoekDocument.VerenigingsType { Code = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Code, Beschrijving = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Beschrijving },
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = Array.Empty<VerenigingZoekDocument.Locatie>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<VerenigingZoekDocument.HoofdactiviteitVerenigingsloket>(),
                Doelgroep = "",
                Activiteiten = Array.Empty<string>(),
                Sleutels = new[]
                {
                    new VerenigingZoekDocument.Sleutel
                    {
                        Bron = Verenigingsbron.Kbo,
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
}
