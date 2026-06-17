#!/usr/bin/env dotnet-script
#nullable enable
#r "nuget: Npgsql, 10.0.1"

using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Npgsql;

var options = SeedOptions.Parse(Args.ToArray());

if (options is null)
{
    SeedOptions.PrintUsage();
    Environment.Exit(1);
}

if (options.ShowHelp)
{
    SeedOptions.PrintUsage();
    Environment.Exit(0);
}

if (!File.Exists(options.FilePath))
{
    Console.Error.WriteLine($"File not found: {options.FilePath}");
    Environment.Exit(1);
}

var vCodes = XlsxVCodeReader.Read(options.FilePath, options.SheetName)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                            .Skip(options.Skip)
                            .Take(options.Limit ?? int.MaxValue)
                            .ToArray();

Console.WriteLine($"File: {options.FilePath}");
Console.WriteLine($"Sheet: {options.SheetName}");
Console.WriteLine($"Distinct vCodes selected: {vCodes.Length}");
Console.WriteLine($"Database: {RedactPassword(options.ConnectionString)}");

var dataSource = NpgsqlDataSource.Create(options.ConnectionString);
var connection = await dataSource.OpenConnectionAsync();

var results = new List<SeedResult>();

foreach (var vCode in vCodes)
{
    var exists = await StreamExists(connection, vCode);

    if (exists)
    {
        results.Add(new SeedResult(vCode, "Skipped", "Stream already exists"));
        continue;
    }

    if (!options.Seed)
    {
        results.Add(new SeedResult(vCode, "WouldSeed", "Stream does not exist"));
        continue;
    }

    await AppendRegistrationEvent(connection, vCode, options.Initiator);
    results.Add(new SeedResult(vCode, "Seeded", "Created minimal VZER registration stream"));
}

PrintSummary(results, options);

if (!string.IsNullOrWhiteSpace(options.ReportPath))
{
    SeedReportWriter.Write(options.ReportPath, results);
    Console.WriteLine($"Report written to {options.ReportPath}");
}

Environment.ExitCode = results.Any(x => x.Status == "Failed") ? 2 : 0;

static async Task<bool> StreamExists(NpgsqlConnection connection, string vCode)
{
    await using var command = new NpgsqlCommand(
        "select exists(select 1 from public.mt_streams where id = @vCode)",
        connection);

    command.Parameters.AddWithValue("vCode", vCode);

    return (bool)(await command.ExecuteScalarAsync() ?? false);
}

static async Task AppendRegistrationEvent(NpgsqlConnection connection, string vCode, string initiator)
{
    var correlationId = Guid.NewGuid().ToString();
    var now = DateTimeOffset.UtcNow;

    await using var command = new NpgsqlCommand(
        """
        select public.mt_quick_append_events(
            @stream,
            null,
            '*DEFAULT*',
            array[@eventId]::uuid[],
            array[@eventType]::varchar[],
            array[@dotnetType]::varchar[],
            array[cast(@body as jsonb)],
            array[null]::varchar[],
            array[@correlationId]::varchar[],
            array[cast(@headers as jsonb)])
        """,
        connection);

    command.Parameters.AddWithValue("stream", vCode);
    command.Parameters.AddWithValue("eventId", Guid.NewGuid());
    command.Parameters.AddWithValue("eventType", "VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd");
    command.Parameters.AddWithValue(
        "dotnetType",
        "AssociationRegistry.Events.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, AssociationRegistry");
    command.Parameters.AddWithValue("body", CreateRegistrationEventJson(vCode));
    command.Parameters.AddWithValue("correlationId", correlationId);
    command.Parameters.AddWithValue("headers", JsonSerializer.Serialize(new
    {
        Initiator = initiator,
        Tijdstip = now.ToString("O", CultureInfo.InvariantCulture),
    }));

    await command.ExecuteNonQueryAsync();
}

