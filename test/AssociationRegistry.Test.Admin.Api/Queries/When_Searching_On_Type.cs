namespace AssociationRegistry.Test.Admin.Api.Queries;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Admin.Schema.Search;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
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
        _feitelijkeVereniging.Verenigingstype = new VerenigingZoekDocument.VerenigingsType
        {
            Code = Verenigingstype.FeitelijkeVereniging.Code,
            Naam = Verenigingstype.FeitelijkeVereniging.Naam,
        };
        _vzer = autoFixture.Create<VerenigingZoekDocument>();
        _vzer.Verenigingstype = new VerenigingZoekDocument.VerenigingsType
        {
            Code = Verenigingstype.VZER.Code,
            Naam = Verenigingstype.VZER.Naam,
        };

        _elasticClient = elasticClient;

    }

    [Fact]
    public async Task? With_VZER_In_Query_Returns_FV_And_VZER()
    {
        var actual = await _query.ExecuteAsync(new BeheerVerenigingenZoekFilter("q=type:VZER", "vCode", new PaginationQueryParams()),
                                              CancellationToken.None);

        actual.Documents.Should().Contain(_feitelijkeVereniging);
        actual.Documents.Should().Contain(_vzer);
    }

    [Fact]
    public async Task With_FV_In_Query_Returns_FV_And_VZER()
    {
        var k = await _elasticClient.BulkAsync(b => b.IndexMany([_feitelijkeVereniging]));
        k.IsValid.Should().BeTrue();
        await _elasticClient.Indices.RefreshAsync();
        _query = new BeheerVerenigingenZoekQuery(_elasticClient, _typeMapping);

        //verenigingstype.code:fv AND
        var q = $"vCode:{_feitelijkeVereniging.VCode}";

        var actual = await _query.ExecuteAsync(new BeheerVerenigingenZoekFilter($"vCode:{_feitelijkeVereniging.VCode}", "vCode", new PaginationQueryParams()),
                                              CancellationToken.None);

        actual.Documents.Should().Contain(_feitelijkeVereniging);
        //actual.Documents.Should().Contain(_vzer);
    }
}
