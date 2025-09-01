namespace AssociationRegistry.Test.Admin.Api.Queries.BeheerVerenigingZoekQuery.When_Searching_With_Characters;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.RequestModels;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Elastic.Clients.Elasticsearch;
using FluentAssertions;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

public class Given_Dots_In_Name_Fixture : ElasticRepositoryFixture
{
    public Given_Dots_In_Name_Fixture() : base(nameof(Given_Dots_In_Name_Fixture))
    {

    }
}

public class Given_Dots_In_Name: IClassFixture<Given_Dots_In_Name_Fixture>, IDisposable, IAsyncDisposable
{
    private readonly Given_Dots_In_Name_Fixture _fixture;
    private readonly ITestOutputHelper _helper;
    private readonly ElasticsearchClient? _elasticClient;
    private readonly Fixture _autoFixture;
    private readonly BeheerVerenigingenZoekQuery _query;

    public Given_Dots_In_Name(Given_Dots_In_Name_Fixture fixture, ITestOutputHelper helper)
    {
        _fixture = fixture;
        _helper = helper;
        _elasticClient = fixture.ElasticClient;
        _autoFixture = new Fixture().CustomizeAdminApi();

        _query = new BeheerVerenigingenZoekQuery(fixture.ElasticClient, fixture.TypeMapping, fixture.ElasticSearchOptions);
    }

    [Fact]
    public async ValueTask When_Searching_Without_Dots()
    {
        var verenigingZoekDocument = _autoFixture.Create<VerenigingZoekDocument>();
        verenigingZoekDocument.Naam = "t.e.k.s.t.";
        await _elasticClient!.IndexAsync(verenigingZoekDocument);
        await _elasticClient.Indices.RefreshAsync(Indices.All);

        var actual = await ExecuteQuery($"tekst");

        actual.Documents.Should().Contain(x => x.VCode == verenigingZoekDocument.VCode);
    }

    private async ValueTask<SearchResponse<VerenigingZoekDocument>> ExecuteQuery(string query)
        => await _query.ExecuteAsync(new BeheerVerenigingenZoekFilter(query, "vCode", new PaginationQueryParams()),
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
