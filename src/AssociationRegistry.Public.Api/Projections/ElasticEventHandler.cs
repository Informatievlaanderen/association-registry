namespace AssociationRegistry.Public.Api.Projections;

using System.Linq;
using Events;
using Nest;
using SearchVerenigingen;

public class ElasticEventHandler
{
    private readonly IElasticClient _elasticClient;
    private readonly IVerenigingBrolFeeder _brolFeeder;

    public ElasticEventHandler(IElasticClient elasticClient, IVerenigingBrolFeeder brolFeeder)
    {
        _elasticClient = elasticClient;
        _brolFeeder = brolFeeder;
    }

    public void HandleEvent(VerenigingWerdGeregistreerd message)
    {
        var document = new VerenigingDocument(
            message.VCode,
            message.Naam,
            _brolFeeder.KorteNaam,
            _brolFeeder.Hoofdlocatie,
            _brolFeeder.Locaties.ToArray(),
            _brolFeeder.Hoofdactiviteiten,
            _brolFeeder.Doelgroep,
            _brolFeeder.Activiteiten.ToArray());
        var response = _elasticClient.IndexDocument(document);

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }
}
