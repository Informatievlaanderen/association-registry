namespace AssociationRegistry.Test.Admin.Api.Queries.BeheerVerenigingZoekQuery;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.RequestModels;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using Xunit;
using VerenigingStatus = AssociationRegistry.Admin.Schema.Constants.VerenigingStatus;

[Collection(nameof(AdminApiCollection))]
[Category(Categories.RefactorAfterVZERMigration)]
public class When_Searching_On_Type
{
    private readonly ITestOutputHelper _helper;
    private BeheerVerenigingenZoekQuery _query;
    private VerenigingZoekDocument? _feitelijkeVereniging;
    private VerenigingZoekDocument? _vzer;
    private ElasticsearchClient _elasticClient;
    private TypeMapping _typeMapping;
    private readonly ElasticSearchOptionsSection _elasticSearchOptions;

    public When_Searching_On_Type(EventsInDbScenariosFixture fixture, ITestOutputHelper helper)
    {
        _helper = helper;
        var elasticClient = fixture.ElasticClient;
        _typeMapping = fixture.ServiceProvider.GetRequiredService<TypeMapping>();
        _elasticSearchOptions = fixture.ServiceProvider.GetRequiredService<IConfiguration>().GetElasticSearchOptionsSection();
        var autoFixture = new Fixture().CustomizeAdminApi();

        _feitelijkeVereniging = autoFixture.Create<VerenigingZoekDocument>();
        _feitelijkeVereniging.Naam = "de kleine vereniging";
        _feitelijkeVereniging.Status = VerenigingStatus.Actief;
        _feitelijkeVereniging.IsDubbel = false;
        _feitelijkeVereniging.IsVerwijderd = false;
        _feitelijkeVereniging.IsUitgeschrevenUitPubliekeDatastroom = false;
        _feitelijkeVereniging.Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
        {
            Code = Verenigingstype.FeitelijkeVereniging.Code,
            Naam = Verenigingstype.FeitelijkeVereniging.Naam,
        };
        _vzer = autoFixture.Create<VerenigingZoekDocument>();
        _vzer.Naam = "de kleine vereniging";
        _vzer.Status = VerenigingStatus.Actief;
        _vzer.IsDubbel = false;
        _vzer.IsVerwijderd = false;
        _vzer.Startdatum = "2025-06-06";
        _vzer.IsUitgeschrevenUitPubliekeDatastroom = false;
        _vzer.Verenigingstype = new VerenigingZoekDocument.Types.VerenigingsType
        {
            Code = Verenigingstype.VZER.Code,
            Naam = Verenigingstype.VZER.Naam,
        };

        _elasticClient = elasticClient;
    }

    [Theory]
    [InlineData("verenigingstype.code:FV")]
    [InlineData("verenigingstype.code:fv")]
    [InlineData("verenigingstype.code: fv ")]
    [InlineData("verenigingstype.code: fv AND verenigingstype.code: vzer")]
    [InlineData("verenigingstype.code: fv AND naam:de kleine vereniging AND verenigingstype.code: vzer")]
    [InlineData("naam:de kleine vereniging AND verenigingstype.code: fv AND verenigingstype.code: vzer")]
    [InlineData("verenigingstype.code:VZER")]
    [InlineData("verenigingstype.code:vzer")]
    [InlineData("verenigingstype.code: vzer ")]
    [InlineData("verenigingstype.code: vzer AND verenigingstype.code: fv")]
    [InlineData("verenigingstype.code: vzer AND naam:de kleine vereniging AND verenigingstype.code: fv")]
    [InlineData("naam:de kleine vereniging AND verenigingstype.code: vzer AND verenigingstype.code: fv")]
    public async ValueTask With_FV_In_Query_Returns_FV_And_VZER(string query)
    {
        var indexFeitelijke = await _elasticClient.IndexAsync<VerenigingZoekDocument>(_feitelijkeVereniging);
        var indexVzer = await _elasticClient.IndexAsync<VerenigingZoekDocument>(_vzer);

        indexFeitelijke.ShouldBeValidIndexResponse();
        indexVzer.ShouldBeValidIndexResponse();

        await _elasticClient.Indices.RefreshAsync(Indices.All);
        _query = new BeheerVerenigingenZoekQuery(_elasticClient, _typeMapping, _elasticSearchOptions);

        var searchResponse = await _query.ExecuteAsync(
            new BeheerVerenigingenZoekFilter(query: $"(vCode:{_feitelijkeVereniging.VCode} OR vCode:{_vzer.VCode}) AND {query}",
                                             sort: "vCode",
                                             paginationQueryParams: new PaginationQueryParams()),
            CancellationToken.None);

        var actualFV = searchResponse.Documents.SingleOrDefault(x => x.VCode == _feitelijkeVereniging.VCode);
        actualFV.Should().BeEquivalentTo(_feitelijkeVereniging);

        var actualVZER = searchResponse.Documents.SingleOrDefault(x => x.VCode == _vzer.VCode);
        actualVZER.Should().BeEquivalentTo(_vzer);
    }
}
