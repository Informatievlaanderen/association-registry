namespace AssociationRegistry.Test.Public.Api.Queries.PubliekVerenigingenZoekQuery.When_Searching_On_Geotags;

using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.RequestModels;
using AssociationRegistry.Public.Schema.Search;
using AssociationRegistry.Test.Public.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Fixtures;
using FluentAssertions;
using Nest;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;
using VerenigingStatus = AssociationRegistry.Public.Schema.Constants.VerenigingStatus;

public class Given_NoGeotagsFixture : ElasticsearchClientFixture
{
    public Given_NoGeotagsFixture() : base(nameof(Given_NoGeotagsFixture))
    {
    }
}

public class Given_NoGeotags: IClassFixture<Given_NoGeotagsFixture>, IDisposable, IAsyncDisposable
{
    private readonly Given_NoGeotagsFixture _fixture;
    private readonly ITestOutputHelper _helper;
    private readonly IElasticClient? _elasticClient;

    public Given_NoGeotags(Given_NoGeotagsFixture fixture, ITestOutputHelper helper)
    {
        _fixture = fixture;
        _helper = helper;
        _elasticClient = fixture.ElasticClient;
    }

    [Fact]
    public async ValueTask When_Searching_Without_Geotags_Then_Still_Returns_Vereniging()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var index = _elasticClient.Indices.Get(Indices.Index<VerenigingZoekDocument>()).Indices.First();

        var typeMapping = index.Value.Mappings;

        var verenigingZoekDocument = fixture.Create<VerenigingZoekDocument>();
        verenigingZoekDocument.VCode = fixture.Create<VCode>();
        verenigingZoekDocument.Status = VerenigingStatus.Actief;
        verenigingZoekDocument.Geotags = [];

        await _elasticClient.IndexDocumentAsync(verenigingZoekDocument);
        await _elasticClient.Indices.RefreshAsync(Indices.All);

        var query = new PubliekVerenigingenZoekQuery(_elasticClient, typeMapping);

        var actual = await query.ExecuteAsync(new PubliekVerenigingenZoekFilter($"vCode:{verenigingZoekDocument.VCode}", "vCode", [], new PaginationQueryParams()),
                                              CancellationToken.None);

        var vereniging = actual.Documents.SingleOrDefault(x => x.VCode == verenigingZoekDocument.VCode);
        vereniging.Should().NotBeNull();
        vereniging!.Geotags.Should().BeEmpty();
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public async ValueTask DisposeAsync()
    {
        // TODO release managed resources here
    }
}
