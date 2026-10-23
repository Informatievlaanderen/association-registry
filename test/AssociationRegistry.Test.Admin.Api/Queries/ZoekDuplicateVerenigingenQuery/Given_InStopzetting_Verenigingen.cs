namespace AssociationRegistry.Test.Admin.Api.Queries.ZoekDuplicateVerenigingenQuery;

using AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;
using AutoFixture;
using Common.AutoFixture;
using Elastic.Clients.Elasticsearch;
using FluentAssertions;
using Framework.Fakes;
using Framework.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class Given_InStopzetting_VerenigingenFixture : ElasticRepositoryFixture
{
    public Given_InStopzetting_VerenigingenFixture()
        : base(nameof(Given_InStopzetting_VerenigingenFixture)) { }
}

public class Given_InStopzetting_Verenigingen : IClassFixture<Given_InStopzetting_VerenigingenFixture>
{
    private readonly Given_InStopzetting_VerenigingenFixture _fixture;
    private readonly Fixture _autoFixture;
    private readonly ZoekDuplicateVerenigingenQuery _query;

    public Given_InStopzetting_Verenigingen(Given_InStopzetting_VerenigingenFixture fixture)
    {
        _fixture = fixture;
        _autoFixture = new Fixture().CustomizeAdminApi();

        _query = new ZoekDuplicateVerenigingenQuery(
            fixture.ElasticClient,
            fixture.ElasticSearchOptions,
            new MinimumScore(0),
            new FakePostcodesInSameGemeenteService(),
            NullLogger<ZoekDuplicateVerenigingenQuery>.Instance
        );
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async ValueTask Then_InStopzetting_Is_Mapped_To_The_Duplicate(bool inStopzetting)
    {
        var document = _autoFixture.Create<DuplicateDetectionDocument>();
        document.InStopzetting = inStopzetting;

        await _fixture.ElasticClient!.IndexAsync(document);
        await _fixture.ElasticClient.Indices.RefreshAsync(Indices.All);

        var actual = await ExecuteQuery(document);

        actual.Single(x => x.VCode == document.VCode).InStopzetting.Should().Be(inStopzetting);
    }

    private async ValueTask<IReadOnlyCollection<DuplicaatVereniging>> ExecuteQuery(DuplicateDetectionDocument document)
    {
        var locaties = document
            .Locaties.Select(x =>
                _autoFixture.Create<Locatie>() with
                {
                    Adres = _autoFixture.Create<Adres>() with
                    {
                        Gemeente = Gemeentenaam.Hydrate(x.Gemeente),
                        Postcode = x.Postcode,
                    },
                }
            )
            .ToArray();

        return await _query.ExecuteAsync(
            VerenigingsNaam.Create(document.Naam),
            locaties,
            minimumScoreOverride: new MinimumScore(0)
        );
    }
}
