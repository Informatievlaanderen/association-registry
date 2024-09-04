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

public class HoofdactiviteitenRecordWriterTests
{
    private Stream _resultStream = null;

    private readonly Fixture _fixture;
    private readonly Mock<IAmazonS3> _s3ClientMock;

    public HoofdactiviteitenRecordWriterTests()
    {
       _fixture = new Fixture().CustomizeDomain();
       _s3ClientMock = SetupS3Client();
    }

    [Fact]
    public async Task WithNoDocuments_ThenCsvExportsOnlyHeaders()
    {
        var docs = Array.Empty<PowerBiExportDocument>();

        await Export(docs);

        var actualResult = await GetActualResult();
        actualResult.Should().BeEquivalentTo($"code,naam,vcode\r\n");
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
        stringBuilder.Append("code,naam,vcode\r\n");

        foreach (var doc in docs)
        {
            foreach (var hoofdactiviteitVerenigingsloket in doc.HoofdactiviteitenVerenigingsloket)
            {
                stringBuilder.Append(
                    $"{hoofdactiviteitVerenigingsloket.Code},{hoofdactiviteitVerenigingsloket.Naam},{doc.VCode}\r\n");
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

    private async Task Export(IEnumerable<PowerBiExportDocument> docs)
    {
        var exporter = new Exporter(WellKnownFileNames.Hoofdactiviteiten,
                                    "something",
                                    new HoofdactiviteitenRecordWriter(),
                                    _s3ClientMock.Object,
                                    new NullLogger<Exporter>());

        await exporter.Export(docs);
    }
}

// namespace AssociationRegistry.Test.PowerBi.ExportHost;
//
// using Admin.Schema.PowerBiExport;
// using AssociationRegistry.PowerBi.ExportHost;
// using AssociationRegistry.PowerBi.ExportHost.Writers;
// using FluentAssertions;
// using System.Text;
// using Xunit;
//
// public class HoofdactiviteitenRecordWriterTests
// {
//     [Fact]
//     public async Task WithNoDocuments_ThenCsvExportsOnlyHeaders()
//     {
//         var docs = Array.Empty<PowerBiExportDocument>();
//
//         var content = await GenerateCsv(docs);
//         content.Should().BeEquivalentTo($"code,naam,vcode\r\n");
//     }
//
//     [Fact]
//     public async Task WithOneDocument_ThenCsvExportShouldExport()
//     {
//         var docs = new List<PowerBiExportDocument>
//         {
//             new()
//             {
//                 VCode = "V0001001",
//                 HoofdactiviteitenVerenigingsloket = [
//                     new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd", Code = "activiteit"},
//                 ]
//             }
//         };
//
//         var content = await GenerateCsv(docs);
//         content.Should().BeEquivalentTo($"code,naam,vcode\r\nactiviteit,hoofd,V0001001\r\n");
//     }
//
//     [Fact]
//     public async Task WithMultipleDocuments_ThenCsvExportShouldExport()
//     {
//         var docs = new List<PowerBiExportDocument>
//         {
//             new()
//             {
//                 VCode = "V0001001",
//                 HoofdactiviteitenVerenigingsloket = [
//                     new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd", Code = "activiteit"},
//                 ]
//             },
//             new()
//             {
//                 VCode = "V0001002",
//                 HoofdactiviteitenVerenigingsloket = [
//                     new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd1", Code = "activiteit1"},
//                     new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd2", Code = "activiteit2"},
//                     new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd3", Code = "activiteit3"},
//                 ]
//             }
//         };
//
//         var content = await GenerateCsv(docs);
//         content.Should().BeEquivalentTo($"code,naam,vcode\r\nactiviteit,hoofd,V0001001\r\nactiviteit1,hoofd1,V0001002\r\nactiviteit2,hoofd2,V0001002\r\nactiviteit3,hoofd3,V0001002\r\n");
//     }
//
//     private static async Task<string> GenerateCsv(IEnumerable<PowerBiExportDocument> docs)
//     {
//         var exporter = new PowerBiDocumentExporter();
//
//         var exportStream = await exporter.Export(docs, new HoofdactiviteitenRecordWriter());
//
//         using var reader = new StreamReader(exportStream, Encoding.UTF8);
//
//         var content = await reader.ReadToEndAsync();
//
//         return content;
//     }
// }
//
//
