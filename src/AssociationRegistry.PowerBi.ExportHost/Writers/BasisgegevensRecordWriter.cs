﻿namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class BasisgegevensRecordWriter : IRecordWriter
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<BasisgegevensRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            csvWriter.WriteRecord(new BasisgegevensRecord(
                                       vereniging.Bron,
                                       vereniging.Doelgroep.Maximumleeftijd,
                                       vereniging.Doelgroep.Minimumleeftijd,
                                       vereniging.Einddatum,
                                       vereniging.IsUitgeschrevenUitPubliekeDatastroom,
                                       vereniging.KorteBeschrijving,
                                       vereniging.KorteNaam,
                                       vereniging.Naam,
                                       vereniging.Roepnaam,
                                       vereniging.Startdatum,
                                       vereniging.Status,
                                       vereniging.VCode,
                                       vereniging.Verenigingstype.Code,
                                       vereniging.Verenigingstype.Naam,
                                       vereniging.KboNummer,
                                       string.Join(", ", vereniging.CorresponderendeVCodes),
                                       vereniging.AantalVertegenwoordigers,
                                       vereniging.DatumLaatsteAanpassing));

            await csvWriter.NextRecordAsync();
        }
    }
}
