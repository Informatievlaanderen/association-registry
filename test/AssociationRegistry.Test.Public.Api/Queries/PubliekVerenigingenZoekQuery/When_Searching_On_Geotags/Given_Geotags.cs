namespace AssociationRegistry.Test.Public.Api.Queries.PubliekVerenigingenZoekQuery.When_Searching_On_Geotags;

using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using Fixtures;
using FluentAssertions;
using Framework;
using Elastic.Clients.Elasticsearch;
using Vereniging;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;
using VerenigingStatus = AssociationRegistry.Public.Schema.Constants.VerenigingStatus;

public class Given_GeotagsFixture : ElasticsearchClientFixture
{
    public Given_GeotagsFixture() : base(nameof(Given_GeotagsFixture))
    {

    }
}

public class Given_Geotags: IClassFixture<Given_GeotagsFixture>, IDisposable, IAsyncDisposable
{
    private readonly Given_GeotagsFixture _fixture;
    private readonly ITestOutputHelper _helper;
    private readonly ElasticsearchClient? _elasticClient;
    private readonly Fixture _autoFixture;
    private readonly PubliekVerenigingenZoekQuery _query;

    public Given_Geotags(Given_GeotagsFixture fixture, ITestOutputHelper helper)
    {
        _fixture = fixture;
        _helper = helper;
        _elasticClient = fixture.ElasticClient;
        _autoFixture = new Fixture().CustomizePublicApi();

        _query = new PubliekVerenigingenZoekQuery(fixture.ElasticClient, fixture.TypeMapping, fixture.ElasticSearchOptions);
    }

    [Fact]
    public async ValueTask When_Searching_Without_Geotags_Then_Still_Returns_Vereniging()
    {
        var geotag = "BE02222";
        var documents = await IndexDocumentsWithGeotags([geotag]);

        var actual = await ExecuteQuery($"vCode:{documents.First().VCode}");

        ShouldFindVerenigingenWithGeotag(actual, documents);
    }

    [Fact]
    public async ValueTask When_Searching_On_Different_Geotag_Then_Returns_Nothing()
    {
        var geotag = "BE02222";
        var documents = await IndexDocumentsWithGeotags([geotag]);

        var actual = await ExecuteQuery("geotags.identificatie:BE03333");

        ShouldNotHaveVereniging(actual, documents[0].VCode);
    }

    [Fact]
    public async ValueTask When_Searching_On_Geotag_Then_Returns_Vereniging()
    {
        var geotag = "BE02222";
        var documents = await IndexDocumentsWithGeotags([geotag]);

        var actual = await ExecuteQuery($"geotags.identificatie:{geotag}");

        ShouldFindVerenigingenWithGeotag(actual, documents);
    }

    [Theory]
    [InlineData("BE33333 BE02222")]
    public async ValueTask CommasAndSpacesLeadsToTokenSplitting(string geotags)
    {
        var geotag = "BE02222";
        var documents = await IndexDocumentsWithGeotags([geotag]);

        var actual = await ExecuteQuery($"geotags.identificatie:{geotags}");

        ShouldFindVerenigingenWithGeotag(actual, documents);
    }

    [Fact]
    public async ValueTask When_Searching_On_Multiple_Geotags_Then_Returns_Multiple_Verenigingen()
    {
        var geotag = "BE02222";
        var geotag2 = "BE99999";
        var documents = await IndexDocumentsWithGeotags([geotag, _autoFixture.Create<string>()], [geotag2, _autoFixture.Create<string>()]);

        var actual = await ExecuteQuery($"geotags.identificatie:({geotag} OR {geotag2})");

        ShouldFindVerenigingenWithGeotag(actual, documents);
    }

    [Fact]
    public async ValueTask When_Searching_On_Multiple_Geotags_Then_Returns_Multiple_Verenigingen2()
    {
        var documents = await IndexDocumentsWithGeotags(
            ["BE33333", "BE02222"],
            ["BE88888", "BE99999"]
        );

        var actual = await ExecuteQuery("geotags.identificatie:(BE33333 OR BE99999 OR BE11111)");

        ShouldFindVerenigingenWithGeotag(actual, documents);
    }

    [Theory]
    [InlineData("BE33333 BE02222")]
    [InlineData("BE33333 OR BE02222 AND RoepNaam")]
    [InlineData("(BE33333 OR BE02222)")]
    [InlineData("(BE33333 AND BE02222)")]
    [InlineData("BE33333 OR BE02222 AND RoepNaam")]
    [InlineData("BE33333 AND BE02222 AND RoepNaam")]
    [InlineData("(BE33333 AND BE02222) AND RoepNaam")]
    [InlineData("BE33333,BE02222")]
    public async ValueTask Found(string geotags)
    {
        var geotag = "BE02222";
        var geotag2 = "BE33333";
        var geotag3 = "BE33334";
        var documents = await IndexDocumentsWithGeotags([geotag, geotag2, geotag3]);

        var actual = await ExecuteQuery($"geotags.identificatie:{geotags}");

        ShouldFindVerenigingenWithGeotag(actual, documents);
    }

    [Theory]
    [InlineData("[BE33333 OR BE02222]")]
    [InlineData("[BE33333 AND BE02222]")]
    [InlineData("(BE33333 AND BE02222) AND KorteNaam")]
    public async ValueTask NotFound(string geotags)
    {
        var geotag = "BE02222";
        var geotag2 = "BE33333";
        var geotag3 = "BE33334";
        var documents = await IndexDocumentsWithGeotags([geotag, geotag2, geotag3]);

        var actual = await ExecuteQuery($"geotags.identificatie:{geotags}");

        actual.Documents.Should().BeEmpty();
    }

    private static void ShouldFindVerenigingenWithGeotag(SearchResponse<VerenigingZoekDocument> actual, VerenigingZoekDocument[] verenigingZoekDocuments)
    {
        foreach (var verenigingZoekDocument in verenigingZoekDocuments)
        {
            var vereniging = actual.Documents.SingleOrDefault(x => x.VCode == verenigingZoekDocument.VCode);
            vereniging.Should().NotBeNull();
            vereniging!.Geotags.Select(x => x.Identificatie).Should().BeEquivalentTo(verenigingZoekDocument.Geotags.Select(x => x.Identificatie));
        }
    }

    private async Task<VerenigingZoekDocument[]> IndexDocumentsWithGeotags(params string[][] geotagsCollection)
    {
        var docs = new List<VerenigingZoekDocument>();

        foreach (var geotags in geotagsCollection)
        {
            var verenigingZoekDocument = _autoFixture.Create<VerenigingZoekDocument>();
            verenigingZoekDocument.Geotags = geotags.Select(x => new VerenigingZoekDocument.Types.Geotag(x)).ToArray();
            verenigingZoekDocument.Roepnaam = "RoepNaam";
           docs.Add(verenigingZoekDocument);
           await _elasticClient!.IndexAsync(verenigingZoekDocument);
        }

        await _elasticClient.Indices.RefreshAsync(Indices.All);

        return docs.ToArray();
    }

    private static void ShouldNotHaveVereniging(SearchResponse<VerenigingZoekDocument> actual, string vCode)
    {
        var vereniging = actual.Documents.SingleOrDefault(x => x.VCode == vCode);
        vereniging.Should().BeNull();
    }

    private async ValueTask<SearchResponse<VerenigingZoekDocument>> ExecuteQuery(string query)
        => await _query.ExecuteAsync(new PubliekVerenigingenZoekFilter(query, "vCode", [], new PaginationQueryParams()),
                                     CancellationToken.None);

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public async ValueTask DisposeAsync()
    {
        // TODO release managed resources here
    }
}
