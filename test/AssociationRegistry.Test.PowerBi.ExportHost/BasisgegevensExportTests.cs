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
using System.Net;
using System.Text;
using Xunit;

public class BasisgegevensExportTests
{
    private Stream _resultStream;
    private readonly Fixture _fixture;
    private readonly Mock<IAmazonS3> _s3ClientMock;

    public BasisgegevensExportTests()
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
            "bron,doelgroep.maximumleeftijd,doelgroep.minimumleeftijd,einddatum,isUitgeschrevenUitPubliekeDatastroom,korteBeschrijving,korteNaam,naam,roepnaam,startdatum,status,vCode,verenigingstype.code,verenigingstype.naam,kboNummer,corresponderendeVCodes,aantalVertegenwoordigers,datumLaatsteAanpassing,verenigingssubtype.code,verenigingssubtype.naam,subverenigingVan.andereVereniging,subverenigingVan.identificatie,subverenigingVan.beschrijving\r\n");

        foreach (var doc in docs)
        {
            stringBuilder.Append(
                $"{doc.Bron},{doc.Doelgroep.Maximumleeftijd},{doc.Doelgroep.Minimumleeftijd},{doc.Einddatum},{doc.IsUitgeschrevenUitPubliekeDatastroom},{doc.KorteBeschrijving},{doc.KorteNaam},{doc.Naam},{doc.Roepnaam},{doc.Startdatum},{doc.Status},{doc.VCode},{doc.Verenigingstype.Code},{doc.Verenigingstype.Naam},{doc.KboNummer},\"{string.Join(separator: ", ", doc.CorresponderendeVCodes)}\",{doc.AantalVertegenwoordigers},{doc.DatumLaatsteAanpassing},{doc.Verenigingssubtype.Code},{doc.Verenigingssubtype.Naam},{doc.SubverenigingVan.AndereVereniging},{doc.SubverenigingVan.Identificatie},{doc.SubverenigingVan.Beschrijving}\r\n");
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
        var exporter = new Exporter(WellKnownFileNames.Basisgegevens,
                                    bucketName: "something",
                                    new BasisgegevensRecordWriter(),
                                    _s3ClientMock.Object,
                                    new NullLogger<Exporter>());

        await exporter.Export(docs);
    }
}
