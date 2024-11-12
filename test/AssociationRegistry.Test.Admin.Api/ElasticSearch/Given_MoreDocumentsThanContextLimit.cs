namespace AssociationRegistry.Test.Admin.Api.ElasticSearch;

using AssociationRegistry.Admin.Api.Verenigingen.Search;
using AssociationRegistry.Admin.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Admin.Schema.Search;
using AutoFixture;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Vereniging;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_MoreDocumentsThanContextLimit
{
    private readonly ITestOutputHelper _helper;
    private readonly IElasticClient _elasticClient;
    private readonly TypeMapping _typeMapping;

    public Given_MoreDocumentsThanContextLimit(EventsInDbScenariosFixture fixture, ITestOutputHelper helper)
    {
        _helper = helper;
        _elasticClient = fixture.ElasticClient;
        _typeMapping = fixture.ServiceProvider.GetRequiredService<TypeMapping>();
    }

    [Fact]
    public async Task? Total_Count_Is_Actual_Number_And_Not_Context_Limit()
    {
        var fixture = new Fixture().CustomizeAdminApi();

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

        var actual = await VerenigingZoekDocumentQuery.Search(_elasticClient, "*", "vCode", new PaginationQueryParams(), _typeMapping);
        actual.Total.Should().BeGreaterThan(desiredCount);
    }
}
