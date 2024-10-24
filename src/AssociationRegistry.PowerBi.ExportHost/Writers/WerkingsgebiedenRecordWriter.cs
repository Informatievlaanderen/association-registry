namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class WerkingsgebiedenRecordWriter : IRecordWriter
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<WerkingsgebiedenRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            foreach (var werkgebied in vereniging.Werkingsgebieden)
            {
                csvWriter.WriteRecord(new WerkingsgebiedenRecord(
                                          werkgebied.Code, werkgebied.Naam,
                                          vereniging.VCode));

                await csvWriter.NextRecordAsync();
            }
        }
    }
}