static string CreateRegistrationEventJson(string vCode)
{
    return JsonSerializer.Serialize(new
    {
        VCode = vCode,
        Naam = $"OR-3203 seed {vCode}",
        KorteNaam = string.Empty,
        KorteBeschrijving = string.Empty,
        Startdatum = (string?)null,
        Doelgroep = new
        {
            Minimumleeftijd = 0,
            Maximumleeftijd = 150,
        },
        IsUitgeschrevenUitPubliekeDatastroom = false,
        Contactgegevens = Array.Empty<object>(),
        Locaties = Array.Empty<object>(),
        Vertegenwoordigers = Array.Empty<object>(),
        HoofdactiviteitenVerenigingsloket = Array.Empty<object>(),
        Bankrekeningnummers = Array.Empty<object>(),
        DuplicatieInfo = (object?)null,
    });
}

static void PrintSummary(IReadOnlyCollection<SeedResult> results, SeedOptions options)
{
    Console.WriteLine($"Mode: {(options.Seed ? "seed" : "dry-run")}");
    Console.WriteLine($"Already existed: {results.Count(x => x.Status == "Skipped")}");
    Console.WriteLine($"Would seed: {results.Count(x => x.Status == "WouldSeed")}");
    Console.WriteLine($"Seeded: {results.Count(x => x.Status == "Seeded")}");

    foreach (var result in results.Where(x => x.Status != "Skipped").Take(10))
        Console.WriteLine($"{result.Status}: {result.VCode} - {result.Message}");
}

static string RedactPassword(string connectionString)
    => Regex.Replace(connectionString, "(Password|Pwd)=([^;]+)", "$1=***", RegexOptions.IgnoreCase);

internal sealed record SeedResult(string VCode, string Status, string Message);

internal sealed record SeedOptions(
    string FilePath,
    string SheetName,
    string ConnectionString,
    bool Seed,
    string? ReportPath,
    int? Limit,
    int Skip,
    string Initiator,
    bool ShowHelp)
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=verenigingsregister;Username=root;Password=root";

    public static SeedOptions? Parse(string[] args)
    {
        var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var flags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (!arg.StartsWith("--", StringComparison.Ordinal))
            {
                Console.Error.WriteLine($"Unexpected argument: {arg}");
                return null;
            }

            if (arg is "--seed" or "--help")
            {
                flags.Add(arg);
                continue;
            }

            if (i + 1 >= args.Length)
            {
                Console.Error.WriteLine($"Missing value for {arg}");
                return null;
            }

            values[arg] = args[++i];
        }

        if (flags.Contains("--help"))
            return new SeedOptions(string.Empty, "Import", DefaultConnectionString, false, null, null, 0, "OR-3203", true);

        if (!values.TryGetValue("--file", out var filePath) || string.IsNullOrWhiteSpace(filePath))
        {
            Console.Error.WriteLine("--file is required.");
            return null;
        }

        if (!TryParseOptionalInt(values, "--limit", out var limit)
            || !TryParseInt(values, "--skip", 0, out var skip))
            return null;

        if (limit.HasValue && limit.Value <= 0)
        {
            Console.Error.WriteLine("--limit must be greater than zero.");
            return null;
        }

        if (skip < 0)
        {
            Console.Error.WriteLine("--skip cannot be negative.");
            return null;
        }

        return new SeedOptions(
            filePath,
            values.GetValueOrDefault("--sheet") ?? "Import",
            values.GetValueOrDefault("--connection-string")
            ?? Environment.GetEnvironmentVariable("IMPORT_ERKENNINGEN_CONNECTION_STRING")
            ?? DefaultConnectionString,
            flags.Contains("--seed"),
            values.GetValueOrDefault("--report"),
            limit,
            skip,
            values.GetValueOrDefault("--initiator") ?? "OR-3203",
            false);
    }

    public static void PrintUsage()
    {
        Console.WriteLine(
            """
            Usage:
              dotnet-script tools/import-erkenningen/seed-vcodes.csx -- --file <xlsx> [options]

            Options:
              --file <path>                 Required. ABB xlsx file.
              --sheet <name>                Sheet name. Defaults to Import.
              --connection-string <value>   Target Marten database. Defaults to local verenigingsregister.
                                            Can also use IMPORT_ERKENNINGEN_CONNECTION_STRING.
              --seed                        Write missing vCode streams. Without this flag, dry-run only.
              --report <path>               Optional CSV report path.
              --limit <number>              Optional number of vCodes to process.
              --skip <number>               Optional number of vCodes to skip.
              --initiator <value>           Metadata initiator. Defaults to OR-3203.
              --help                        Show this help.
            """);
    }

    private static bool TryParseOptionalInt(
        IReadOnlyDictionary<string, string?> values,
        string name,
        out int? parsed)
    {
        parsed = null;

        if (!values.TryGetValue(name, out var value) || string.IsNullOrWhiteSpace(value))
            return true;

        if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var result))
        {
            parsed = result;
            return true;
        }

        Console.Error.WriteLine($"{name} must be an integer.");
        return false;
    }

    private static bool TryParseInt(
        IReadOnlyDictionary<string, string?> values,
        string name,
        int defaultValue,
        out int parsed)
    {
        parsed = defaultValue;

        if (!values.TryGetValue(name, out var value) || string.IsNullOrWhiteSpace(value))
            return true;

        if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out parsed))
            return true;

        Console.Error.WriteLine($"{name} must be an integer.");
        return false;
    }
}

