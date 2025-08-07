namespace AssociationRegistry.Test.Admin.Api.Queries.BeheerVerenigingZoekQuery.When_Searching_On_Geotags;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Framework.Fixtures;
using Elastic.Clients.Elasticsearch;
using Hosts.Configuration.ConfigurationBindings;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;
using VerenigingStatus = AssociationRegistry.Admin.Schema.Constants.VerenigingStatus;

public class Given_NoGeotagsFixture : ElasticRepositoryFixture
{
    public Given_NoGeotagsFixture() : base(nameof(Given_NoGeotagsFixture))
    {
    }
}

public class Given_NoGeotags: IClassFixture<Given_NoGeotagsFixture>, IDisposable, IAsyncDisposable
{
    private readonly Given_NoGeotagsFixture _fixture;
    private readonly ITestOutputHelper _helper;
    private readonly ElasticsearchClient? _elasticClient;

    public Given_NoGeotags(Given_NoGeotagsFixture fixture, ITestOutputHelper helper)
    {
        _fixture = fixture;
        _helper = helper;
        _elasticClient = fixture.ElasticClient;
    }

    [Fact]
    public async ValueTask When_Searching_Without_Geotags_Then_Still_Returns_Vereniging()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var index = _elasticClient.Indices.Get(Indices.Index<VerenigingZoekDocument>()).Indices.First();

        var typeMapping = index.Value.Mappings;

        var verenigingZoekDocument = fixture.Create<VerenigingZoekDocument>();
        verenigingZoekDocument.VCode = fixture.Create<VCode>();
        verenigingZoekDocument.Status = VerenigingStatus.Actief;
        verenigingZoekDocument.Geotags = [];

        await _elasticClient.IndexAsync(verenigingZoekDocument);
        await _elasticClient.Indices.RefreshAsync(Indices.All);

        var query = new BeheerVerenigingenZoekQuery(_elasticClient, typeMapping, _fixture.ElasticSearchOptions);

        var actual = await query.ExecuteAsync(new BeheerVerenigingenZoekFilter($"vCode:{verenigingZoekDocument.VCode}", "vCode", new PaginationQueryParams()),
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
