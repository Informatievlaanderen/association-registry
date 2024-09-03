namespace AssociationRegistry.Test.PowerBi.ExportHost;

using Admin.Schema.PowerBiExport;
using AssociationRegistry.PowerBi.ExportHost;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using System.Text;
using Xunit;

public class basisgegevensExportTests
{
    [Fact]
    public async Task WithMultipleDocuments_ThenCsvExportShouldExport()
    {

        var fixture = new Fixture().CustomizeDomain();

        var docs = fixture.CreateMany<PowerBiExportDocument>();

        var content = await GenerateCsv(docs);
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("bron,doelgroep.maximumleeftijd,doelgroep.minimumleeftijd,einddatum,isUitgeschrevenUitPubliekeDatastroom,korteBeschrijving,korteNaam,naam,roepnaam,startdatum,status,vCode,verenigingstype.code,verenigingstype.naam,kboNummer,corresponderendeVCodes,aantalVertegenwoordigers,datumLaatsteAanpassing\r\n");

        foreach (var doc in docs)
        {
            stringBuilder.Append(
                $"{doc.Bron},{doc.Doelgroep.Maximumleeftijd},{doc.Doelgroep.Minimumleeftijd},{doc.Einddatum},{doc.IsUitgeschrevenUitPubliekeDatastroom},{doc.KorteBeschrijving},{doc.KorteNaam},{doc.Naam},{doc.Roepnaam},{doc.Startdatum},{doc.Status},{doc.VCode},{doc.Verenigingstype.Code},{doc.Verenigingstype.Naam},{doc.KboNummer},\"{string.Join(", ", doc.CorresponderendeVCodes)}\",{doc.AantalVertegenwoordigers},{doc.DatumLaatsteAanpassing}\r\n");
        }
        content.Should().BeEquivalentTo(stringBuilder.ToString());
    }

    private static async Task<string> GenerateCsv(IEnumerable<PowerBiExportDocument> docs)
    {
        var exporter = new PowerBiDocumentExporter();

        var exportStream = await exporter.ExportBasisgegevens(docs);

        using var reader = new StreamReader(exportStream, Encoding.UTF8);

        var content = await reader.ReadToEndAsync();

        return content;
    }
}


