namespace AssociationRegistry.Public.Api.Projections;

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
            _brolFeeder.AndereLocaties,
            _brolFeeder.Hoofdactiviteit,
            _brolFeeder.Doelgroep);
        _elasticClient.IndexDocument(document);
    }
}
