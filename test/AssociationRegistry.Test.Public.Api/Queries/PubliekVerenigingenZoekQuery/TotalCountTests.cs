namespace AssociationRegistry.Test.Public.Api.Queries.PubliekVerenigingenZoekQuery;

using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Public.Schema.Search;
using AssociationRegistry.Test.Public.Api.Framework;
using AssociationRegistry.Test.Public.Api.When_Saving_A_Document_To_Elastic;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Nest;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;
using VerenigingStatus = AssociationRegistry.Public.Schema.Constants.VerenigingStatus;

public class PubliekVerenigingenZoekQuery_Fixture : ElasticRepositoryFixture
{
    public PubliekVerenigingenZoekQuery_Fixture() : base(nameof(PubliekVerenigingenZoekQuery_Fixture))
    {
    }
}

public class PubliekVerenigingenZoekQueryTests: IClassFixture<PubliekVerenigingenZoekQuery_Fixture>, IDisposable, IAsyncDisposable
{
    private readonly PubliekVerenigingenZoekQuery_Fixture _fixture;
    private readonly ITestOutputHelper _helper;
    private readonly IElasticClient? _elasticClient;

    public PubliekVerenigingenZoekQueryTests(PubliekVerenigingenZoekQuery_Fixture fixture, ITestOutputHelper helper)
    {
        _fixture = fixture;
        _helper = helper;
        _elasticClient = fixture.ElasticClient;
    }

    [Fact]
    public async ValueTask Given_More_Than_ElasticSearch_Context_Limit_Total_Count_Is_Actual_Number()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var totalCount = 0;
        var desiredCount = 10000;
        var batchCount = 500;

        var index = _elasticClient.Indices.Get(Indices.Index<VerenigingZoekDocument>()).Indices.First();

        var typeMapping = index.Value.Mappings;

        do
        {
            var docs = new List<VerenigingZoekDocument>();

            for (var i = 0; i < batchCount; i++)
            {
                var verenigingZoekDocument = fixture.Create<VerenigingZoekDocument>();
                verenigingZoekDocument.VCode = VCode.Create(10000 + i);
                verenigingZoekDocument.Status = VerenigingStatus.Actief;
                docs.Add(verenigingZoekDocument);
            }

            var result = await _elasticClient.BulkAsync(b => b.IndexMany(docs));
            if(!result.IsValid)
                _helper.WriteLine(result.DebugInformation);

            totalCount += batchCount;
        } while (totalCount < desiredCount);

        await _elasticClient.Indices.RefreshAsync(Indices.All);

        var query = new PubliekVerenigingenZoekQuery(_elasticClient, typeMapping);

        var actual = await query.ExecuteAsync(new PubliekVerenigingenZoekFilter("*", "vCode", [], new PaginationQueryParams()),
                                              CancellationToken.None);

        actual.Total.Should().Be(desiredCount);
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
