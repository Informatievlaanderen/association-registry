namespace AssociationRegistry.Test.PowerBi.ExportHost;

using Admin.Schema.PowerBiExport;
using Amazon.S3;
using Amazon.S3.Model;
using AssociationRegistry.PowerBi.ExportHost;
using AssociationRegistry.PowerBi.ExportHost.Writers;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text;
using Xunit;

public class ContactgegevensExportTests
{
    private Stream _resultStream = null;

    [Fact]
    public async Task WithMultipleDocuments_ThenCsvExportShouldExport()
    {
        var fixture = new Fixture().CustomizeDomain();
        var s3ClientMock = SetupS3Client();
        var docs = fixture.CreateMany<PowerBiExportDocument>();

        await Export(docs, s3ClientMock.Object);

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
        stringBuilder.Append("beschrijving,bron,contactgegevenId,contactgegevenType,isPrimair,vCode,waarde\r\n");

        foreach (var doc in docs)
        {
            foreach (var contactgegeven in doc.Contactgegevens)
            {
                stringBuilder.Append($"{contactgegeven.Beschrijving},{contactgegeven.Bron},{contactgegeven.ContactgegevenId},{contactgegeven.Contactgegeventype},{contactgegeven.IsPrimair},{doc.VCode},{contactgegeven.Waarde}\r\n");
            }
        }

        return stringBuilder.ToString();
    }

    private Mock<IAmazonS3> SetupS3Client()
    {
        var s3ClientMock = new Mock<IAmazonS3>();

        s3ClientMock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                    .Callback<PutObjectRequest, CancellationToken>((request, _) => _resultStream = request.InputStream)
                    .ReturnsAsync(new PutObjectResponse { HttpStatusCode = System.Net.HttpStatusCode.OK });

        return s3ClientMock;
    }

    private static async Task Export(IEnumerable<PowerBiExportDocument> docs, IAmazonS3 s3Client)
    {
        var exporter = new Exporter(WellKnownFileNames.Contactgegevens,
                                    "something",
                                    new ContactgegevensRecordWriter(),
                                    s3Client,
                                    new NullLogger<Exporter>());

        await exporter.Export(docs);
    }
}
