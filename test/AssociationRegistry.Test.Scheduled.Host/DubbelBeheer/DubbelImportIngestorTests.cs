namespace AssociationRegistry.Test.Scheduled.Host.DubbelBeheer;

using AssociationRegistry.Scheduled.Host.DubbelBeheer;
using AssociationRegistry.Scheduled.Host.DubbelBeheer.Schema;
using AssociationRegistry.Scheduled.Host.Infrastructure.MartenSetup;
using Common.Framework;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;

[Collection(nameof(DubbelImportCollection))]
public class DubbelImportIngestorTests : IAsyncLifetime
{
    private const string Key = "dubbels.csv";
    private const string Csv = "DubbeleVCode;AuthentiekeVCode\nV0001001;V0001002\nV0001003;V0001004\n";

    private DocumentStore _store = null!;
    private InMemoryS3 _s3 = null!;
    private FixedTimeProvider _time = null!;
    private RecordingLineProcessor _lineProcessor = null!;
    private RecordingReportGenerator _reportGenerator = null!;
    private DubbelImportIngestor _ingestor = null!;

    public async ValueTask InitializeAsync()
    {
        _store = await TestDocumentStoreFactory.CreateAsync(
            nameof(DubbelImportIngestorTests),
            options => options.RegisterDubbelImportDocumentTypes()
        );

        _s3 = new InMemoryS3();
        _time = new FixedTimeProvider();
        _lineProcessor = new RecordingLineProcessor();
        _reportGenerator = new RecordingReportGenerator();

        _ingestor = new DubbelImportIngestor(
            _s3.Client,
            new DubbelImportOptions { SourceBucket = "source-bucket" },
            _store,
            _lineProcessor,
            _reportGenerator,
            _time,
            NullLogger<DubbelImportIngestor>.Instance
        );
    }

    public async ValueTask DisposeAsync() => await _store.DisposeAsync();

    private async Task<IReadOnlyList<DubbelImportDocument>> Lines()
    {
        await using var session = _store.QuerySession();

        return await session.Query<DubbelImportDocument>().OrderBy(x => x.LineNumber).ToListAsync();
    }

    private async Task<DubbelImportBatchDocument?> Batch()
    {
        await using var session = _store.QuerySession();

        return await session.LoadAsync<DubbelImportBatchDocument>(DubbelImportBatchDocument.IdFor(Key));
    }

    private async Task SaveBatch(DubbelImportBatchDocument batch)
    {
        await using var session = _store.LightweightSession();
        session.Update(batch);
        await session.SaveChangesAsync();
    }

    /// <summary>Leest het bestand in en laat de run stoppen voor er een rij verwerkt wordt.</summary>
    private async Task<DubbelImportBatchDocument> StageWithoutProcessing()
    {
        _s3.AddFile(Key, Csv);
        _lineProcessor.OnProcess = _ => throw new OperationCanceledException();

        var act = () => _ingestor.Ingest(CancellationToken.None);
        await act.Should().ThrowAsync<OperationCanceledException>();

        _lineProcessor.OnProcess = null;
        _lineProcessor.LineNumbers.Clear();

        return (await Batch())!;
    }

    [Fact]
    public async Task Elke_rij_wordt_bewaard_verwerkt_en_gerapporteerd_en_het_bestand_verwijderd()
    {
        _s3.AddFile(Key, Csv);
        _s3.AddFile("readme.txt", "geen csv");

        await _ingestor.Ingest(CancellationToken.None);

        var batch = (await Batch())!;
        batch.State.Should().Be(DubbelImportBatchState.ReportSent);
        batch.LockedBy.Should().BeEmpty();
        batch.LeaseUntil.Should().Be(DateTimeOffset.MinValue);

        (await Lines())
            .Should()
            .BeEquivalentTo(
                [
                    new
                    {
                        BatchId = batch.Id,
                        SourceKey = Key,
                        LineNumber = 1,
                        DubbeleVCode = "V0001001",
                        AuthentiekeVCode = "V0001002",
                        Status = DubbelImportLineStatus.Processed,
                    },
                    new
                    {
                        BatchId = batch.Id,
                        SourceKey = Key,
                        LineNumber = 2,
                        DubbeleVCode = "V0001003",
                        AuthentiekeVCode = "V0001004",
                        Status = DubbelImportLineStatus.Processed,
                    },
                ],
                options => options.WithStrictOrdering()
            );

        _lineProcessor.LineNumbers.Should().Equal(1, 2);
        _reportGenerator.Count.Should().Be(1);
        _s3.Objects.Keys.Should().Equal("readme.txt");
    }

