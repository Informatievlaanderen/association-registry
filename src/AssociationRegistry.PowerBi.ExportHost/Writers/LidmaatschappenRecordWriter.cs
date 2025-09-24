namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class LidmaatschappenRecordWriter: IRecordWriter<PowerBiExportDocument>
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<LidmaatschappenRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            foreach (var lidmaatschap in vereniging.Lidmaatschappen)
            {
                csvWriter.WriteRecord(new LidmaatschappenRecord(
                                          lidmaatschap.LidmaatschapId,
                                          lidmaatschap.AndereVereniging,
                                          lidmaatschap.Van,
                                          lidmaatschap.Tot,
                                          lidmaatschap.Identificatie,
                                          lidmaatschap.Beschrijving,
                                          vereniging.VCode));

                await csvWriter.NextRecordAsync();
            }
        }
    }
}
