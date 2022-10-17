namespace AssociationRegistry.Public.Api.Projections;

using Events;
using Nest;
using SearchVerenigingen;

public class ElasticEventHandler
{
    private readonly IElasticClient _elasticClient;

    public ElasticEventHandler(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public void HandleEvent(VerenigingWerdGeregistreerd message)
        => _elasticClient.IndexDocument(new VerenigingDocument(message.VCode, message.Naam ));
}
