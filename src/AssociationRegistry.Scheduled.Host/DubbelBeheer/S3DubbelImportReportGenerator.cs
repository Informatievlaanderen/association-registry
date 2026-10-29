namespace AssociationRegistry.Scheduled.Host.DubbelBeheer;

using System.Globalization;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using CsvHelper;
using CsvHelper.Configuration;
using Marten;
using Microsoft.Extensions.Logging;
using Schema;

/// <summary>
///     Schrijft per rij van de batch het resultaat als CSV naar de report bucket, in de volgorde van het bestand.
///     Het formaat volgt het ingelezen bestand (puntkomma, UTF-8 met BOM), zodat het rechtstreeks in Excel opent.
///     Faalt het wegschrijven, dan blijft de batch afgewerkt maar zonder rapport, en probeert een volgende run opnieuw.
/// </summary>
public class S3DubbelImportReportGenerator(
    IAmazonS3 s3,
    DubbelImportOptions options,
    IDocumentStore store,
    ILogger<S3DubbelImportReportGenerator> logger
) : IDubbelImportReportGenerator
{
    private static readonly CsvConfiguration Configuration = new(CultureInfo.InvariantCulture) { Delimiter = ";" };

    public async Task Generate(DubbelImportBatchDocument batch, CancellationToken cancellationToken)
    {
        var lines = await LoadLines(batch, cancellationToken);
        var reportKey = ReportKeyFor(batch.S3Key);

        await s3.PutObjectAsync(
            new PutObjectRequest
            {
                BucketName = options.ReportBucket,
                Key = reportKey,
                InputStream = WriteCsv(lines),
                ContentType = "text/csv",
            },
            cancellationToken
        );

        logger.LogInformation(
            "Rapport voor {SourceKey} werd geschreven naar {ReportBucket}/{ReportKey}: {Processed} verwerkt, {Failed} gefaald",
            batch.S3Key,
            options.ReportBucket,
            reportKey,
            lines.Count(x => x.Status == DubbelImportLineStatus.Processed),
            lines.Count(x => x.Status == DubbelImportLineStatus.Failed)
        );
    }

    /// <summary>Bv. <c>dubbels.csv</c> wordt <c>dubbels-rapport.csv</c>.</summary>
    public static string ReportKeyFor(string s3Key)
    {
        var withoutExtension = s3Key.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) ? s3Key[..^4] : s3Key;

        return $"{withoutExtension}-rapport.csv";
    }

    private async Task<IReadOnlyList<DubbelImportDocument>> LoadLines(
        DubbelImportBatchDocument batch,
        CancellationToken cancellationToken
    )
    {
        await using var session = store.QuerySession();

        return await session
            .Query<DubbelImportDocument>()
            .Where(x => x.BatchId == batch.Id)
            .OrderBy(x => x.LineNumber)
            .ToListAsync(cancellationToken);
    }

    private static MemoryStream WriteCsv(IEnumerable<DubbelImportDocument> lines)
    {
        var stream = new MemoryStream();

        using (
            var writer = new StreamWriter(
                stream,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true),
                -1,
                leaveOpen: true
            )
        )
        using (var csv = new CsvWriter(writer, Configuration))
        {
            csv.WriteRecords(lines.Select(ReportRow.From));
        }

        stream.Position = 0;

        return stream;
    }

    private record ReportRow(int Regel, string DubbeleVCode, string AuthentiekeVCode, string Status, string Reden)
    {
        public static ReportRow From(DubbelImportDocument line) =>
            new(line.LineNumber, line.DubbeleVCode, line.AuthentiekeVCode, StatusFor(line.Status), line.ErrorMessage);

        private static string StatusFor(DubbelImportLineStatus status) =>
            status switch
            {
                DubbelImportLineStatus.Processed => "Verwerkt",
                DubbelImportLineStatus.Failed => "Gefaald",
                _ => "Niet verwerkt",
            };
    }
}
