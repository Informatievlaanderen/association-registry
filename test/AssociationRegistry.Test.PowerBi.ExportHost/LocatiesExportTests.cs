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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class LocatiesExportTests
{
    private Stream _resultStream;
    private readonly Fixture _fixture;
    private readonly Mock<IAmazonS3> _s3ClientMock;

    public LocatiesExportTests()
    {
        _fixture = new Fixture().CustomizeDomain();
        _s3ClientMock = SetupS3Client();
    }

    [Fact]
    public async Task WithMultipleDocuments_ThenCsvExportShouldExport()
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

        stringBuilder.Append(
            "adresId.broncode,adresId.bronwaarde,adresvoorstelling,bron,busnummer,gemeente,huisnummer,isPrimair,land,locatieId,locatieType,naam,postcode,straatnaam,vCode\r\n");

        foreach (var doc in docs)
        {
            foreach (var locatie in doc.Locaties)
            {
                stringBuilder.Append(
                    $"{locatie.AdresId?.Broncode},{locatie.AdresId?.Bronwaarde},{locatie.Adresvoorstelling},{locatie.Bron},{locatie.Adres?.Busnummer},{locatie.Adres?.Gemeente},{locatie.Adres?.Huisnummer},{locatie.IsPrimair},{locatie.Adres?.Land},{locatie.LocatieId},{locatie.Locatietype},{locatie.Naam},{locatie.Adres?.Postcode},{locatie.Adres?.Straatnaam},{doc.VCode}\r\n");
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
        var exporter = new Exporter(WellKnownFileNames.Locaties,
                                    bucketName: "something",
                                    new LocatiesRecordWriter(),
                                    _s3ClientMock.Object,
                                    new NullLogger<Exporter>());

        await exporter.Export(docs);
    }
}
