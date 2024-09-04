﻿namespace AssociationRegistry.PowerBi.ExportHost.Exporters;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class BasisgegevensExporter
{
    private readonly CsvWriter _csvWriter;

    public BasisgegevensExporter(CsvWriter csvWriter)
    {
        _csvWriter = csvWriter;
    }

    public async Task Export(IEnumerable<PowerBiExportDocument> docs)
    {
        foreach (var vereniging in docs)
        {
            _csvWriter.WriteRecord(new BasisgegevensRecord(
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

            await _csvWriter.NextRecordAsync();
        }
    }
}