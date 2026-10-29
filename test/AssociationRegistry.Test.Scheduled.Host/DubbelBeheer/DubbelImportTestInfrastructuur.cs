namespace AssociationRegistry.Test.Scheduled.Host.DubbelBeheer;

using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using AssociationRegistry.Scheduled.Host.DubbelBeheer;
using AssociationRegistry.Scheduled.Host.DubbelBeheer.Schema;
using Moq;

/// <summary>Eenvoudige in-memory S3 op basis van Moq, enkel de operaties die de import gebruikt.</summary>
public class InMemoryS3
{
    public Dictionary<string, byte[]> Objects { get; } = new();

    /// <summary>Weggeschreven bestanden per bucket en key, apart van de bestanden die de import inleest.</summary>
    public Dictionary<(string Bucket, string Key), (byte[] Content, string ContentType)> Uploads { get; } = new();

    public IAmazonS3 Client { get; }

    /// <summary>Wordt uitgevoerd net voor een bestand opgehaald wordt.</summary>
    public Func<string, Task>? OnGetObject { get; set; }

    public InMemoryS3()
    {
        var mock = new Mock<IAmazonS3>();

        mock.Setup(x => x.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
                new ListObjectsV2Response
                {
                    S3Objects = Objects.Keys.Select(key => new S3Object { Key = key }).ToList(),
                }
            );

        mock.Setup(x => x.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(
                async (string _, string key, CancellationToken _) =>
                {
                    if (OnGetObject is not null)
                        await OnGetObject(key);

                    return new GetObjectResponse { Key = key, ResponseStream = new MemoryStream(Objects[key]) };
                }
            );

        mock.Setup(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (string _, string key, CancellationToken _) =>
                {
                    Objects.Remove(key);

                    return new DeleteObjectResponse();
                }
            );

        mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (PutObjectRequest request, CancellationToken _) =>
                {
                    using var content = new MemoryStream();
                    request.InputStream.CopyTo(content);
                    Uploads[(request.BucketName, request.Key)] = (content.ToArray(), request.ContentType);

                    return new PutObjectResponse();
                }
            );

        Client = mock.Object;
    }

    public void AddFile(string key, string content) => Objects[key] = Encoding.UTF8.GetBytes(content);
}

public class FixedTimeProvider : TimeProvider
{
    public DateTimeOffset Now { get; set; } =
        new(year: 2026, month: 10, day: 28, hour: 12, minute: 0, second: 0, TimeSpan.Zero);

    public override DateTimeOffset GetUtcNow() => Now;
}

public class RecordingLineProcessor : IDubbelImportLineProcessor
{
    public List<int> LineNumbers { get; } = [];

    /// <summary>Wordt uitgevoerd nadat een rij opgenomen werd.</summary>
    public Func<DubbelImportDocument, Task>? OnProcess { get; set; }

    public Func<DubbelImportDocument, DubbelImportLineResult> Result { get; set; } =
        _ => DubbelImportLineResult.Processed;

    public async Task<DubbelImportLineResult> Process(DubbelImportDocument line, CancellationToken cancellationToken)
    {
        LineNumbers.Add(line.LineNumber);

        if (OnProcess is not null)
            await OnProcess(line);

        return Result(line);
    }
}

public class RecordingReportGenerator : IDubbelImportReportGenerator
{
    public int Count { get; private set; }

    public Task Generate(DubbelImportBatchDocument batch, CancellationToken cancellationToken)
    {
        Count++;

        return Task.CompletedTask;
    }
}
