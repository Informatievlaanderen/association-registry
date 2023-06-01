namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System.Linq;
using System.Threading.Tasks;
using Events;
using Infrastructure.Extensions;
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
            new VerenigingDocument
            {
                VCode = message.Data.VCode,
                Type = new VerenigingDocument.VerenigingsType
                {
                    Code = Verenigingstype.FeitelijkeVereniging.Code,
                    Beschrijving = Verenigingstype.FeitelijkeVereniging.Beschrijving,
                },
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = message.Data.Locaties.Select(
                    loc => new VerenigingDocument.Locatie
                    {
                        Locatietype = loc.Locatietype,
                        Naam = loc.Naam,
                        Adres = loc.ToAdresString(),
                        Hoofdlocatie = loc.Hoofdlocatie,
                        Postcode = loc.Postcode,
                        Gemeente = loc.Gemeente,
                    }).ToArray(),
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                    .Select(
                        hoofdactiviteitVerenigingsloket =>
                            new VerenigingDocument.HoofdactiviteitVerenigingsloket
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
            new VerenigingDocument
            {
                VCode = message.Data.VCode,
                Type = new VerenigingDocument.VerenigingsType
                {
                    Code = Verenigingstype.Afdeling.Code,
                    Beschrijving = Verenigingstype.Afdeling.Beschrijving,
                },
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = message.Data.Locaties.Select(
                    loc => new VerenigingDocument.Locatie
                    {
                        Locatietype = loc.Locatietype,
                        Naam = loc.Naam,
                        Adres = loc.ToAdresString(),
                        Hoofdlocatie = loc.Hoofdlocatie,
                        Postcode = loc.Postcode,
                        Gemeente = loc.Gemeente,
                    }).ToArray(),
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                    .Select(
                        hoofdactiviteitVerenigingsloket =>
                            new VerenigingDocument.HoofdactiviteitVerenigingsloket
                            {
                                Code = hoofdactiviteitVerenigingsloket.Code,
                                Naam = hoofdactiviteitVerenigingsloket.Beschrijving,
                            })
                    .ToArray(),
                Doelgroep = _brolFeeder.Doelgroep,
                Activiteiten = _brolFeeder.Activiteiten.ToArray(),
            }
        );

    public async Task Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingDocument
            {
                VCode = message.Data.VCode,
                Type = new VerenigingDocument.VerenigingsType { Code = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Code, Beschrijving = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Beschrijving },
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = Array.Empty<VerenigingDocument.Locatie>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<VerenigingDocument.HoofdactiviteitVerenigingsloket>(),
                Doelgroep = "",
                Activiteiten = Array.Empty<string>(),
                Sleutels = new[]
                {
                    new VerenigingDocument.Sleutel
                    {
                        Bron = Bron.Kbo,
                        Waarde = message.Data.KboNummer,
                    },
                },
            }
        );

    public async Task Handle(EventEnvelope<NaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new VerenigingDocument
            {
                Naam = message.Data.Naam,
            }
        );

    public async Task Handle(EventEnvelope<KorteNaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new VerenigingDocument
            {
                KorteNaam = message.Data.KorteNaam,
            }
        );

    public void Handle(EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message)
    {
        _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingDocument
            {
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                    .Select(
                        hoofdactiviteitVerenigingsloket =>
                            new VerenigingDocument.HoofdactiviteitVerenigingsloket
                            {
                                Code = hoofdactiviteitVerenigingsloket.Code,
                                Naam = hoofdactiviteitVerenigingsloket.Beschrijving,
                            })
                    .ToArray(),
            });
    }

    private static VerenigingDocument.Locatie ToDocument( Registratiedata.Locatie loc)
        => new()
        {
            Locatietype = loc.Locatietype,
            Naam = loc.Naam,
            Adres = loc.ToAdresString(),
            Hoofdlocatie = loc.Hoofdlocatie,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
        };
}
