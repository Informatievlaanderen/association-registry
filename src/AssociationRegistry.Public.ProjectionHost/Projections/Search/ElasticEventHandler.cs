namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System.Linq;
using System.Threading.Tasks;
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

    public async Task Handle(EventEnvelope<VerenigingWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingDocument(
                message.Data.VCode,
                message.Data.Naam,
                message.Data.KorteNaam,
                message.Data.Locaties?.Select(ToDocument).ToArray() ?? Array.Empty<VerenigingDocument.Locatie>(),
                _brolFeeder.Hoofdactiviteiten,
                _brolFeeder.Doelgroep,
                _brolFeeder.Activiteiten.ToArray()
            )
        );

    private static VerenigingDocument.Locatie ToDocument(VerenigingWerdGeregistreerd.Locatie loc)
        => new(loc.LocatieType, loc.Naam, loc.ToAdresString(), loc.HoofdLocatie, loc.Postcode, loc.Gemeente);
}