    [Fact]
    public async Task Een_onderbroken_batch_wordt_hervat_ook_als_het_bestand_al_verwijderd_werd()
    {
        await StageWithoutProcessing();
        _s3.Objects.Should().BeEmpty();

        _time.Now += DubbelImportBatchDocument.LeaseDuration;
        await _ingestor.Ingest(CancellationToken.None);

        _lineProcessor.LineNumbers.Should().Equal(1, 2);
        (await Batch())!.State.Should().Be(DubbelImportBatchState.ReportSent);
        (await Lines()).Should().HaveCount(2);
    }

    [Fact]
    public async Task Enkel_de_open_rijen_worden_hervat()
    {
        _s3.AddFile(Key, Csv);
        _lineProcessor.OnProcess = line =>
            line.LineNumber == 2 ? throw new OperationCanceledException() : Task.CompletedTask;

        var act = () => _ingestor.Ingest(CancellationToken.None);
        await act.Should().ThrowAsync<OperationCanceledException>();

        _lineProcessor.OnProcess = null;
        _lineProcessor.LineNumbers.Clear();
        _time.Now += DubbelImportBatchDocument.LeaseDuration;

        await _ingestor.Ingest(CancellationToken.None);

        _lineProcessor.LineNumbers.Should().Equal(2);
        (await Lines()).Should().OnlyContain(x => x.Status == DubbelImportLineStatus.Processed);
    }

    [Fact]
    public async Task Een_gefaalde_rij_wordt_met_de_reden_bewaard_en_de_volgende_rijen_worden_verwerkt()
    {
        _s3.AddFile(Key, Csv);
        _lineProcessor.Result = line =>
            line.LineNumber == 1 ? DubbelImportLineResult.Failed("geweigerd") : DubbelImportLineResult.Processed;

        await _ingestor.Ingest(CancellationToken.None);

        (await Lines())
            .Select(x => (x.LineNumber, x.Status, Reason: x.ErrorMessage))
            .Should()
            .Equal((1, DubbelImportLineStatus.Failed, "geweigerd"), (2, DubbelImportLineStatus.Processed, ""));
        (await Batch())!.State.Should().Be(DubbelImportBatchState.ReportSent);
    }

    [Fact]
    public async Task Een_batch_met_een_lopende_lease_van_een_andere_run_wordt_niet_verwerkt()
    {
        await StageWithoutProcessing();

        var otherRun = Guid.NewGuid();
        var batch = (await Batch())!;
        batch.ExtendLease(otherRun, _time.Now);
        await SaveBatch(batch);

        await _ingestor.Ingest(CancellationToken.None);

        _lineProcessor.LineNumbers.Should().BeEmpty();
        var afterRun = (await Batch())!;
        afterRun.State.Should().Be(DubbelImportBatchState.Pending);
        afterRun.LockedBy.Should().Be(otherRun);
    }

    [Fact]
    public async Task Een_vervallen_lease_van_een_andere_run_wordt_overgenomen()
    {
        var batch = await StageWithoutProcessing();
        batch.ExtendLease(Guid.NewGuid(), _time.Now);
        await SaveBatch(batch);

        _time.Now += DubbelImportBatchDocument.LeaseDuration;
        await _ingestor.Ingest(CancellationToken.None);

        _lineProcessor.LineNumbers.Should().Equal(1, 2);
        (await Batch())!.State.Should().Be(DubbelImportBatchState.ReportSent);
    }

