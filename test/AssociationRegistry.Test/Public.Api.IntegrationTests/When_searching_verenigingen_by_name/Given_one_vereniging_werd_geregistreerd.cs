namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_searching_verenigingen_by_name;

using AssociationRegistry.Public.Api.SearchVerenigingen;
using Fixtures;
using FluentAssertions;
using Nest;
using Xunit;

public class Given_one_vereniging_werd_geregistreerd : IClassFixture<PublicElasticFixture>
{
    private readonly HttpClient _httpClient;
    private readonly ElasticClient _elasticClient;

    const string VerenigingenZoekenOpNaam = "/v1/verenigingen/zoeken2?q=com";


    public Given_one_vereniging_werd_geregistreerd(PublicElasticFixture verenigingPublicApiFixture)
    {
        _httpClient = verenigingPublicApiFixture.HttpClient;
        _elasticClient = verenigingPublicApiFixture.ElasticClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _httpClient.GetAsync(VerenigingenZoekenOpNaam)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var responseMessage = await _httpClient.GetAsync(VerenigingenZoekenOpNaam);
        var content = await responseMessage.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_one_vereniging_werd_geregistreerd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_the_result_is_valid()
    {
        var result = await SearchVerenigingenController.Search(PublicElasticFixture.Naam, _elasticClient);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var result = await SearchVerenigingenController.Search("dena", _elasticClient);

        result.Hits.Should().HaveCount(0);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var result = await SearchVerenigingenController.Search("*dena*", _elasticClient);

        result.Hits.Should().HaveCount(1);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var result = await SearchVerenigingenController.Search("oudenaarde", _elasticClient);

        result.Hits.Should().HaveCount(1);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var result = await SearchVerenigingenController.Search(PublicElasticFixture.VCode, _elasticClient);

        result.Hits.Should().HaveCount(1);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_vCode()
    {
        var result = await SearchVerenigingenController.Search("0001", _elasticClient);

        result.Hits.Should().HaveCount(0);
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
