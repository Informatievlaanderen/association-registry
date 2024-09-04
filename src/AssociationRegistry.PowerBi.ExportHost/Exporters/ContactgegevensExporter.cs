namespace AssociationRegistry.PowerBi.ExportHost.Exporters;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class ContactgegevensExporter : IExporter
{
   public async Task Export(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<ContactgegevensRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            foreach (var contactgegeven in vereniging.Contactgegevens)
            {
                csvWriter.WriteRecord(new ContactgegevensRecord(
                                           contactgegeven.Beschrijving,
                                           contactgegeven.Bron,
                                           contactgegeven.ContactgegevenId,
                                           contactgegeven.Contactgegeventype,
                                           contactgegeven.IsPrimair,
                                           vereniging.VCode,
                                           contactgegeven.Waarde));

                await csvWriter.NextRecordAsync();
            }
        }
    }
}
