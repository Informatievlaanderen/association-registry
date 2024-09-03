namespace AssociationRegistry.Test.PowerBi.ExportHost;

using Admin.Schema.PowerBiExport;
using AssociationRegistry.PowerBi.ExportHost;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using System.Text;
using Xunit;

public class LocatiesExportTests
{
    [Fact]
    public async Task WithMultipleDocuments_ThenCsvExportShouldExport()
    {

        var fixture = new Fixture().CustomizeDomain();

        var docs = fixture.CreateMany<PowerBiExportDocument>();

        var content = await GenerateCsv(docs);
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("adresId.broncode,adresId.bronwaarde,adresvoorstelling,bron,busnummer,gemeente,huisnummer,isPrimair,land,locatieId,locatieType,naam,postcode,straatnaam,vCode\r\n");

        foreach (var doc in docs)
        {
            foreach (var locatie in doc.Locaties)
            {
                stringBuilder.Append(
                    $"{locatie.AdresId?.Broncode},{locatie.AdresId?.Bronwaarde},{locatie.Adresvoorstelling},{locatie.Bron},{locatie.Adres?.Busnummer},{locatie.Adres?.Gemeente},{locatie.Adres?.Huisnummer},{locatie.IsPrimair},{locatie.Adres?.Land},{locatie.LocatieId},{locatie.Locatietype},{locatie.Naam},{locatie.Adres?.Postcode},{locatie.Adres?.Straatnaam},{doc.VCode}\r\n");
            }
        }
        content.Should().BeEquivalentTo(stringBuilder.ToString());
    }

    private static async Task<string> GenerateCsv(IEnumerable<PowerBiExportDocument> docs)
    {
        var exporter = new PowerBiDocumentExporter();

        var exportStream = await exporter.ExportLocaties(docs);

        using var reader = new StreamReader(exportStream, Encoding.UTF8);

        var content = await reader.ReadToEndAsync();

        return content;
    }
}


