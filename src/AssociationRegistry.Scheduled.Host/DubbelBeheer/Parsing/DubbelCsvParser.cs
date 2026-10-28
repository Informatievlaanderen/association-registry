namespace AssociationRegistry.Scheduled.Host.DubbelBeheer.Parsing;

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public static class DubbelCsvParser
{
    private static readonly CsvConfiguration Configuration = new(CultureInfo.InvariantCulture)
    {
        DetectDelimiter = true,
        TrimOptions = TrimOptions.Trim,
        PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
    };

    public static IReadOnlyList<DubbelCsvRecord> Parse(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, Configuration);

        return csv.GetRecords<DubbelCsvRecord>().ToList();
    }
}