    [Fact]
    public async Task Een_run_die_de_lease_verliest_stopt_zonder_de_rij_als_verwerkt_te_bewaren()
    {
        var otherRun = Guid.NewGuid();
        _s3.AddFile(Key, Csv);

        // Terwijl de eerste rij verwerkt wordt, neemt een andere run de lease over.
        _lineProcessor.OnProcess = async line =>
        {
            if (line.LineNumber != 1)
                return;

            var batch = (await Batch())!;
            batch.ExtendLease(otherRun, _time.Now);
            await SaveBatch(batch);
        };

        await _ingestor.Ingest(CancellationToken.None);

        _lineProcessor.LineNumbers.Should().Equal(1);
        (await Lines()).Should().OnlyContain(x => x.Status == DubbelImportLineStatus.Open);

        var afterRun = (await Batch())!;
        afterRun.State.Should().Be(DubbelImportBatchState.Pending);
        afterRun.LockedBy.Should().Be(otherRun);
    }

    [Fact]
    public async Task Een_finished_batch_genereert_enkel_nog_het_rapport()
    {
        var batch = await StageWithoutProcessing();
        batch.State = DubbelImportBatchState.Finished;
        batch.ReleaseLease();
        await SaveBatch(batch);

        await _ingestor.Ingest(CancellationToken.None);

        _lineProcessor.LineNumbers.Should().BeEmpty();
        _reportGenerator.Count.Should().Be(1);
        (await Batch())!.State.Should().Be(DubbelImportBatchState.ReportSent);
    }

    [Fact]
    public async Task Een_volledig_verwerkt_bestand_wordt_niet_opnieuw_verwerkt()
    {
        _s3.AddFile(Key, Csv);
        await _ingestor.Ingest(CancellationToken.None);

        _s3.AddFile(Key, Csv);
        await _ingestor.Ingest(CancellationToken.None);

        _lineProcessor.LineNumbers.Should().Equal(1, 2);
        _reportGenerator.Count.Should().Be(1);
        (await Lines()).Should().HaveCount(2);
    }

    [Fact]
    public async Task Een_gelijktijdige_run_die_hetzelfde_bestand_inleest_faalt_zonder_rijen_te_dupliceren()
    {
        _s3.AddFile(Key, Csv);

        // Een andere run bewaart de batch net voor deze run dat doet.
        _s3.OnGetObject = async _ =>
        {
            _s3.OnGetObject = null;

            await using var session = _store.LightweightSession();
            session.Insert(DubbelImportBatchDocument.Create(Key, Guid.NewGuid(), _time.Now));
            await session.SaveChangesAsync();
        };

        await _ingestor.Ingest(CancellationToken.None);

        (await Lines()).Should().BeEmpty();
        _lineProcessor.LineNumbers.Should().BeEmpty();
        _s3.Objects.Should().ContainKey(Key);
    }

    [Fact]
    public async Task Meer_dan_een_csv_faalt()
    {
        _s3.AddFile("eerste.csv", Csv);
        _s3.AddFile("tweede.csv", Csv);

        var act = () => _ingestor.Ingest(CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        (await Lines()).Should().BeEmpty();
        _s3.Objects.Should().HaveCount(2);
    }

    [Fact]
    public async Task Een_ongeldige_csv_wordt_niet_bewaard_en_blijft_staan()
    {
        _s3.AddFile("gelijk.csv", "DubbeleVCode;AuthentiekeVCode\nV0001001;V0001001\n");

        var act = () => _ingestor.Ingest(CancellationToken.None);

        (await act.Should().ThrowAsync<Exception>()).Which.GetBaseException().Message.Should().Contain("zijn gelijk");
        (await Lines()).Should().BeEmpty();
        _s3.Objects.Should().ContainKey("gelijk.csv");
    }
}

[CollectionDefinition(nameof(DubbelImportCollection), DisableParallelization = true)]
public class DubbelImportCollection;
