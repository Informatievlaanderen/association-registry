namespace AssociationRegistry.Vereniging;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Framework;
using System.Collections.ObjectModel;
using System.Globalization;

public class NutsLauReader
{
    public static IReadOnlyCollection<NutsLauLine> Read()
    {
        var resourceName = "AssociationRegistry.Resources.nuts-lau-codes.csv";
        var assembly = typeof(Vereniging).Assembly;
        var stream =  assembly.GetResource(resourceName);

        using var streamReader = new StreamReader(stream);
        using var csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
        });

        var records = csvReader.GetRecords<NutsLauLine>()
                               .ToArray();

        return new ReadOnlyCollection<NutsLauLine>(records);
    }
}

public record NutsLauLine
{
    [Index(0)] public string Nuts { get; init; } = string.Empty;
    [Index(1)] public string Lau { get; init; } = string.Empty;
    [Index(2)] public string Gemeente { get; init; } = string.Empty;
}