internal static class SeedReportWriter
{
    public static void Write(string reportPath, IReadOnlyCollection<SeedResult> results)
    {
        var builder = new StringBuilder();
        builder.AppendLine("vCode,status,message");

        foreach (var result in results.OrderBy(x => x.VCode, StringComparer.OrdinalIgnoreCase))
            builder.AppendLine(string.Join(',', Csv(result.VCode), Csv(result.Status), Csv(result.Message)));

        File.WriteAllText(reportPath, builder.ToString(), Encoding.UTF8);
    }

    private static string Csv(string value)
        => $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
}

internal static class XlsxVCodeReader
{
    private static readonly XNamespace SpreadsheetNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    private static readonly XNamespace RelationshipNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
    private static readonly XNamespace PackageRelationshipNamespace = "http://schemas.openxmlformats.org/package/2006/relationships";

    public static string[] Read(string filePath, string sheetName)
    {
        using var archive = ZipFile.OpenRead(filePath);
        var sharedStrings = ReadSharedStrings(archive);
        var sheetPath = ResolveSheetPath(archive, sheetName);
        var rows = ReadRows(archive, sheetPath, sharedStrings);

        if (rows.Count == 0)
            return [];

        var headers = rows[0].Cells.Select(x => x.Value.Trim()).ToArray();
        var vCodeColumn = Array.FindIndex(headers, IsVCodeHeader);

        if (vCodeColumn < 0)
            throw new InvalidOperationException("Column 'vCode' or 'V-code' not found.");

        return rows.Skip(1)
                   .Select(row => row.Cells.GetValueOrDefault(vCodeColumn, string.Empty).Trim())
                   .Where(vCode => Regex.IsMatch(vCode, "^V\\d{7}$", RegexOptions.CultureInvariant))
                   .ToArray();
    }

    private static bool IsVCodeHeader(string header)
        => string.Equals(header, "vCode", StringComparison.OrdinalIgnoreCase)
           || string.Equals(header, "V-code", StringComparison.OrdinalIgnoreCase);

    private static string[] ReadSharedStrings(ZipArchive archive)
    {
        var entry = archive.GetEntry("xl/sharedStrings.xml");

        if (entry is null)
            return [];

        using var stream = entry.Open();
        var document = XDocument.Load(stream);

        return document.Descendants(SpreadsheetNamespace + "si")
                       .Select(si => string.Concat(si.Descendants(SpreadsheetNamespace + "t").Select(t => t.Value)))
                       .ToArray();
    }

