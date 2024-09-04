namespace AssociationRegistry.Test.PowerBi.ExportHost;

using Admin.Schema.PowerBiExport;
using AssociationRegistry.PowerBi.ExportHost;
using AssociationRegistry.PowerBi.ExportHost.Exporters;
using FluentAssertions;
using System.Text;
using Xunit;

public class HoofdactiviteitenRecordWriterTests
{
    [Fact]
    public async Task WithNoDocuments_ThenCsvExportsOnlyHeaders()
    {
        var docs = Array.Empty<PowerBiExportDocument>();

        var content = await GenerateCsv(docs);
        content.Should().BeEquivalentTo($"code,naam,vcode\r\n");
    }

    [Fact]
    public async Task WithOneDocument_ThenCsvExportShouldExport()
    {
        var docs = new List<PowerBiExportDocument>
        {
            new()
            {
                VCode = "V0001001",
                HoofdactiviteitenVerenigingsloket = [
                    new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd", Code = "activiteit"},
                ]
            }
        };

        var content = await GenerateCsv(docs);
        content.Should().BeEquivalentTo($"code,naam,vcode\r\nactiviteit,hoofd,V0001001\r\n");
    }

    [Fact]
    public async Task WithMultipleDocuments_ThenCsvExportShouldExport()
    {
        var docs = new List<PowerBiExportDocument>
        {
            new()
            {
                VCode = "V0001001",
                HoofdactiviteitenVerenigingsloket = [
                    new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd", Code = "activiteit"},
                ]
            },
            new()
            {
                VCode = "V0001002",
                HoofdactiviteitenVerenigingsloket = [
                    new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd1", Code = "activiteit1"},
                    new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd2", Code = "activiteit2"},
                    new HoofdactiviteitVerenigingsloket(){ Naam = "hoofd3", Code = "activiteit3"},
                ]
            }
        };

        var content = await GenerateCsv(docs);
        content.Should().BeEquivalentTo($"code,naam,vcode\r\nactiviteit,hoofd,V0001001\r\nactiviteit1,hoofd1,V0001002\r\nactiviteit2,hoofd2,V0001002\r\nactiviteit3,hoofd3,V0001002\r\n");
    }

    private static async Task<string> GenerateCsv(IEnumerable<PowerBiExportDocument> docs)
    {
        var exporter = new PowerBiDocumentExporter();

        var exportStream = await exporter.Export(docs, new HoofdactiviteitenRecordWriter());

        using var reader = new StreamReader(exportStream, Encoding.UTF8);

        var content = await reader.ReadToEndAsync();

        return content;
    }
}


