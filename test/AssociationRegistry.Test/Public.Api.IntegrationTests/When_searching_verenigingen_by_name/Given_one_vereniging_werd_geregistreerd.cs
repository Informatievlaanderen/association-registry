namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_searching_verenigingen_by_name;

using AssociationRegistry.Public.Api.SearchVerenigingen;
using Fixtures;
using FluentAssertions;
using Nest;
using Xunit;

public class One_vereniging_werd_geregistreerd_fixture: PublicElasticFixture {

    public const string VCode = "v000001";
    public const string Naam = "Feestcommittee Oudenaarde";

    public One_vereniging_werd_geregistreerd_fixture() : base(nameof(One_vereniging_werd_geregistreerd_fixture))
    {
        AddEvent(VCode, new VerenigingWerdGeregistreerd(VCode, Naam));
    }
}

public class Given_one_vereniging_werd_geregistreerd : IClassFixture<One_vereniging_werd_geregistreerd_fixture>
{
    private readonly One_vereniging_werd_geregistreerd_fixture _classFixture;
    private readonly string _goldenMasterWithOneVereniging;

    private const string VerenigingenZoekenOpNaam = "/v1/verenigingen/zoeken2?q=" + One_vereniging_werd_geregistreerd_fixture.Naam;
    private const string VerenigingenZoekenOpDeelVanEenTermVanDeNaam = "/v1/verenigingen/zoeken2?q=dena";
    private const string VerenigingenZoekenOpDeelVanNaamMetWildcards = "/v1/verenigingen/zoeken2?q=*dena*";
    private const string VerenigingenZoekenOpTermInNaam = "/v1/verenigingen/zoeken2?q=oudenaarde";

    private const string VerenigingenZoekenOpVCode = "/v1/verenigingen/zoeken2?q=" + One_vereniging_werd_geregistreerd_fixture.VCode;
    private const string VerenigingenZoekenOpDeelVanDeVCode = "/v1/verenigingen/zoeken2?q=001";

    private const string EmptyArrayResponse = "[]";

    public Given_one_vereniging_werd_geregistreerd(One_vereniging_werd_geregistreerd_fixture classFixture)
    {
        _classFixture = classFixture;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_one_vereniging_werd_geregistreerd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _classFixture.GetResponseMessage(VerenigingenZoekenOpNaam)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpNaam);

        content.Should().BeEquivalentJson(_goldenMasterWithOneVereniging);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpDeelVanEenTermVanDeNaam);

        content.Should().BeEquivalentJson(EmptyArrayResponse);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpDeelVanNaamMetWildcards);

        content.Should().BeEquivalentJson(_goldenMasterWithOneVereniging);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpTermInNaam);

        content.Should().BeEquivalentJson(_goldenMasterWithOneVereniging);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpVCode);

        content.Should().BeEquivalentJson(_goldenMasterWithOneVereniging);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_vCode()
    {
        var content = await _classFixture.Search(VerenigingenZoekenOpDeelVanDeVCode);

        content.Should().BeEquivalentJson(EmptyArrayResponse);
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
