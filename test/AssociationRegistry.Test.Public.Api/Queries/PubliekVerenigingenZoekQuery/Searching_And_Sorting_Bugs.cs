namespace AssociationRegistry.Test.Public.Api.Queries.PubliekVerenigingenZoekQuery;

using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Public.Schema.Search;
using AutoFixture;
using FluentAssertions;
using Framework;
using Nest;
using Vereniging;
using When_Saving_A_Document_To_Elastic;
using Xunit;

public class Searching_And_Sorting_BugsFixture : ElasticRepositoryFixture
{
    public VerenigingZoekDocument[] Documents { get; private set; }
    public Fixture Fixture { get; private set; }

    public Searching_And_Sorting_BugsFixture() : base(nameof(Searching_And_Sorting_BugsFixture))
    {
        Fixture = new Fixture().CustomizePublicApi();
    }

    protected override async Task InsertDocuments()
    {
        Documents = CreateSorteerVerenigingDocuments(
            adresvoorstelling: "Sorteerstraat 1, 1079 SorteerGemeente Laboriosam quam quia ipsam recusandae eveniet architecto tempora nihil.2025-06-11T06:31:11.595Z, Sorterië",
            verenigingData: new[]
            {
                ("V0002001", "Naam 1 Laboriosam quam quia ipsam recusandae eveniet architecto tempora nihil.2025-06-11T06:31:11.595Z", "kort A", 12, 120),
                ("V0002002", "naam 2 Laboriosam quam quia ipsam recusandae eveniet architecto tempora nihil.2025-06-11T06:31:11.595Z", "Kort B", 2, 50),
                ("V0002003", "Naam 3 Laboriosam quam quia ipsam recusandae eveniet architecto tempora nihil.2025-06-11T06:31:11.595Z", "Kort A", 12, 50),
            });

        foreach (var document in Documents)
        {
            await IndexDocument(document);
        }
    }

    private VerenigingZoekDocument[] CreateSorteerVerenigingDocuments(
        string adresvoorstelling,
        (string VCode, string Naam, string KorteNaam, int MinLeeftijd, int MaxLeeftijd)[] verenigingData)
    {
        return verenigingData.Select(data => new VerenigingZoekDocument
        {
            VCode = data.VCode,
            Naam = data.Naam,
            KorteNaam = data.KorteNaam,
            Doelgroep = new VerenigingZoekDocument.Types.Doelgroep
            {
                Minimumleeftijd = data.MinLeeftijd,
                Maximumleeftijd = data.MaxLeeftijd
            },
            Locaties = new[]
            {
                new VerenigingZoekDocument.Types.Locatie
                {
                    Locatietype = "Activiteiten",
                    Adresvoorstelling = adresvoorstelling,
                    Gemeente = "SorteerGemeente Laboriosam quam quia ipsam recusandae eveniet architecto tempora nihil.2025-06-11T06:31:11.595Z",
                    Postcode = "1079",
                },
            },
            IsVerwijderd = false,
            IsDubbel = false,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            Status = VerenigingStatus.Actief.StatusNaam,
        }).ToArray();
    }
}

public class Searching_And_Sorting_Bugs : IClassFixture<Searching_And_Sorting_BugsFixture>
{
    private readonly IElasticClient _elasticClient;
    private readonly ITypeMapping _typeMapping;
    private readonly VerenigingZoekDocument[] _documents;

    public Searching_And_Sorting_Bugs(Searching_And_Sorting_BugsFixture fixture, ITestOutputHelper helper)
    {
        _elasticClient = fixture.ElasticClient;
        _typeMapping = fixture.TypeMapping;
        _documents = fixture.Documents;
    }

    [Theory]
    [InlineData("V0002001")]
    [InlineData("locaties.gemeente:SorteerGemeente")]
    //[InlineData("locaties.gemeente:SorteerGemeente Est corporis necessitatibus dolorem nam possimus omnis quas quae quo")]
    [InlineData("locaties.gemeente:SorteerGemeente Est corporis necessitatibus dolorem nam possimus omnis quas quae quo.2025-06-11T06:31:35.892Z")]
    [InlineData("locaties.gemeente:SorteerGemeente Laboriosam quam quia ipsam recusandae eveniet architecto tempora nihil.2025-06-11T06:31:11.595Z")]
    public async Task Then_vereniging_can_be_retrieved_by_exact_name(string query)
    {
        var response = await ExecuteSearch(query);
        response.Documents.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("1976-01-25")]
    public async Task Then_vereniging_can_be_retrieved_by_Date(string query)
    {
        var response = await ExecuteSearch(query);
        response.Documents.Should().NotBeEmpty();
    }

    private async Task<ISearchResponse<VerenigingZoekDocument>> ExecuteSearch(string query, string? sort = null)
    {
        var zoekQuery = new PubliekVerenigingenZoekQuery(_elasticClient, _typeMapping);

        return await zoekQuery.ExecuteAsync(
            new(query, sort, [], new PaginationQueryParams()),
            CancellationToken.None);
    }
}
