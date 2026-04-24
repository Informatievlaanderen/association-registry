namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using CsvHelper;
using Records;

public class ErkenningenRecordWriter : IRecordWriter<PowerBiExportDocument>
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<ErkenningenRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            foreach (var erkenning in vereniging.Erkenningen)
            {
                csvWriter.WriteRecord(
                    new ErkenningenRecord(
                        erkenning.ErkenningId,
                        erkenning.GeregistreerdDoor.OvoCode,
                        erkenning.GeregistreerdDoor.Naam,
                        erkenning.IpdcProduct.Naam,
                        erkenning.IpdcProduct.Nummer,
                        erkenning.Startdatum,
                        erkenning.Einddatum,
                        erkenning.Hernieuwingsdatum,
                        erkenning.HernieuwingsUrl,
                        erkenning.RedenSchorsing,
                        erkenning.Status,
                        vereniging.VCode
                    )
                );

                await csvWriter.NextRecordAsync();
            }
        }
    }
}
