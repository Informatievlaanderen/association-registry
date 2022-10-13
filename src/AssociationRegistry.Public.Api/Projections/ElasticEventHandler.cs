namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_searching_verenigingen_by_name;

using AssociationRegistry.Public.Api.SearchVerenigingen;
using Events;
using Nest;

public class ElasticEventHandler
{
    private readonly IElasticClient _elasticClient;

    public ElasticEventHandler(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public void HandleEvent(VerenigingWerdGeregistreerd message)
    {
        _elasticClient.IndexDocument(new VerenigingDocument(){ VCode = message.VCode, Naam = message.Naam });
    }
}
