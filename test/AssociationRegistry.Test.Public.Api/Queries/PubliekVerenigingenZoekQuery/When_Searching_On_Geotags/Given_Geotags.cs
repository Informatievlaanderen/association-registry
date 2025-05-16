namespace AssociationRegistry.Test.Public.Api.Queries.PubliekVerenigingenZoekQuery.When_Searching_On_Geotags;

using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using FluentAssertions;
using Framework;
using Nest;
using Vereniging;
using When_Saving_A_Document_To_Elastic;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;
using VerenigingStatus = AssociationRegistry.Public.Schema.Constants.VerenigingStatus;

public class Given_GeotagsFixture : ElasticRepositoryFixture
{
    public Given_GeotagsFixture() : base(nameof(Given_GeotagsFixture))
    {
    }
}

public class Given_Geotags: IClassFixture<Given_GeotagsFixture>, IDisposable, IAsyncDisposable
{
    private readonly Given_GeotagsFixture _fixture;
    private readonly ITestOutputHelper _helper;
    private readonly IElasticClient? _elasticClient;

    public Given_Geotags(Given_GeotagsFixture fixture, ITestOutputHelper helper)
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
        verenigingZoekDocument.Geotags = [new VerenigingZoekDocument.Types.Geotag("BE02222")];

        await _elasticClient.IndexDocumentAsync(verenigingZoekDocument);
        await _elasticClient.Indices.RefreshAsync(Indices.All);

        var query = new PubliekVerenigingenZoekQuery(_elasticClient, typeMapping);

        var actual = await query.ExecuteAsync(new PubliekVerenigingenZoekFilter($"vCode:{verenigingZoekDocument.VCode}", "vCode", [], new PaginationQueryParams()),
                                              CancellationToken.None);

        var vereniging = actual.Documents.SingleOrDefault(x => x.VCode == verenigingZoekDocument.VCode);
        vereniging.Should().NotBeNull();
        vereniging!.Geotags.Should().BeEquivalentTo([new VerenigingZoekDocument.Types.Geotag("BE02222")]);
    }

    [Fact]
    public async ValueTask When_Searching_On_Different_Geotag_Then_Returns_Nothing()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var index = _elasticClient.Indices.Get(Indices.Index<VerenigingZoekDocument>()).Indices.First();

        var typeMapping = index.Value.Mappings;

        var verenigingZoekDocument = fixture.Create<VerenigingZoekDocument>();
        verenigingZoekDocument.VCode = fixture.Create<VCode>();
        verenigingZoekDocument.Status = VerenigingStatus.Actief;
        verenigingZoekDocument.Geotags = [new VerenigingZoekDocument.Types.Geotag("BE02222")];

        await _elasticClient.IndexDocumentAsync(verenigingZoekDocument);
        await _elasticClient.Indices.RefreshAsync(Indices.All);

        var query = new PubliekVerenigingenZoekQuery(_elasticClient, typeMapping);
        var actual = await query.ExecuteAsync(new PubliekVerenigingenZoekFilter($"geotags.identifier:BE03333", "vCode", [], new PaginationQueryParams()),
                                              CancellationToken.None);

        var vereniging = actual.Documents.SingleOrDefault(x => x.VCode == verenigingZoekDocument.VCode);
        vereniging.Should().BeNull();
    }

    [Fact]
    public async ValueTask When_Searching_On_Geotag_Then_Returns_Vereniging()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var index = _elasticClient.Indices.Get(Indices.Index<VerenigingZoekDocument>()).Indices.First();

        var typeMapping = index.Value.Mappings;

        var verenigingZoekDocument = fixture.Create<VerenigingZoekDocument>();
        verenigingZoekDocument.VCode = fixture.Create<VCode>();
        verenigingZoekDocument.Status = VerenigingStatus.Actief;
        verenigingZoekDocument.Geotags = [new VerenigingZoekDocument.Types.Geotag("BE02222")];

        await _elasticClient.IndexDocumentAsync(verenigingZoekDocument);
        await _elasticClient.Indices.RefreshAsync(Indices.All);

        var query = new PubliekVerenigingenZoekQuery(_elasticClient, typeMapping);

        await query.Explain(verenigingZoekDocument.VCode,
                      new PubliekVerenigingenZoekFilter($"geotags.identifier:BE02222", "vCode", [], new PaginationQueryParams()));
        var actual = await query.ExecuteAsync(new PubliekVerenigingenZoekFilter($"geotags.identifier:BE02222", "vCode", [], new PaginationQueryParams()),
                                              CancellationToken.None);

        var vereniging = actual.Documents.SingleOrDefault(x => x.VCode == verenigingZoekDocument.VCode);
        vereniging.Should().NotBeNull();
        vereniging!.Geotags.Should().BeEquivalentTo(verenigingZoekDocument.Geotags);
    }

    [Fact]
    public async ValueTask When_Searching_On_Multiple_Geotags_Then_Returns_Vereniging()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var index = _elasticClient.Indices.Get(Indices.Index<VerenigingZoekDocument>()).Indices.First();

        var typeMapping = index.Value.Mappings;

        var verenigingZoekDocument = fixture.Create<VerenigingZoekDocument>();
        verenigingZoekDocument.VCode = fixture.Create<VCode>();
        verenigingZoekDocument.Status = VerenigingStatus.Actief;
        verenigingZoekDocument.Geotags = [new VerenigingZoekDocument.Types.Geotag("BE02222")];

        await _elasticClient.IndexDocumentAsync(verenigingZoekDocument);
        await _elasticClient.Indices.RefreshAsync(Indices.All);

        var query = new PubliekVerenigingenZoekQuery(_elasticClient, typeMapping);
        var actual = await query.ExecuteAsync(new PubliekVerenigingenZoekFilter($"geotags.identifier:BE33333,BE02222", "vCode", [], new PaginationQueryParams()),
                                              CancellationToken.None);

        var vereniging = actual.Documents.SingleOrDefault(x => x.VCode == verenigingZoekDocument.VCode);
        vereniging.Should().NotBeNull();
        vereniging!.Geotags.Should().BeEquivalentTo(verenigingZoekDocument.Geotags);
    }

    [Fact]
    public async ValueTask When_Searching_On_Multiple_Geotags_Then_Returns_Vereniging2()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var index = _elasticClient.Indices.Get(Indices.Index<VerenigingZoekDocument>()).Indices.First();

        var typeMapping = index.Value.Mappings;

        var verenigingZoekDocument = fixture.Create<VerenigingZoekDocument>();
        verenigingZoekDocument.VCode = fixture.Create<VCode>();
        verenigingZoekDocument.Status = VerenigingStatus.Actief;
        verenigingZoekDocument.Geotags = [new VerenigingZoekDocument.Types.Geotag("BE33333,BE02222")];

        await _elasticClient.IndexDocumentAsync(verenigingZoekDocument);
        await _elasticClient.Indices.RefreshAsync(Indices.All);

        var query = new PubliekVerenigingenZoekQuery(_elasticClient, typeMapping);
        var actual = await query.ExecuteAsync(new PubliekVerenigingenZoekFilter($"geotags.identifier:BE33333,BE02222", "vCode", [], new PaginationQueryParams()),
                                              CancellationToken.None);

        var vereniging = actual.Documents.SingleOrDefault(x => x.VCode == verenigingZoekDocument.VCode);
        vereniging.Should().NotBeNull();
        vereniging!.Geotags.Should().BeEquivalentTo(verenigingZoekDocument.Geotags);
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
