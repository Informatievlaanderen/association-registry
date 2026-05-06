namespace AssociationRegistry.Test.Scheduled.Host.PowerBi;

using System.Net;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using AssociationRegistry.Admin.Schema.PowerBiExport;
using AssociationRegistry.Scheduled.Host.PowerBi;
using AssociationRegistry.Scheduled.Host.PowerBi.Writers;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class WerkingsgebiedenRecordWriterTests
{
    private Stream _resultStream;
    private readonly Fixture _fixture;
    private readonly Mock<IAmazonS3> _s3ClientMock;

    public WerkingsgebiedenRecordWriterTests()
    {
        _fixture = new Fixture().CustomizeDomain();
        _s3ClientMock = SetupS3Client();
    }

    [Fact]
    public async ValueTask WithNoDocuments_ThenCsvExportsOnlyHeaders()
    {
        var docs = Array.Empty<PowerBiExportDocument>();

        await Export(docs);

        var actualResult = await GetActualResult();
        actualResult.Should().BeEquivalentTo("code,naam,vcode\r\n");
    }

    [Fact]
    public async ValueTask WithMultipleDocuments_ThenCsvExportShouldExport()
    {
        var docs = _fixture.CreateMany<PowerBiExportDocument>();

        await Export(docs);

        var actualResult = await GetActualResult();
        var expectedResult = GetExpectedResult(docs);

        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    private async Task<string> GetActualResult()
    {
        using var reader = new StreamReader(_resultStream, Encoding.UTF8);

        var content = await reader.ReadToEndAsync();

        return content;
    }

    private static string GetExpectedResult(IEnumerable<PowerBiExportDocument> docs)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("code,naam,vcode\r\n");

        foreach (var doc in docs)
        {
            foreach (var werkgebied in doc.Werkingsgebieden)
            {
                stringBuilder.Append(
                    $"{werkgebied.Code},{werkgebied.Naam},{doc.VCode}\r\n");
            }
        }

        return stringBuilder.ToString();
    }

    private Mock<IAmazonS3> SetupS3Client()
    {
        var s3ClientMock = new Mock<IAmazonS3>();

        s3ClientMock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                    .Callback<PutObjectRequest, CancellationToken>((request, _) => _resultStream = request.InputStream)
                    .ReturnsAsync(new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK });

        return s3ClientMock;
    }

    private async Task Export(IEnumerable<PowerBiExportDocument> docs)
    {
        var exporter = new Exporter<PowerBiExportDocument>(WellKnownFileNames.Werkingsgebieden,
                                                           bucketName: "something",
                                                           new WerkingsgebiedenRecordWriter(),
                                                           _s3ClientMock.Object,
                                                           new NullLogger<Exporter<PowerBiExportDocument>>());

        await exporter.Export(docs);
    }
}
