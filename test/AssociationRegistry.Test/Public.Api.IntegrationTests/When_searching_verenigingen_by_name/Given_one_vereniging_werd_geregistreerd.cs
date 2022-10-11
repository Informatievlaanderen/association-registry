namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_searching_verenigingen_by_name;

using AssociationRegistry.Public.Api.SearchVerenigingen;
using Fixtures;
using FluentAssertions;
using Nest;
using Xunit;

public class Given_one_vereniging_werd_geregistreerd : IClassFixture<VerenigingPublicApiFixture>
{
    private readonly HttpClient _httpClient;

    const string VerenigingenZoekenOpNaam = "/v1/verenigingen/zoeken2?q=com";
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

    [Fact]
    public async Task? Then_something()
    {
        var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
            .BasicAuthentication("elastic", "local_development")
            .DefaultIndex("verenigingsregister-verenigingen");
        var client = new ElasticClient(settings);
        var queryString = Naam;

        var result = await SearchVerenigingenController.Search(queryString, client);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task? Then_something_else()
    {
        var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
            .BasicAuthentication("elastic", "local_development")
            .DefaultIndex("verenigingsregister-verenigingen");
        var client = new ElasticClient(settings);
        var queryString = Naam;

        var result = await SearchVerenigingenController.Search(queryString, client);

        result.Hits.Should().HaveCount(1);
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
