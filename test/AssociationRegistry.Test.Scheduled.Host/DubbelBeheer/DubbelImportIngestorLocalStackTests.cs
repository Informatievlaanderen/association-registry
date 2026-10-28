namespace AssociationRegistry.Test.Scheduled.Host.DubbelBeheer;

using System.Text;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using AssociationRegistry.Scheduled.Host.DubbelBeheer;
using AssociationRegistry.Scheduled.Host.DubbelBeheer.Schema;
using Common.Framework;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
///     Integratietest tegen een echte, lokaal draaiende LocalStack-compatibele S3 (het "ministack" docker-compose
///     service op poort 4566, hetzelfde endpoint dat Scheduled.Host zelf gebruikt via DubbelImportOptions.UseLocalStack).
///     Start dit lokaal met: docker compose up -d ministack ministack-init
///     Als die container niet bereikbaar is, wordt de test overgeslagen in plaats van te falen.
/// </summary>
[Collection(nameof(DubbelImportCollection))]
public class DubbelImportIngestorLocalStackTests : IAsyncLifetime
{
    private const string Bucket = "verenigingsregister-dubbelbeheer";

    private DocumentStore _store = null!;
    private DubbelImportIngestor _ingestor = null!;
    private AmazonS3Client _s3 = null!;
    private string _key = null!;

    public async ValueTask InitializeAsync()
    {
        _s3 = new AmazonS3Client(
            new BasicAWSCredentials(accessKey: "test", secretKey: "test"),
            new AmazonS3Config
            {
                ServiceURL = "http://127.0.0.1:4566",
                ForcePathStyle = true,
                UseHttp = true,
            }
        );

        _store = await TestDocumentStoreFactory.CreateAsync(
            nameof(DubbelImportIngestorLocalStackTests),
            options => options.RegisterDocumentType<DubbelImportDocument>()
        );

        _ingestor = new DubbelImportIngestor(
            _s3,
            new DubbelImportOptions { SourceBucket = Bucket },
            _store,
            NullLogger<DubbelImportIngestor>.Instance
        );

        _key = $"localstack-test-{Guid.NewGuid()}.csv";
    }

    public async ValueTask DisposeAsync()
    {
        _s3.Dispose();
        await _store.DisposeAsync();
    }

    [Fact]
    public async Task Leest_een_via_aws_cli_geuploade_csv_in_van_de_lokale_s3_emulator()
    {
        await _s3.PutObjectAsync(
            new PutObjectRequest
            {
                BucketName = Bucket,
                Key = _key,
                InputStream = new MemoryStream(
                    Encoding.UTF8.GetBytes("DubbeleVCode;AuthentiekeVCode\nV1001001;V1001002\nV1001003;V1001004\n")
                ),
            }
        );

        await _ingestor.Ingest(CancellationToken.None);

        await using var session = _store.QuerySession();
        var rijen = await session
            .Query<DubbelImportDocument>()
            .Where(x => x.SourceKey == _key)
            .OrderBy(x => x.Volgnummer)
            .ToListAsync();
        rijen.Select(x => x.DubbeleVCode).Should().Equal("V1001001", "V1001003");

        var overgebleven = await _s3.ListObjectsV2Async(
            new ListObjectsV2Request { BucketName = Bucket, Prefix = _key }
        );
        (overgebleven.S3Objects ?? []).Should().BeEmpty();
    }
}
