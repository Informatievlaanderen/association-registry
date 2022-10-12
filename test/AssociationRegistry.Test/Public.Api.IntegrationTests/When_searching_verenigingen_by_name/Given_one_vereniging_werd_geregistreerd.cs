namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_searching_verenigingen_by_name;

using AssociationRegistry.Public.Api.SearchVerenigingen;
using Fixtures;
using FluentAssertions;
using Nest;
using Xunit;

public class Given_one_vereniging_werd_geregistreerd : IClassFixture<PublicElasticFixture>
{
    private readonly HttpClient _httpClient;

    const string VerenigingenZoekenOpNaam = "/v1/verenigingen/zoeken2?q=" + PublicElasticFixture.Naam;
    const string VerenigingenZoekenOpDeelVanEenTermVanDeNaam = "/v1/verenigingen/zoeken2?q=dena";
    const string VerenigingenZoekenOpDeelVanNaamMetWildcards = "/v1/verenigingen/zoeken2?q=*dena*";
    const string VerenigingenZoekenOpTermInNaam = "/v1/verenigingen/zoeken2?q=oudenaarde";

    const string VerenigingenZoekenOpVCode = "/v1/verenigingen/zoeken2?q=" + PublicElasticFixture.VCode;
    const string VerenigingenZoekenOpDeelVanDeVCode = "/v1/verenigingen/zoeken2?q=001";

    private const string EmptyArray = "[]";

    public Given_one_vereniging_werd_geregistreerd(PublicElasticFixture verenigingPublicApiFixture)
    {
        _httpClient = verenigingPublicApiFixture.HttpClient;
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
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var responseMessage = await _httpClient.GetAsync(VerenigingenZoekenOpDeelVanEenTermVanDeNaam);
        var content = await responseMessage.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(EmptyArray);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var responseMessage = await _httpClient.GetAsync(VerenigingenZoekenOpDeelVanNaamMetWildcards);
        var content = await responseMessage.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_one_vereniging_werd_geregistreerd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var responseMessage = await _httpClient.GetAsync(VerenigingenZoekenOpTermInNaam);

        var content = await responseMessage.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_one_vereniging_werd_geregistreerd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var responseMessage = await _httpClient.GetAsync(VerenigingenZoekenOpVCode);
        var content = await responseMessage.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_one_vereniging_werd_geregistreerd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_vCode()
    {
        var responseMessage = await _httpClient.GetAsync(VerenigingenZoekenOpDeelVanDeVCode);
        var content = await responseMessage.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(EmptyArray);
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
