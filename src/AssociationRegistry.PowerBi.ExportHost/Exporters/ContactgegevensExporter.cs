namespace AssociationRegistry.PowerBi.ExportHost.Exporters;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class ContactgegevensExporter
{
    private readonly CsvWriter _csvWriter;

    public ContactgegevensExporter(CsvWriter csvWriter)
    {
        _csvWriter = csvWriter;
    }

    public async Task Export(IEnumerable<PowerBiExportDocument> docs)
    {
        foreach (var vereniging in docs)
        {
            foreach (var contactgegeven in vereniging.Contactgegevens)
            {
                _csvWriter.WriteRecord(new ContactgegevensRecord(
                                           contactgegeven.Beschrijving,
                                           contactgegeven.Bron,
                                           contactgegeven.ContactgegevenId,
                                           contactgegeven.Contactgegeventype,
                                           contactgegeven.IsPrimair,
                                           vereniging.VCode,
                                           contactgegeven.Waarde));

                await _csvWriter.NextRecordAsync();
            }
        }
    }
}
