namespace AssociationRegistry.Test.Public.Api.When_Searching;

using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.RequestModels;
using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using Microsoft.Extensions.DependencyInjection;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Configuration;
using Vereniging;

using Xunit;

[Collection(nameof(PublicApiCollection))]
public class When_Searching_On_Type
{
    private PubliekVerenigingenZoekQuery _query;
    private VerenigingZoekDocument? _feitelijkeVereniging;
    private VerenigingZoekDocument? _vzer;
    private ElasticsearchClient _elasticClient;
    private TypeMapping _typeMapping;
    private readonly ElasticSearchOptionsSection _elasticClientOptions;

    public When_Searching_On_Type(GivenEventsFixture fixture)
    {
        var elasticClient = fixture.ElasticClient;
        _typeMapping = fixture.ServiceProvider.GetRequiredService<TypeMapping>();
        _elasticClientOptions = fixture.ServiceProvider.GetRequiredService<IConfiguration>().GetElasticSearchOptionsSection();
        var autoFixture = new Fixture().CustomizePublicApi();

        _feitelijkeVereniging = autoFixture.Create<VerenigingZoekDocument>();

        _feitelijkeVereniging.Naam = "de kleine vereniging";
        _feitelijkeVereniging.Verenigingstype = new VerenigingZoekDocument.Types.Verenigingstype
        {
            Code = Verenigingstype.FeitelijkeVereniging.Code,
            Naam = Verenigingstype.FeitelijkeVereniging.Naam,
        };
        _vzer = autoFixture.Create<VerenigingZoekDocument>();

        _vzer.Naam = "de kleine vereniging";

        _vzer.Verenigingstype = new VerenigingZoekDocument.Types.Verenigingstype
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
    public async Task With_FV_In_Query_Returns_FV_And_VZER(string query)
    {
        var indexFeitelijke = await _elasticClient.IndexAsync<VerenigingZoekDocument>(_feitelijkeVereniging);
        var indexVzer = await _elasticClient.IndexAsync<VerenigingZoekDocument>(_vzer);

        indexFeitelijke.ShouldBeValidIndexResponse();
        indexVzer.ShouldBeValidIndexResponse();

        await _elasticClient.Indices.RefreshAsync(Indices.All);
        _query = new PubliekVerenigingenZoekQuery(_elasticClient, _typeMapping, _elasticClientOptions);

        var searchResponse = await _query.ExecuteAsync(
            new PubliekVerenigingenZoekFilter(query: query,
                                             sort: "vCode",
                                             hoofdactiviteiten:[],
                                             paginationQueryParams: new PaginationQueryParams()),
            CancellationToken.None);

        var actualFV = searchResponse.Documents.SingleOrDefault(x => x.VCode == _feitelijkeVereniging.VCode);
        actualFV.Should().BeEquivalentTo(_feitelijkeVereniging);

        var actualVZER = searchResponse.Documents.SingleOrDefault(x => x.VCode == _vzer.VCode);
        actualVZER.Should().BeEquivalentTo(_vzer);
    }
}
