﻿namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class LocatiesRecordWriter : IRecordWriter
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<LocatiesRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            foreach (var locatie in vereniging.Locaties)
            {
                csvWriter.WriteRecord(new LocatiesRecord(
                                           locatie.AdresId?.Broncode,
                                           locatie.AdresId?.Bronwaarde,
                                           locatie.Adresvoorstelling,
                                           locatie.Bron,
                                           locatie.Adres?.Busnummer,
                                           locatie.Adres?.Gemeente,
                                           locatie.Adres?.Huisnummer,
                                           locatie.IsPrimair,
                                           locatie.Adres?.Land,
                                           locatie.LocatieId,
                                           locatie.Locatietype,
                                           locatie.Naam,
                                           locatie.Adres?.Postcode,
                                           locatie.Adres?.Straatnaam,
                                           vereniging.VCode));

                await csvWriter.NextRecordAsync();
            }
        }

    }

}
