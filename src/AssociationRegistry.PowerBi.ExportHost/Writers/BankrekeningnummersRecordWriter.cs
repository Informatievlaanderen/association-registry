namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class BankrekeningnummersRecordWriter: IRecordWriter<PowerBiExportDocument>
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<BankrekeningnummerRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            foreach (var bankrekeningnummer in vereniging.Bankrekeningnummers)
            {
                csvWriter.WriteRecord(new BankrekeningnummerRecord(
                                          bankrekeningnummer.BankrekeningnummerId,
                                          bankrekeningnummer.Iban,
                                          bankrekeningnummer.GebruiktVoor,
                                          bankrekeningnummer.Titularis,
                                          vereniging.VCode));

                await csvWriter.NextRecordAsync();
            }
        }
    }
}
