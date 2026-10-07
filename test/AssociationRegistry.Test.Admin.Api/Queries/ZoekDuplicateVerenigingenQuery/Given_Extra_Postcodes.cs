namespace AssociationRegistry.Test.Admin.Api.Queries.ZoekDuplicateVerenigingenQuery;

using AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using Common.AutoFixture;
using Elastic.Clients.Elasticsearch;
using FluentAssertions;
using Framework.Fakes;
using Framework.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Vereniging;
using Xunit;

public class Given_Extra_PostcodesFixture : ElasticRepositoryFixture
{
    public Given_Extra_PostcodesFixture()
        : base(nameof(Given_Extra_PostcodesFixture)) { }
}

public class Given_Extra_Postcodes : IClassFixture<Given_Extra_PostcodesFixture>
{
    // Deliberately a gemeente that never matches the indexed documents,
    // so that a hit can only come from the postcode filter.
    private const string NietMatchendeGemeente = "Nergensdorp";

    private readonly Given_Extra_PostcodesFixture _fixture;
    private readonly Fixture _autoFixture;

    public Given_Extra_Postcodes(Given_Extra_PostcodesFixture fixture)
    {
        _fixture = fixture;
        _autoFixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async ValueTask With_Extra_Postcode_Matching_Document_Then_Duplicate_Is_Found()
    {
        var document = await IndexDocument(postcodes: "1501");

        var actual = await Execute(document.Naam, "1500", extraPostcodes: ["1500", "1501", "1502"]);

        actual.Select(x => x.VCode).Should().Contain(document.VCode);
    }

    [Fact]
    public async ValueTask Without_Extra_Postcodes_Then_Duplicate_In_Neighbouring_Postcode_Is_Not_Found()
    {
        var document = await IndexDocument(postcodes: "1501");

        var actual = await Execute(document.Naam, "1500", extraPostcodes: null);

        actual.Select(x => x.VCode).Should().NotContain(document.VCode);
    }

    [Fact]
    public async ValueTask With_Extra_Postcodes_Not_Matching_Document_Then_Duplicate_Is_Not_Found()
    {
        var document = await IndexDocument(postcodes: "9000");

        var actual = await Execute(document.Naam, "1500", extraPostcodes: ["1500", "1501", "1502"]);

        actual.Select(x => x.VCode).Should().NotContain(document.VCode);
    }

    [Fact]
    public async ValueTask With_Own_Postcode_Then_Duplicate_Is_Still_Found()
    {
        var document = await IndexDocument(postcodes: "1500");

        var actual = await Execute(document.Naam, "1500", extraPostcodes: ["1500", "1501", "1502"]);

        actual.Select(x => x.VCode).Should().Contain(document.VCode);
    }

    [Fact]
    public async ValueTask With_Duplicate_Postcodes_In_Extra_Postcodes_Then_Duplicate_Is_Found_Once()
    {
        var document = await IndexDocument(postcodes: "1502");

        var actual = await Execute(document.Naam, "1500", extraPostcodes: ["1502", "1502", "1500", "1500"]);

        actual.Count(x => x.VCode == document.VCode).Should().Be(1);
    }

    [Fact]
    public async ValueTask With_Document_On_Multiple_Postcodes_Then_Any_Matching_Extra_Postcode_Finds_It()
    {
        var document = await IndexDocument(postcodes: ["9000", "1502"]);

        var actual = await Execute(document.Naam, "1500", extraPostcodes: ["1500", "1502"]);

        actual.Select(x => x.VCode).Should().Contain(document.VCode);
    }

    [Fact]
    public async ValueTask With_Extra_Postcode_But_Different_Naam_Then_Duplicate_Is_Not_Found()
    {
        var document = await IndexDocument(postcodes: "1501");

        var actual = await Execute("Totaal Andere Zaken Qwxzy", "1500", extraPostcodes: ["1500", "1501"]);

        actual.Select(x => x.VCode).Should().NotContain(document.VCode);
    }

    [Fact]
    public async ValueTask With_Extra_Postcode_And_Document_Is_Dubbel_Then_Duplicate_Is_Not_Found()
    {
        var document = await IndexDocument(postcodes: "1501", configure: d => d.IsDubbel = true);

        var actual = await Execute(document.Naam, "1500", extraPostcodes: ["1500", "1501"]);

        actual.Select(x => x.VCode).Should().NotContain(document.VCode);
    }

    [Fact]
    public async ValueTask With_Extra_Postcode_And_Document_Is_Gestopt_Then_Duplicate_Is_Not_Found()
    {
        var document = await IndexDocument(postcodes: "1501", configure: d => d.IsGestopt = true);

        var actual = await Execute(document.Naam, "1500", extraPostcodes: ["1500", "1501"]);

        actual.Select(x => x.VCode).Should().NotContain(document.VCode);
    }

    [Fact]
    public async ValueTask With_Extra_Postcode_And_Document_Is_Verwijderd_Then_Duplicate_Is_Not_Found()
    {
        var document = await IndexDocument(postcodes: "1501", configure: d => d.IsVerwijderd = true);

        var actual = await Execute(document.Naam, "1500", extraPostcodes: ["1500", "1501"]);

        actual.Select(x => x.VCode).Should().NotContain(document.VCode);
    }

    [Fact]
    public async ValueTask With_Empty_Extra_Postcodes_Then_Only_Own_Postcode_Is_Used()
    {
        var document = await IndexDocument(postcodes: "1501");

        var actual = await Execute(document.Naam, "1500", extraPostcodes: []);

        actual.Select(x => x.VCode).Should().NotContain(document.VCode);
    }

    private Task<DuplicateDetectionDocument> IndexDocument(
        string postcodes,
        Action<DuplicateDetectionDocument>? configure = null
    ) => IndexDocument([postcodes], configure);

    private async Task<DuplicateDetectionDocument> IndexDocument(
        string[] postcodes,
        Action<DuplicateDetectionDocument>? configure = null
    )
    {
        var document = _autoFixture.Create<DuplicateDetectionDocument>();
        document.IsDubbel = false;
        document.IsGestopt = false;
        document.IsVerwijderd = false;
        document.Locaties = postcodes
            .Select(p =>
                _autoFixture.Create<DuplicateDetectionDocument.Locatie>() with
                {
                    Postcode = p,
                    Gemeente = "Nietbestaandegemeente",
                }
            )
            .ToArray();

        configure?.Invoke(document);

        await _fixture.ElasticClient.IndexAsync(document);
        await _fixture.ElasticClient.Indices.RefreshAsync(Indices.All);

        return document;
    }

    private async Task<IReadOnlyCollection<DuplicaatVereniging>> Execute(
        string naam,
        string eigenPostcode,
        string[]? extraPostcodes
    )
    {
        var locatie = _autoFixture.Create<Locatie>() with
        {
            Adres = _autoFixture.Create<Adres>() with
            {
                Gemeente = Gemeentenaam.Hydrate(NietMatchendeGemeente),
                Postcode = eigenPostcode,
            },
        };

        var locaties = new DuplicateVerenigingZoekQueryLocaties([locatie]);

        var query = new ZoekDuplicateVerenigingenQuery(
            _fixture.ElasticClient,
            _fixture.ElasticSearchOptions,
            new MinimumScore(0),
            new FakePostcodesInSameGemeenteService(extraPostcodes ?? []),
            NullLogger<ZoekDuplicateVerenigingenQuery>.Instance
        );

        return await query.ExecuteAsync(
            VerenigingsNaam.Create(naam),
            locaties,
            minimumScoreOverride: new MinimumScore(0)
        );
    }
}
