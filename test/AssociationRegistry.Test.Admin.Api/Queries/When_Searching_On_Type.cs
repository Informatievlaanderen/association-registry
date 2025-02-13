namespace AssociationRegistry.Test.Admin.Api.Queries;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Admin.Schema.Search;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class When_Searching_On_Type
{
    private BeheerVerenigingenZoekQuery _query;
    private VerenigingZoekDocument? _feitelijkeVereniging;
    private VerenigingZoekDocument? _vzer;
    private IElasticClient _elasticClient;
    private TypeMapping _typeMapping;

    public When_Searching_On_Type(EventsInDbScenariosFixture fixture)
    {
        var elasticClient = fixture.ElasticClient;
        _typeMapping = fixture.ServiceProvider.GetRequiredService<TypeMapping>();

        var autoFixture = new Fixture().CustomizeAdminApi();

        _feitelijkeVereniging = autoFixture.Create<VerenigingZoekDocument>();
        _feitelijkeVereniging.Naam = "de kleine vereniging";
        _feitelijkeVereniging.Verenigingstype = new VerenigingZoekDocument.VerenigingsType
        {
            Code = Verenigingstype.FeitelijkeVereniging.Code,
            Naam = Verenigingstype.FeitelijkeVereniging.Naam,
        };
        _vzer = autoFixture.Create<VerenigingZoekDocument>();
        _vzer.Naam = "de kleine vereniging";
        _vzer.Verenigingstype = new VerenigingZoekDocument.VerenigingsType
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
        var indexFeitelijke = await _elasticClient.IndexDocumentAsync<VerenigingZoekDocument>(_feitelijkeVereniging);
        var indexVzer = await _elasticClient.IndexDocumentAsync<VerenigingZoekDocument>(_vzer);

        indexFeitelijke.ShouldBeValidIndexResponse();
        indexVzer.ShouldBeValidIndexResponse();

        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);
        _query = new BeheerVerenigingenZoekQuery(_elasticClient, _typeMapping);

        var searchResponse = await _query.ExecuteAsync(
            new BeheerVerenigingenZoekFilter(query: query,
                                             sort: "vCode",
                                             paginationQueryParams: new PaginationQueryParams()),
            CancellationToken.None);

        var actualFV = searchResponse.Documents.SingleOrDefault(x => x.VCode == _feitelijkeVereniging.VCode);
        actualFV.Should().BeEquivalentTo(_feitelijkeVereniging);

        var actualVZER = searchResponse.Documents.SingleOrDefault(x => x.VCode == _vzer.VCode);
        actualVZER.Should().BeEquivalentTo(_vzer);
    }
}
