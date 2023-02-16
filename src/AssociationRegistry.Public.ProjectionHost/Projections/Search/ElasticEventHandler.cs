namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System.Linq;
using System.Threading.Tasks;
using Events;
using Infrastructure.Extensions;
using Schema.Search;

public class ElasticEventHandler
{
    private readonly IElasticRepository _elasticRepository;
    private readonly IVerenigingBrolFeeder _brolFeeder;

    public ElasticEventHandler(IElasticRepository elasticRepository, IVerenigingBrolFeeder brolFeeder)
    {
        _elasticRepository = elasticRepository;
        _brolFeeder = brolFeeder;
    }

    public async Task Handle(EventEnvelope<VerenigingWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingDocument
            {
                VCode = message.Data.VCode,
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = message.Data.Locaties.Select(ToDocument).ToArray(),
                Hoofdactiviteiten = message.Data.HoofdactiviteitenVerenigingsloket.Select(ToDocument).ToArray(),
                Doelgroep = _brolFeeder.Doelgroep,
                Activiteiten = _brolFeeder.Activiteiten.ToArray(),
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

    private static VerenigingDocument.Locatie ToDocument(VerenigingWerdGeregistreerd.Locatie loc)
        => new(loc.Locatietype, loc.Naam, loc.ToAdresString(), loc.Hoofdlocatie, loc.Postcode, loc.Gemeente);

    private static VerenigingDocument.Hoofdactiviteit ToDocument(VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
        => new(hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Beschrijving);
}
