namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_searching_verenigingen_by_name;

using System.ComponentModel;
using System.Diagnostics;
using Fixtures;
using FluentAssertions;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Nest;
using Xunit;

public class Given_one_vereniging_werd_geregistreerd : IClassFixture<VerenigingPublicApiFixture>
{
    private readonly HttpClient _httpClient;

    const string VerenigingenZoekenOpNaam = "/v1/verenigingen/zoeken2?q=naam:bd";
    private const string VCode = "v000001";
    private const string Naam = "Feestcommittee Oudenaarde";

    public Given_one_vereniging_werd_geregistreerd(VerenigingPublicApiFixture verenigingPublicApiFixture)
    {
        _httpClient = verenigingPublicApiFixture.HttpClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _httpClient.GetAsync(VerenigingenZoekenOpNaam)).Should().BeSuccessful();


    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
            .BasicAuthentication("elastic", "local_development")
            .DefaultIndex("verenigingsregister-verenigingen");
        var client = new ElasticClient(settings);
        var esEventHandler = new ElasticEventHandler(client);
        //esEventHandler.HandleEvent(new VerenigingWerdGeregistreerd(VCode, Naam));

        var responseMessage = await _httpClient.GetAsync(VerenigingenZoekenOpNaam);
        var content = await responseMessage.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_one_vereniging_werd_geregistreerd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }

}

public class VerenigingWerdGeregistreerd
{
    public string VCode { get; }
    public string Naam { get; }

    public VerenigingWerdGeregistreerd(string vCode, string naam)
    {
        VCode = vCode;
        Naam = naam;
    }
}

public class Vereniging
{
    public string VCode { get; set; }
    public string Naam { get; set; }
}

public class ElasticEventHandler
{
    private readonly IElasticClient _elasticClient;

    public ElasticEventHandler(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public void HandleEvent(VerenigingWerdGeregistreerd message)
    {
        _elasticClient.IndexDocument(new Vereniging(){ VCode = message.VCode, Naam = message.Naam });
    }
}

/*public class ElasticSearchProjection: IProjection
{
    public void Apply(IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
    {
        var events = streams.SelectMany(x => x.Events).OrderBy(s => s.Sequence).Select(s => s.Data);

        foreach (var @event in events)
        {

        }
    }

    public Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams,
        CancellationToken cancellation)
    {
        Apply(operations, streams);
        return Task.CompletedTask;
    }
}*/
