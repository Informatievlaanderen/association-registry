namespace AssociationRegistry.Scheduled.Host.PowerBi.Writers;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using CsvHelper;
using Records;

public class WerkingsgebiedenRecordWriter: IRecordWriter<PowerBiExportDocument>
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
