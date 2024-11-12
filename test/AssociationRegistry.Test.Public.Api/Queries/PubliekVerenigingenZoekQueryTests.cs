namespace AssociationRegistry.Test.Public.Api.Queries;

using Admin.Api.Queries;
using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Vereniging;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class PubliekVerenigingenZoekQueryTests
{
    private readonly ITestOutputHelper _helper;
    private readonly IElasticClient _elasticClient;
    private readonly TypeMapping _typeMapping;

    public PubliekVerenigingenZoekQueryTests(GivenEventsFixture fixture, ITestOutputHelper helper)
    {
        _helper = helper;
        _elasticClient = fixture.ElasticClient;
        _typeMapping = fixture.ServiceProvider.GetRequiredService<TypeMapping>();
    }

    [Fact]
    public async Task? Given_More_Than_ElasticSearch_Context_Limit_Total_Count_Is_Actual_Number()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var totalCount = 0;
        var desiredCount = 10000;
        var batchCount = 500;

        do
        {
            var docs = new List<VerenigingZoekDocument>();

            for (var i = 0; i < batchCount; i++)
                docs.Add(new() { VCode = fixture.Create<VCode>() });

            await _elasticClient.BulkAsync(b => b.IndexMany(docs));
            totalCount += batchCount;
        } while (totalCount < desiredCount);

        var query = new PubliekVerenigingenZoekQuery(_elasticClient, _typeMapping);

        var actual = await query.ExecuteAsync(new PubliekVerenigingenZoekFilter("*", "vCode", [], new PaginationQueryParams()),
                                              CancellationToken.None);

        actual.Total.Should().BeGreaterThan(desiredCount);
    }
}
