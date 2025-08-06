namespace AssociationRegistry.Test.Admin.Api.Queries.ZoekDuplicateVerenigingenQuery;

using AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;
using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Framework.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Elastic.Clients.Elasticsearch;
using Vereniging;
using Xunit;

public class Given_Gestopte_VerenigingenFixture : ElasticRepositoryFixture
{
    public Given_Gestopte_VerenigingenFixture() : base(nameof(Given_Gestopte_VerenigingenFixture))
    {

    }
}

public class Given_Gestopte_Verenigingen : IClassFixture<Given_Gestopte_VerenigingenFixture>, IDisposable, IAsyncDisposable
{
    public string Query { get; }
    private readonly Given_Gestopte_VerenigingenFixture _fixture;
    private readonly ITestOutputHelper _helper;
    private readonly ElasticsearchClient? _elasticClient;
    private readonly Fixture _autoFixture;
    private readonly ZoekDuplicateVerenigingenQuery _query;

    public Given_Gestopte_Verenigingen(Given_Gestopte_VerenigingenFixture fixture, ITestOutputHelper helper)
    {
        _fixture = fixture;
        _helper = helper;
        _elasticClient = fixture.ElasticClient;
        _autoFixture = new Fixture().CustomizeAdminApi();

        _query = new ZoekDuplicateVerenigingenQuery(fixture.ElasticClient, fixture.ElasticSearchOptions, new MinimumScore(0), NullLogger<ZoekDuplicateVerenigingenQuery>.Instance);
    }

    [Fact]
    public async ValueTask Then_Query_Returns_Empty()
    {
        var document = await IndexDocument();

        var actual = await ExecuteQuery(document);

        actual.Should().BeEmpty();
    }

    private async Task<DuplicateDetectionDocument> IndexDocument()
    {
        var duplicateDetectionDoc = _autoFixture.Create<DuplicateDetectionDocument>();
        duplicateDetectionDoc.IsGestopt = true;

        await _elasticClient!.IndexAsync(duplicateDetectionDoc);

        await _elasticClient.Indices.RefreshAsync(Indices.All);

        return duplicateDetectionDoc;
    }

    private async ValueTask<IReadOnlyCollection<DuplicaatVereniging>> ExecuteQuery(DuplicateDetectionDocument document)
    {
        var locaties = document.Locaties.Select(x => _autoFixture.Create<Locatie>() with{
            Adres = _autoFixture.Create<Adres>() with
            {
                Gemeente = Gemeentenaam.Hydrate(x.Gemeente),
                Postcode = x.Postcode,
            }
        }).ToArray();

        return await _query.ExecuteAsync(VerenigingsNaam.Create(document.Naam), locaties, minimumScoreOverride: new MinimumScore(0));
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }
}
