namespace AssociationRegistry.Public.Api.Projections;

using System.Linq;
using System.Threading.Tasks;
using Extensions;
using SearchVerenigingen;
using Vereniging;

public class ElasticEventHandler : IDomainEventHandler<VerenigingWerdGeregistreerd>
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
                message.KorteNaam,
                message.Locaties?.Select(ToDocument).ToArray() ?? System.Array.Empty<VerenigingDocument.Locatie>(),
                _brolFeeder.Hoofdactiviteiten,
                _brolFeeder.Doelgroep,
                _brolFeeder.Activiteiten.ToArray()
            )
        );

    private static VerenigingDocument.Locatie ToDocument(VerenigingWerdGeregistreerd.Locatie loc)
        => new(loc.LocatieType, loc.Naam, loc.ToAdresString(), loc.HoofdLocatie, loc.Postcode, loc.Gemeente);


}
