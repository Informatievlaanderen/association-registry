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
    {
        var brolFeeder = new VerenigingBrolFeeder();

        var document = new VerenigingDocument(
            message.VCode,
            message.Naam,
            brolFeeder.KorteNaam,
            brolFeeder.Hoofdlocatie,
            brolFeeder.AndereLocaties,
            brolFeeder.PROTPUT,
            brolFeeder.Doelgroep);
        _elasticClient.IndexDocument(document);
    }
}