    private static string ResolveSheetPath(ZipArchive archive, string sheetName)
    {
        var workbookEntry = archive.GetEntry("xl/workbook.xml")
                            ?? throw new InvalidOperationException("Invalid xlsx: xl/workbook.xml not found.");
        var relationshipsEntry = archive.GetEntry("xl/_rels/workbook.xml.rels")
                                 ?? throw new InvalidOperationException("Invalid xlsx: workbook relationships not found.");

        XDocument workbook;
        XDocument relationships;

        using (var stream = workbookEntry.Open())
            workbook = XDocument.Load(stream);

        using (var stream = relationshipsEntry.Open())
            relationships = XDocument.Load(stream);

        var sheet = workbook.Descendants(SpreadsheetNamespace + "sheet")
                            .SingleOrDefault(x => string.Equals(
                                (string?)x.Attribute("name"),
                                sheetName,
                                StringComparison.OrdinalIgnoreCase));

        if (sheet is null)
            throw new InvalidOperationException($"Sheet '{sheetName}' not found.");

        var relationshipId = (string?)sheet.Attribute(RelationshipNamespace + "id")
                             ?? throw new InvalidOperationException($"Sheet '{sheetName}' has no relationship id.");

        var target = relationships.Descendants(PackageRelationshipNamespace + "Relationship")
                                  .Where(x => string.Equals((string?)x.Attribute("Id"), relationshipId, StringComparison.Ordinal))
                                  .Select(x => (string?)x.Attribute("Target"))
                                  .SingleOrDefault();

        if (string.IsNullOrWhiteSpace(target))
            throw new InvalidOperationException($"Relationship target for sheet '{sheetName}' not found.");

        return target.StartsWith("xl/", StringComparison.Ordinal) ? target : $"xl/{target}";
    }

    private static List<ParsedXlsxRow> ReadRows(ZipArchive archive, string sheetPath, IReadOnlyList<string> sharedStrings)
    {
        var sheetEntry = archive.GetEntry(sheetPath)
                         ?? throw new InvalidOperationException($"Invalid xlsx: {sheetPath} not found.");

        XDocument sheetDocument;

        using (var stream = sheetEntry.Open())
            sheetDocument = XDocument.Load(stream);

        return sheetDocument.Descendants(SpreadsheetNamespace + "row")
                            .Select(row => new ParsedXlsxRow(
                                int.Parse((string?)row.Attribute("r") ?? "0", CultureInfo.InvariantCulture),
                                row.Elements(SpreadsheetNamespace + "c")
                                   .ToDictionary(
                                       cell => ColumnIndex((string?)cell.Attribute("r") ?? "A1"),
                                       cell => CellValue(cell, sharedStrings))))
                            .Where(row => row.Cells.Count > 0)
                            .ToList();
    }

    private static string CellValue(XElement cell, IReadOnlyList<string> sharedStrings)
    {
        var cellType = (string?)cell.Attribute("t");

        if (cellType == "inlineStr")
            return string.Concat(cell.Descendants(SpreadsheetNamespace + "t").Select(x => x.Value));

        var rawValue = cell.Element(SpreadsheetNamespace + "v")?.Value ?? string.Empty;

        if (cellType == "s" && int.TryParse(rawValue, NumberStyles.None, CultureInfo.InvariantCulture, out var sharedStringIndex))
            return sharedStringIndex >= 0 && sharedStringIndex < sharedStrings.Count
                ? sharedStrings[sharedStringIndex]
                : string.Empty;

        return rawValue;
    }

    private static int ColumnIndex(string cellReference)
    {
        var column = Regex.Match(cellReference, "^[A-Z]+", RegexOptions.CultureInvariant).Value;
        var index = 0;

        foreach (var character in column)
            index = index * 26 + character - 'A' + 1;

        return index - 1;
    }

    private sealed record ParsedXlsxRow(int RowNumber, IReadOnlyDictionary<int, string> Cells);
}
