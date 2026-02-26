namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using CsvHelper;
using Records;

public class BankrekeningnummerRecordWriter : IRecordWriter<PowerBiExportDocument>
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<BankrekeningnummerRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            foreach (var bankrekeningnummer in vereniging.Bankrekeningnummers)
            {
                csvWriter.WriteRecord(
                    new BankrekeningnummerRecord(
                        bankrekeningnummer.BankrekeningnummerId,
                        bankrekeningnummer.Doel,
                        vereniging.VCode,
                        string.Join(", ", bankrekeningnummer.BevestigdDoor),
                        bankrekeningnummer.Bron
                    )
                );
                await csvWriter.NextRecordAsync();
            }
        }
    }
}
