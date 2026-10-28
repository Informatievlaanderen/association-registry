namespace AssociationRegistry.Test.Scheduled.Host.DubbelBeheer;

using AssociationRegistry.Scheduled.Host.DubbelBeheer;
using AssociationRegistry.Scheduled.Host.DubbelBeheer.Schema;
using Common.Framework;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;

[Collection(nameof(DubbelImportCollection))]
public class DubbelImportIngestorTests : IAsyncLifetime
{
    private const string Csv = "DubbeleVCode;AuthentiekeVCode\nV0001001;V0001002\nV0001003;V0001004\n";

    private DocumentStore _store = null!;
    private InMemoryS3 _s3 = null!;
    private DubbelImportIngestor _ingestor = null!;

    public async ValueTask InitializeAsync()
    {
        _store = await TestDocumentStoreFactory.CreateAsync(
            nameof(DubbelImportIngestorTests),
            options => options.RegisterDocumentType<DubbelImportDocument>()
        );

        _s3 = new InMemoryS3();

        _ingestor = new DubbelImportIngestor(
            _s3.Client,
            new DubbelImportOptions { SourceBucket = "source-bucket" },
            _store,
            NullLogger<DubbelImportIngestor>.Instance
        );
    }

    public async ValueTask DisposeAsync() => await _store.DisposeAsync();

    private async Task<IReadOnlyList<DubbelImportDocument>> TeVerwerkenRijen()
    {
        await using var session = _store.QuerySession();

        return await session.Query<DubbelImportDocument>().OrderBy(x => x.Volgnummer).ToListAsync();
    }

    [Fact]
    public async Task Elke_rij_van_de_csv_wordt_apart_bewaard_en_het_bestand_daarna_verwijderd()
    {
        _s3.VoegBestandToe("dubbels.csv", Csv);
        _s3.VoegBestandToe("readme.txt", "geen csv");

        await _ingestor.Ingest(CancellationToken.None);

        (await TeVerwerkenRijen())
            .Should()
            .BeEquivalentTo(
                [
                    new
                    {
                        SourceKey = "dubbels.csv",
                        Volgnummer = 1,
                        DubbeleVCode = "V0001001",
                        AuthentiekeVCode = "V0001002",
                    },
                    new
                    {
                        SourceKey = "dubbels.csv",
                        Volgnummer = 2,
                        DubbeleVCode = "V0001003",
                        AuthentiekeVCode = "V0001004",
                    },
                ],
                options => options.WithStrictOrdering()
            );

        _s3.Objecten.Keys.Should().Equal("readme.txt");
    }

    [Fact]
    public async Task Een_al_ingelezen_bestand_wordt_niet_opnieuw_bewaard_maar_wel_verwijderd()
    {
        _s3.VoegBestandToe("dubbels.csv", Csv);
        await _ingestor.Ingest(CancellationToken.None);

        // Het verwijderen faalde de vorige keer, het bestand staat er dus nog.
        _s3.VoegBestandToe("dubbels.csv", Csv);
        await _ingestor.Ingest(CancellationToken.None);

        (await TeVerwerkenRijen()).Should().HaveCount(2);
        _s3.Objecten.Should().BeEmpty();
    }

    [Fact]
    public async Task Meer_dan_een_csv_faalt()
    {
        _s3.VoegBestandToe("eerste.csv", Csv);
        _s3.VoegBestandToe("tweede.csv", Csv);

        var act = () => _ingestor.Ingest(CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        (await TeVerwerkenRijen()).Should().BeEmpty();
        _s3.Objecten.Should().HaveCount(2);
    }

    [Fact]
    public async Task Een_ongeldige_csv_wordt_niet_bewaard_en_blijft_staan()
    {
        _s3.VoegBestandToe("gelijk.csv", "DubbeleVCode;AuthentiekeVCode\nV0001001;V0001001\n");

        var act = () => _ingestor.Ingest(CancellationToken.None);

        (await act.Should().ThrowAsync<Exception>()).Which.GetBaseException().Message.Should().Contain("zijn gelijk");
        (await TeVerwerkenRijen()).Should().BeEmpty();
        _s3.Objecten.Should().ContainKey("gelijk.csv");
    }
}

[CollectionDefinition(nameof(DubbelImportCollection), DisableParallelization = true)]
public class DubbelImportCollection;
