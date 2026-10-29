namespace AssociationRegistry.Test.Scheduled.Host.DubbelBeheer;

using System.Text;
using AssociationRegistry.Scheduled.Host.DubbelBeheer;
using AssociationRegistry.Scheduled.Host.DubbelBeheer.Schema;
using AssociationRegistry.Scheduled.Host.Infrastructure.MartenSetup;
using Common.Framework;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;

[Collection(nameof(DubbelImportCollection))]
public class S3DubbelImportReportGeneratorTests : IAsyncLifetime
{
    private const string ReportBucket = "report-bucket";
    private const string SourceKey = "dubbels.csv";
    private const string ReportKey = "dubbels-rapport.csv";

    private DocumentStore _store = null!;
    private InMemoryS3 _s3 = null!;
    private S3DubbelImportReportGenerator _generator = null!;
    private DubbelImportBatchDocument _batch = null!;

    public async ValueTask InitializeAsync()
    {
        _store = await TestDocumentStoreFactory.CreateAsync(
            nameof(S3DubbelImportReportGeneratorTests),
            options => options.RegisterDubbelImportDocumentTypes()
        );

        _s3 = new InMemoryS3();

        _generator = new S3DubbelImportReportGenerator(
            _s3.Client,
            new DubbelImportOptions { SourceBucket = "source-bucket", ReportBucket = ReportBucket },
            _store,
            NullLogger<S3DubbelImportReportGenerator>.Instance
        );

        _batch = DubbelImportBatchDocument.Create(SourceKey, Guid.NewGuid(), DateTimeOffset.UtcNow);
    }

    public async ValueTask DisposeAsync() => await _store.DisposeAsync();

    private async Task Store(params DubbelImportDocument[] lines)
    {
        await using var session = _store.LightweightSession();
        session.Store(lines);
        await session.SaveChangesAsync();
    }

    private DubbelImportDocument Line(
        int lineNumber,
        string dubbele,
        string authentieke,
        DubbelImportLineStatus status,
        string errorMessage = "",
        Guid? batchId = null
    ) =>
        new()
        {
            Id = Guid.NewGuid(),
            BatchId = batchId ?? _batch.Id,
            SourceKey = SourceKey,
            LineNumber = lineNumber,
            DubbeleVCode = dubbele,
            AuthentiekeVCode = authentieke,
            Status = status,
            ErrorMessage = errorMessage,
        };

    private string Report()
    {
        var content = _s3.Uploads[(ReportBucket, ReportKey)].Content;

        return new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetString(content);
    }

    [Fact]
    public async Task Schrijft_per_rij_het_resultaat_in_de_volgorde_van_het_bestand()
    {
        await Store(
            Line(2, "V0005003", "V0005004", DubbelImportLineStatus.Failed, "Deze vereniging is dubbel."),
            Line(1, "V0005001", "V0005002", DubbelImportLineStatus.Processed)
        );

        await _generator.Generate(_batch, CancellationToken.None);

        Report()
            .Should()
            .Be(
                "﻿Regel;DubbeleVCode;AuthentiekeVCode;Status;Reden\r\n"
                    + "1;V0005001;V0005002;Verwerkt;\r\n"
                    + "2;V0005003;V0005004;Gefaald;Deze vereniging is dubbel.\r\n"
            );

        _s3.Uploads[(ReportBucket, ReportKey)].ContentType.Should().Be("text/csv");
    }

    [Fact]
    public async Task Neemt_enkel_de_rijen_van_de_batch_op()
    {
        await Store(
            Line(1, "V0005001", "V0005002", DubbelImportLineStatus.Processed),
            Line(1, "V0005009", "V0005008", DubbelImportLineStatus.Processed, batchId: Guid.NewGuid())
        );

        await _generator.Generate(_batch, CancellationToken.None);

        Report().Should().Contain("V0005001").And.NotContain("V0005009");
    }

    [Fact]
    public async Task Opnieuw_genereren_overschrijft_hetzelfde_rapport()
    {
        await Store(Line(1, "V0005001", "V0005002", DubbelImportLineStatus.Processed));

        await _generator.Generate(_batch, CancellationToken.None);
        await _generator.Generate(_batch, CancellationToken.None);

        _s3.Uploads.Keys.Should().Equal((ReportBucket, ReportKey));
    }

    [Theory]
    [InlineData("dubbels.csv", "dubbels-rapport.csv")]
    [InlineData("DUBBELS.CSV", "DUBBELS-rapport.csv")]
    [InlineData("map/dubbels.csv", "map/dubbels-rapport.csv")]
    [InlineData("dubbels", "dubbels-rapport.csv")]
    public void De_rapport_key_volgt_de_naam_van_het_bestand(string sourceKey, string reportKey)
    {
        S3DubbelImportReportGenerator.ReportKeyFor(sourceKey).Should().Be(reportKey);
    }
}
