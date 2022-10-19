namespace AssociationRegistry.Public.Api.Projections;

using System.Linq;
using System.Threading.Tasks;
using Events;
using SearchVerenigingen;

public class ElasticEventHandler
{
    private readonly IElasticRepository _elasticRepository;
    private readonly IVerenigingBrolFeeder _brolFeeder;

    public ElasticEventHandler(IElasticRepository elasticRepository, IVerenigingBrolFeeder brolFeeder)
    {
        _elasticRepository = elasticRepository;
        _brolFeeder = brolFeeder;
    }

    public async Task HandleEvent(VerenigingWerdGeregistreerd message)
        => await _elasticRepository.IndexAsync(
            new VerenigingDocument(
                message.VCode,
                message.Naam,
                _brolFeeder.KorteNaam,
                _brolFeeder.Hoofdlocatie,
                _brolFeeder.Locaties.ToArray(),
                _brolFeeder.Hoofdactiviteiten,
                _brolFeeder.Doelgroep,
                _brolFeeder.Activiteiten.ToArray()
            )
        );
}
