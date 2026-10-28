namespace AssociationRegistry.Scheduled.Host.DubbelBeheer;

using Amazon.S3;
using Amazon.S3.Model;
using JasperFx;
using Marten;
using Marten.Exceptions;
using Microsoft.Extensions.Logging;
using Parsing;
using Schema;

/// <summary>
///     Verwerkt het CSV-bestand met dubbele verenigingen. Een run doet één van twee dingen:
///     <list type="bullet">
///         <item>Er is een onafgewerkte batch: de lease overnemen en verdergaan met de open rijen.</item>
///         <item>Er is geen onafgewerkte batch: het bestand uit S3 samen met de batch in één transactie inlezen.</item>
///     </list>
///     Daarna worden de open rijen in volgorde verwerkt en wordt het rapport gestuurd.
///     Een run werkt enkel zolang hij de lease op de batch heeft. Elke bewaarde rij verlengt de lease;
///     na het rapport wordt de lease vrijgegeven. Kan of mag de run niet verder, dan stopt hij zonder te falen.
/// </summary>
public class DubbelImportIngestor(
    IAmazonS3 s3,
    DubbelImportOptions options,
    IDocumentStore store,
    IDubbelImportLineProcessor lineProcessor,
    IDubbelImportReportGenerator reportGenerator,
    TimeProvider timeProvider,
    ILogger<DubbelImportIngestor> logger
)
{
    public async Task Ingest(CancellationToken cancellationToken)
    {
        var runId = Guid.NewGuid();

        try
        {
            var batch =
                await ContinueUnfinishedBatch(runId, cancellationToken)
                ?? await DownloadFileFromS3AndSaveLinesInDb(runId, cancellationToken);

            if (batch is null)
            {
                logger.LogInformation("Geen onafgewerkte batch en geen bestand in S3, er is niets te doen");
                return;
            }

            if (batch.State == DubbelImportBatchState.Pending)
                await ProcessLines(batch, runId, cancellationToken);

            if (batch.State == DubbelImportBatchState.Finished)
                await SendReport(batch, cancellationToken);

            logger.LogInformation("Bestand {SourceKey} werd volledig verwerkt", batch.S3Key);
        }
        catch (RunStoppedException e)
        {
            logger.LogWarning("Run gestopt: {Reason}", e.Message);
        }
    }

    /// <summary>
    ///     Neemt een batch over die een vorige run niet afwerkte. Heeft een andere run nog de lease,
    ///     dan stopt deze run, zodat er nooit een tweede bestand ingelezen wordt terwijl een batch bezig is.
    /// </summary>
    private async Task<DubbelImportBatchDocument?> ContinueUnfinishedBatch(
        Guid runId,
        CancellationToken cancellationToken
    )
    {
        var batch = await LoadUnfinishedBatch(cancellationToken);

        if (batch is null)
            return null;

        if (batch.IsLeasedByOtherRun(runId, timeProvider.GetUtcNow()))
            throw new RunStoppedException(
                $"run {batch.LockedBy} heeft de lease op {batch.S3Key} tot {batch.LeaseUntil}"
            );

        await SaveAndExtendLease(batch, runId, cancellationToken);

        logger.LogInformation("Onafgewerkte batch voor {SourceKey} wordt verdergezet", batch.S3Key);

        return batch;
    }

    /// <summary>
    ///     Leest het bestand uit S3 en bewaart de batch en alle rijen in één transactie.
    ///     De batch wordt aangemaakt met deze run als houder van de lease.
    /// </summary>
    private async Task<DubbelImportBatchDocument?> DownloadFileFromS3AndSaveLinesInDb(
        Guid runId,
        CancellationToken cancellationToken
    )
    {
        var s3Key = await FindCsvKey(cancellationToken);

        if (s3Key is null)
            return null;

        if ((await LoadBatch(s3Key, cancellationToken))?.State == DubbelImportBatchState.ReportSent)
            throw new RunStoppedException(
                $"{s3Key} werd al volledig verwerkt; verwijder het bestand of upload het onder een andere naam"
            );

        using var response = await s3.GetObjectAsync(options.SourceBucket, s3Key, cancellationToken);
        var records = DubbelCsvParser.Parse(response.ResponseStream);
        var now = timeProvider.GetUtcNow();
        var batch = DubbelImportBatchDocument.Create(s3Key, runId, now);

        await using var session = store.LightweightSession();
        session.Insert(batch);
        session.Insert(DubbelImportDocument.Create(batch.Id, s3Key, records, now).ToArray());

        try
        {
            await session.SaveChangesAsync(cancellationToken);
        }
        catch (DocumentAlreadyExistsException)
        {
            throw new RunStoppedException($"{s3Key} wordt al door een andere run ingelezen");
        }

        logger.LogInformation("Bestand {SourceKey} ingelezen met {Count} rijen", s3Key, records.Count);

        return batch;
    }

    private async Task ProcessLines(DubbelImportBatchDocument batch, Guid runId, CancellationToken cancellationToken)
    {
        await s3.DeleteObjectAsync(options.SourceBucket, batch.S3Key, cancellationToken);

        foreach (var line in await LoadOpenLines(batch, cancellationToken))
        {
            await lineProcessor.Process(line, cancellationToken);
            line.Status = DubbelImportLineStatus.Processed;

            await SaveAndExtendLease(batch, runId, cancellationToken, line);
        }

        batch.State = DubbelImportBatchState.Finished;
        await SaveAndExtendLease(batch, runId, cancellationToken);
    }

    /// <summary>
    ///     Stuurt het rapport en geeft de lease vrij. De laatste save blijft versiegecontroleerd:
    ///     heeft een andere run de batch intussen overgenomen, dan faalt hij.
    /// </summary>
    private async Task SendReport(DubbelImportBatchDocument batch, CancellationToken cancellationToken)
    {
        await reportGenerator.Generate(batch, cancellationToken);

        batch.State = DubbelImportBatchState.ReportSent;
        batch.ReleaseLease();
        await Save(batch, cancellationToken);
    }

    private Task SaveAndExtendLease(
        DubbelImportBatchDocument batch,
        Guid runId,
        CancellationToken cancellationToken,
        DubbelImportDocument? line = null
    )
    {
        batch.ExtendLease(runId, timeProvider.GetUtcNow());

        return Save(batch, cancellationToken, line);
    }

    /// <summary>
    ///     Bewaart de batch, en eventueel een verwerkte rij, in één SaveChanges.
    ///     Faalt de versiecontrole op de batch, dan heeft een andere run de lease overgenomen.
    /// </summary>
    private async Task Save(
        DubbelImportBatchDocument batch,
        CancellationToken cancellationToken,
        DubbelImportDocument? line = null
    )
    {
        await using var session = store.LightweightSession();
        session.Update(batch);

        if (line is not null)
            session.Update(line);

        try
        {
            await session.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            throw new RunStoppedException($"een andere run heeft de lease op {batch.S3Key} overgenomen");
        }
    }

    private async Task<DubbelImportBatchDocument?> LoadBatch(string s3Key, CancellationToken cancellationToken)
    {
        await using var session = store.QuerySession();

        return await session.LoadAsync<DubbelImportBatchDocument>(
            DubbelImportBatchDocument.IdFor(s3Key),
            cancellationToken
        );
    }

    private async Task<DubbelImportBatchDocument?> LoadUnfinishedBatch(CancellationToken cancellationToken)
    {
        await using var session = store.QuerySession();

        return await session
            .Query<DubbelImportBatchDocument>()
            .Where(x => x.State != DubbelImportBatchState.ReportSent)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<DubbelImportDocument>> LoadOpenLines(
        DubbelImportBatchDocument batch,
        CancellationToken cancellationToken
    )
    {
        await using var session = store.QuerySession();

        return await session
            .Query<DubbelImportDocument>()
            .Where(x => x.BatchId == batch.Id && x.Status == DubbelImportLineStatus.Open)
            .OrderBy(x => x.LineNumber)
            .ToListAsync(cancellationToken);
    }

    private async Task<string?> FindCsvKey(CancellationToken cancellationToken)
    {
        var response = await s3.ListObjectsV2Async(
            new ListObjectsV2Request { BucketName = options.SourceBucket },
            cancellationToken
        );

        return (response.S3Objects ?? [])
            .SingleOrDefault(x => x.Key.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            ?.Key;
    }

    /// <summary>
    ///     De run kan of mag niet verder (een andere run heeft de batch, of het bestand werd al verwerkt)
    ///     en stopt zonder te falen.
    /// </summary>
    private sealed class RunStoppedException(string reason) : Exception(reason);
}
