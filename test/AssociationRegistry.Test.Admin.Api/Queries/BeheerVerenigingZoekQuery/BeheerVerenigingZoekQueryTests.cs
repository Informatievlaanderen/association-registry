namespace AssociationRegistry.Test.Admin.Api.Queries.BeheerVerenigingZoekQuery;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Nest;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using Doelgroep = AssociationRegistry.Admin.Schema.Search.VerenigingZoekDocument.Types.Doelgroep;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[Category(Categories.RefactorAfterVZERMigration)]
[IntegrationTest]
public class BeheerVerenigingenZoekQueryTests
{
    private readonly ITestOutputHelper _helper;
    private readonly IElasticClient _elasticClient;
    private readonly TypeMapping _typeMapping;

    public BeheerVerenigingenZoekQueryTests(EventsInDbScenariosFixture fixture, ITestOutputHelper helper)
    {
        _helper = helper;
        _elasticClient = fixture.ElasticClient;
        _typeMapping = fixture.ServiceProvider.GetRequiredService<TypeMapping>();
    }

    [Fact]
    public async ValueTask? Given_More_Than_ElasticSearch_Context_Limit_Total_Count_Is_Actual_Number()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var totalCount = 0;
        var desiredCount = 10000;
        var batchCount = 500;
        var docs = new List<VerenigingZoekDocument>();

        do
        {

            for (var i = 0; i < batchCount; i++)
                docs.Add(new()
                {
                    VCode = fixture.Create<VCode>(),
                    Naam = fixture.Create<string>(),
                    KorteNaam = fixture.Create<string>(),
                    Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
                    {
                        Code = Verenigingstype.FeitelijkeVereniging.Code,
                        Naam = Verenigingstype.FeitelijkeVereniging.Naam,
                    },
                    HoofdactiviteitenVerenigingsloket = [],
                    Locaties = [],
                    Lidmaatschappen = [],
                    Werkingsgebieden = [],
                    Sleutels = [],
                    Doelgroep = new Doelgroep
                    {
                        JsonLdMetadata = new JsonLdMetadata(),
                        Minimumleeftijd = 18,
                        Maximumleeftijd = 99
                    }
                });

            await _elasticClient.BulkAsync(b => b.IndexMany(docs));
            totalCount += batchCount;
        } while (totalCount < desiredCount);

        await _elasticClient.Indices.RefreshAsync();
        var query = new BeheerVerenigingenZoekQuery(_elasticClient, _typeMapping);

        var actual = await query.ExecuteAsync(new BeheerVerenigingenZoekFilter("*", "vCode", new PaginationQueryParams()),
                                              CancellationToken.None);

        actual.Total.Should().BeGreaterThan(desiredCount);
    }
}
