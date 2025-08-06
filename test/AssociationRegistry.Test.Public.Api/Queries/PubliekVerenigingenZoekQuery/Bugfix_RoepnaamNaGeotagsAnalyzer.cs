namespace AssociationRegistry.Test.Public.Api.Queries.PubliekVerenigingenZoekQuery;

using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.RequestModels;
using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using Fixtures;
using FluentAssertions;
using Framework;
using Elastic.Clients.Elasticsearch;
using Vereniging;
using Xunit;

public class Bugfix_RoepnaamNaGeotagsAnalyzerFixture : ElasticsearchClientFixture
{
    public Bugfix_RoepnaamNaGeotagsAnalyzerFixture() : base(nameof(Bugfix_RoepnaamNaGeotagsAnalyzerFixture))
    {

    }
}

public class Bugfix_RoepnaamNaGeotagsAnalyzer : IClassFixture<Bugfix_RoepnaamNaGeotagsAnalyzerFixture>, IDisposable, IAsyncDisposable
{
    private readonly Bugfix_RoepnaamNaGeotagsAnalyzerFixture _fixture;
    private readonly ITestOutputHelper _helper;
    private readonly ElasticsearchClient? _elasticClient;
    private readonly Fixture _autoFixture;
    private readonly PubliekVerenigingenZoekQuery _query;
    private const string _roepnaam = "Roepnaam Et velit totam numquam voluptatibus quam ratione.2025-06-06T11:42:23.689Z";

    public Bugfix_RoepnaamNaGeotagsAnalyzer(Bugfix_RoepnaamNaGeotagsAnalyzerFixture fixture, ITestOutputHelper helper)
    {
        _fixture = fixture;
        _helper = helper;
        _elasticClient = fixture.ElasticClient;
        _autoFixture = new Fixture().CustomizePublicApi();

        _query = new PubliekVerenigingenZoekQuery(fixture.ElasticClient, fixture.TypeMapping, fixture.ElasticSearchOptions);
    }

    [Fact]
    public async ValueTask When_Searching_On_Roepnaam_Then_Returns_Vereniging()
    {
        var verenigingZoekDocument = new VerenigingZoekDocument()
        {
            VCode = _autoFixture.Create<VCode>(),
            Roepnaam = _roepnaam,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            IsVerwijderd = false,
            IsDubbel = false,
            Status = VerenigingStatus.Actief.StatusNaam
        };

        await _elasticClient!.IndexAsync(verenigingZoekDocument);
        await _elasticClient.Indices.RefreshAsync(Indices.All);

        var searchResponse = await _query.ExecuteAsync(new PubliekVerenigingenZoekFilter($"*", null, [], new PaginationQueryParams()), CancellationToken.None);

        searchResponse.Documents.Should().NotBeEmpty();

        searchResponse = await _query.ExecuteAsync(new PubliekVerenigingenZoekFilter($"roepnaam:\"{_roepnaam}\"", null, [], new PaginationQueryParams()), CancellationToken.None);

        searchResponse.Documents.Should().NotBeEmpty();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }
}
