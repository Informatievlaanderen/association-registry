#!/usr/bin/env dotnet-script
#nullable enable
#r "System.Net.Http"

using System.Globalization;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

var options = ImportOptions.Parse(Args.ToArray());

if (options is null)
{
    ImportOptions.PrintUsage();
    Environment.Exit(1);
}

if (options.ShowHelp)
{
    ImportOptions.PrintUsage();
    Environment.Exit(0);
}

if (options.Upload && options.BaseUrl is null)
{
    Console.Error.WriteLine("--base-url is required when --upload is used.");
    Environment.Exit(1);
}

if (!File.Exists(options.FilePath))
{
    Console.Error.WriteLine($"File not found: {options.FilePath}");
    Environment.Exit(1);
}

var rows = XlsxRecognitionReader.Read(options.FilePath, options.SheetName)
                               .Select(RecognitionImportRow.FromWorksheetRow)
                               .ToArray();

var validationResults = RecognitionImportValidator.Validate(rows);
var executableRows = validationResults
                    .Where(x => x.Errors.Count == 0)
                    .Skip(options.Skip)
                    .Take(options.Limit ?? int.MaxValue)
                    .ToArray();

PrintSummary(validationResults, executableRows.Length, options);

var uploadResults = Array.Empty<UploadResult>();

if (options.Upload)
{
    uploadResults = await UploadAsync(executableRows, options);
    PrintUploadSummary(uploadResults);
}
else
{
    Console.WriteLine("Dry-run only. No API calls were made.");
}

if (!string.IsNullOrWhiteSpace(options.ReportPath))
{
    var reportedRows = ReportWriter.Write(options.ReportPath, validationResults, uploadResults);
    Console.WriteLine($"Failure report written to {options.ReportPath} ({reportedRows} rows)");
}

Environment.ExitCode = uploadResults.Any(x => !x.Succeeded) || validationResults.Any(x => x.Errors.Count > 0) ? 2 : 0;

static async Task<UploadResult[]> UploadAsync(ValidatedRecognitionImportRow[] rows, ImportOptions options)
{
    using var httpClient = new HttpClient
    {
        BaseAddress = options.BaseUrl,
        Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds),
    };

    var token = options.BearerToken ?? Environment.GetEnvironmentVariable("IMPORT_ERKENNINGEN_BEARER_TOKEN");

    if (!string.IsNullOrWhiteSpace(token))
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    var results = new List<UploadResult>();

    foreach (var row in rows)
    {
        var result = await UploadRowAsync(httpClient, row);
        results.Add(result);

        var status = result.Succeeded ? "OK" : "FAIL";
        Console.WriteLine(
            $"{status} row {row.Row.RowNumber}: {row.Row.VCode} {row.Row.Startdatum:yyyy-MM-dd} - {row.Row.Einddatum:yyyy-MM-dd} ({row.Row.GeregistreerdDoor}) {result.StatusCodeText}");

        if (options.DelayMilliseconds > 0)
            await Task.Delay(options.DelayMilliseconds);
    }

    return results.ToArray();
}

static async Task<UploadResult> UploadRowAsync(HttpClient httpClient, ValidatedRecognitionImportRow row)
{
    using var request = new HttpRequestMessage(
        HttpMethod.Post,
        $"v1/verenigingen/{Uri.EscapeDataString(row.Row.VCode)}/erkenningen");

    request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString("D"));
    request.Headers.Add("VR-Initiator", row.Row.GeregistreerdDoor);

    var requestBody = JsonSerializer.Serialize(
        new
        {
            Erkenning = new
            {
                ipdcProductNummer = row.Row.IpdcProduct,
                Startdatum = row.Row.Startdatum?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Einddatum = row.Row.Einddatum?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Hernieuwingsdatum = (string?)null,
                HernieuwingsUrl = string.Empty,
            },
        });

    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

    try
    {
        using var response = await httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        return new UploadResult(
            row.Row.RowNumber,
            row.Row.VCode,
            response.IsSuccessStatusCode,
            (int)response.StatusCode,
            response.ReasonPhrase ?? string.Empty,
            TrimForReport(responseBody));
    }
    catch (Exception exception)
    {
        return new UploadResult(row.Row.RowNumber, row.Row.VCode, false, 0, "Exception", exception.Message);
    }
}

static string TrimForReport(string value)
{
    value = value.ReplaceLineEndings(" ").Trim();
    return value.Length <= 500 ? value : value[..500];
}

static void PrintSummary(
    IReadOnlyCollection<ValidatedRecognitionImportRow> rows,
    int executableRows,
    ImportOptions options)
{
    var invalidRows = rows.Count(x => x.Errors.Count > 0);
    var duplicateVCodes = rows.GroupBy(x => x.Row.VCode)
                             .Where(x => !string.IsNullOrWhiteSpace(x.Key) && x.Count() > 1)
                             .Count();

    var potentialConflicts = rows.Sum(x => x.Warnings.Count(w => w.StartsWith("Potential overlapping recognition", StringComparison.Ordinal)));

    Console.WriteLine($"File: {options.FilePath}");
    Console.WriteLine($"Sheet: {options.SheetName}");
    Console.WriteLine($"Rows: {rows.Count}");
    Console.WriteLine($"Valid rows: {rows.Count - invalidRows}");
    Console.WriteLine($"Invalid rows: {invalidRows}");
    Console.WriteLine($"Duplicate vCodes in file: {duplicateVCodes}");
    Console.WriteLine($"Potential overlapping recognitions in file: {potentialConflicts}");
    Console.WriteLine($"Rows selected for {(options.Upload ? "upload" : "dry-run")}: {executableRows}");

    PrintTopCounts("Initiators", rows.Select(x => x.Row.GeregistreerdDoor));
    PrintTopCounts("IPDC products", rows.Select(x => x.Row.IpdcProduct));

    foreach (var invalid in rows.Where(x => x.Errors.Count > 0).Take(10))
        Console.WriteLine($"Invalid row {invalid.Row.RowNumber}: {string.Join("; ", invalid.Errors)}");

    if (invalidRows > 10)
        Console.WriteLine($"... {invalidRows - 10} more invalid rows");
}

static void PrintTopCounts(string label, IEnumerable<string> values)
{
    var counts = values.Where(x => !string.IsNullOrWhiteSpace(x))
                       .GroupBy(x => x)
                       .OrderByDescending(x => x.Count())
                       .ThenBy(x => x.Key)
                       .Take(10)
                       .Select(x => $"{x.Key}: {x.Count()}");

    Console.WriteLine($"{label}: {string.Join(", ", counts)}");
}

static void PrintUploadSummary(IReadOnlyCollection<UploadResult> results)
{
    Console.WriteLine($"Upload succeeded: {results.Count(x => x.Succeeded)}");
    Console.WriteLine($"Upload failed: {results.Count(x => !x.Succeeded)}");
}

internal sealed record ImportOptions(
    string FilePath,
    string SheetName,
    Uri? BaseUrl,
    bool Upload,
    string? ReportPath,
    int? Limit,
    int Skip,
    int DelayMilliseconds,
    int TimeoutSeconds,
    string? BearerToken,
    bool ShowHelp)
{
    public static ImportOptions? Parse(string[] args)
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

            if (arg is "--upload" or "--help")
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
            return new ImportOptions(string.Empty, "Import", null, false, null, null, 0, 0, 100, null, true);

        if (!values.TryGetValue("--file", out var filePath) || string.IsNullOrWhiteSpace(filePath))
        {
            Console.Error.WriteLine("--file is required.");
            return null;
        }

        Uri? baseUrl = null;

        if (values.TryGetValue("--base-url", out var baseUrlValue) && !string.IsNullOrWhiteSpace(baseUrlValue))
        {
            if (!Uri.TryCreate(EnsureTrailingSlash(baseUrlValue), UriKind.Absolute, out baseUrl))
            {
                Console.Error.WriteLine("--base-url must be an absolute URL.");
                return null;
            }
        }

        if (!TryParseOptionalInt(values, "--limit", out var limit)
            || !TryParseInt(values, "--skip", 0, out var skip)
            || !TryParseInt(values, "--delay-ms", 0, out var delayMilliseconds)
            || !TryParseInt(values, "--timeout-seconds", 100, out var timeoutSeconds))
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

        if (delayMilliseconds < 0)
        {
            Console.Error.WriteLine("--delay-ms cannot be negative.");
            return null;
        }

        if (timeoutSeconds <= 0)
        {
            Console.Error.WriteLine("--timeout-seconds must be greater than zero.");
            return null;
        }

        return new ImportOptions(
            filePath,
            values.GetValueOrDefault("--sheet") ?? "Import",
            baseUrl,
            flags.Contains("--upload"),
            values.GetValueOrDefault("--report"),
            limit,
            skip,
            delayMilliseconds,
            timeoutSeconds,
            values.GetValueOrDefault("--bearer-token"),
            false);
    }

    public static void PrintUsage()
    {
        Console.WriteLine(
            """
            Usage:
              dotnet-script tools/import-erkenningen/import-erkenningen.csx -- --file <xlsx> [options]

            Options:
              --file <path>              Required. ABB xlsx file.
              --sheet <name>             Sheet name. Defaults to Import.
              --report <path>            Optional CSV failure report path.
              --base-url <url>           Admin API base URL. Required with --upload.
              --upload                   Execute API calls. Without this flag, dry-run only.
              --bearer-token <token>     Optional bearer token. Can also use IMPORT_ERKENNINGEN_BEARER_TOKEN.
              --limit <number>           Optional number of valid rows to process.
              --skip <number>            Optional number of valid rows to skip.
              --delay-ms <number>        Optional delay between API calls.
              --timeout-seconds <number> HTTP timeout. Defaults to 100.
              --help                     Show this help.
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

    private static string EnsureTrailingSlash(string value)
        => value.EndsWith("/", StringComparison.Ordinal) ? value : $"{value}/";
}

internal sealed record WorksheetRow(int RowNumber, IReadOnlyDictionary<string, string> Values)
{
    private static readonly IReadOnlyDictionary<string, string[]> ColumnAliases =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["vCode"] = ["V-code"],
            ["geregistreerdDoor"] = ["Erkend door OVO-code"],
            ["ipdcProduct"] = ["IPDC-code"],
            ["startdatum"] = ["Ingangsdatum"],
            ["einddatum"] = ["Einddatum"],
        };

    public string Get(string column)
    {
        if (Values.TryGetValue(column, out var value))
            return value.Trim();

        if (!ColumnAliases.TryGetValue(column, out var aliases))
            return string.Empty;

        foreach (var alias in aliases)
        {
            if (Values.TryGetValue(alias, out value))
                return value.Trim();
        }

        return string.Empty;
    }
}

internal sealed record RecognitionImportRow(
    int RowNumber,
    string VCode,
    string GeregistreerdDoor,
    string IpdcProduct,
    DateOnly? Startdatum,
    DateOnly? Einddatum,
    string RawStartdatum,
    string RawEinddatum)
{
    public static RecognitionImportRow FromWorksheetRow(WorksheetRow row)
    {
        var rawStartdatum = row.Get("startdatum");
        var rawEinddatum = row.Get("einddatum");

        return new RecognitionImportRow(
            row.RowNumber,
            row.Get("vCode"),
            row.Get("geregistreerdDoor"),
            row.Get("ipdcProduct"),
            ExcelDateParser.TryParse(rawStartdatum, out var startdatum) ? startdatum : null,
            ExcelDateParser.TryParse(rawEinddatum, out var einddatum) ? einddatum : null,
            rawStartdatum,
            rawEinddatum);
    }
}

internal sealed record ValidatedRecognitionImportRow(
    RecognitionImportRow Row,
    IReadOnlyCollection<string> Errors,
    IReadOnlyCollection<string> Warnings);

internal static class RecognitionImportValidator
{
    private static readonly Regex VCodeRegex = new("^V\\d{7}$", RegexOptions.Compiled);
    private static readonly Regex OvoRegex = new("^OVO\\d{6}$", RegexOptions.Compiled);

    public static ValidatedRecognitionImportRow[] Validate(IReadOnlyCollection<RecognitionImportRow> rows)
    {
        var results = rows.Select(ValidateSingle).ToArray();

        var warningsByRow = results.ToDictionary(x => x.Row.RowNumber, _ => new List<string>());

        foreach (var group in results
                    .Where(x => x.Errors.Count == 0)
                    .GroupBy(x => new { x.Row.VCode, x.Row.GeregistreerdDoor, x.Row.IpdcProduct }))
        {
            var groupRows = group.OrderBy(x => x.Row.Startdatum).ToArray();

            for (var i = 0; i < groupRows.Length; i++)
            {
                for (var j = i + 1; j < groupRows.Length; j++)
                {
                    if (!Overlaps(groupRows[i].Row, groupRows[j].Row))
                        continue;

                    var message =
                        $"Potential overlapping recognition with row {groupRows[j].Row.RowNumber} for same vCode/OVO/IPDC";
                    warningsByRow[groupRows[i].Row.RowNumber].Add(message);
                    warningsByRow[groupRows[j].Row.RowNumber].Add(
                        $"Potential overlapping recognition with row {groupRows[i].Row.RowNumber} for same vCode/OVO/IPDC");
                }
            }
        }

        return results.Select(x => x with { Warnings = x.Warnings.Concat(warningsByRow[x.Row.RowNumber]).ToArray() })
                      .ToArray();
    }

    private static ValidatedRecognitionImportRow ValidateSingle(RecognitionImportRow row)
    {
        var errors = new List<string>();

        if (!VCodeRegex.IsMatch(row.VCode))
            errors.Add($"Invalid vCode '{row.VCode}'");

        if (string.IsNullOrWhiteSpace(row.GeregistreerdDoor)
            || row.GeregistreerdDoor.Equals("NULL", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("geregistreerdDoor is missing");
        }
        else if (!OvoRegex.IsMatch(row.GeregistreerdDoor))
        {
            errors.Add($"Invalid geregistreerdDoor '{row.GeregistreerdDoor}'");
        }

        if (string.IsNullOrWhiteSpace(row.IpdcProduct))
            errors.Add("ipdcProduct is missing");

        if (!ExcelDateParser.TryParse(row.RawStartdatum, out _))
            errors.Add($"Invalid startdatum '{row.RawStartdatum}'");

        if (!ExcelDateParser.TryParse(row.RawEinddatum, out _))
            errors.Add($"Invalid einddatum '{row.RawEinddatum}'");

        if (row.Startdatum.HasValue && row.Einddatum.HasValue && row.Startdatum.Value > row.Einddatum.Value)
            errors.Add("startdatum is after einddatum");

        return new ValidatedRecognitionImportRow(row, errors, Array.Empty<string>());
    }

    private static bool Overlaps(RecognitionImportRow left, RecognitionImportRow right)
    {
        var leftStart = left.Startdatum ?? DateOnly.MinValue;
        var leftEnd = left.Einddatum ?? DateOnly.MaxValue;
        var rightStart = right.Startdatum ?? DateOnly.MinValue;
        var rightEnd = right.Einddatum ?? DateOnly.MaxValue;

        return rightStart <= leftEnd && rightEnd >= leftStart;
    }
}

internal static class ExcelDateParser
{
    public static bool TryParse(string value, out DateOnly? date)
    {
        date = null;

        if (string.IsNullOrWhiteSpace(value))
            return true;

        if (DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            date = parsedDate;
            return true;
        }

        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var serial))
            return false;

        try
        {
            date = DateOnly.FromDateTime(new DateTime(1899, 12, 30).AddDays(serial));
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
}

internal static class XlsxRecognitionReader
{
    private static readonly XNamespace SpreadsheetNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    private static readonly XNamespace RelationshipNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
    private static readonly XNamespace PackageRelationshipNamespace = "http://schemas.openxmlformats.org/package/2006/relationships";

    public static WorksheetRow[] Read(string filePath, string sheetName)
    {
        using var archive = ZipFile.OpenRead(filePath);
        var sharedStrings = ReadSharedStrings(archive);
        var sheetPath = ResolveSheetPath(archive, sheetName);
        var rows = ReadRows(archive, sheetPath, sharedStrings);

        if (rows.Count == 0)
            return Array.Empty<WorksheetRow>();

        var headers = rows[0].Cells.Select(x => x.Value.Trim()).ToArray();

        return rows.Skip(1)
                   .Where(x => x.Cells.Values.Any(value => !string.IsNullOrWhiteSpace(value)))
                   .Select(row => new WorksheetRow(
                       row.RowNumber,
                       headers.Select((header, columnIndex) => new { header, columnIndex })
                              .Where(x => !string.IsNullOrWhiteSpace(x.header))
                              .ToDictionary(
                                  x => x.header,
                                  x => row.Cells.GetValueOrDefault(x.columnIndex, string.Empty),
                                  StringComparer.OrdinalIgnoreCase)))
                   .ToArray();
    }

    private static string[] ReadSharedStrings(ZipArchive archive)
    {
        var entry = archive.GetEntry("xl/sharedStrings.xml");

        if (entry is null)
            return Array.Empty<string>();

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

internal sealed record UploadResult(
    int RowNumber,
    string VCode,
    bool Succeeded,
    int HttpStatusCode,
    string Reason,
    string Message)
{
    public string StatusCodeText => HttpStatusCode == 0 ? Reason : $"{HttpStatusCode} {Reason}";
}

internal static class ReportWriter
{
    public static int Write(
        string reportPath,
        IReadOnlyCollection<ValidatedRecognitionImportRow> validationResults,
        IReadOnlyCollection<UploadResult> uploadResults)
    {
        var uploadByRow = uploadResults.ToDictionary(x => x.RowNumber);
        var builder = new StringBuilder();

        builder.AppendLine(
            "rowNumber,vCode,geregistreerdDoor,ipdcProduct,startdatum,einddatum,validationStatus,errors,warnings,uploadStatus,httpStatus,message");

        var reportedRows = 0;

        foreach (var result in validationResults.OrderBy(x => x.Row.RowNumber))
        {
            uploadByRow.TryGetValue(result.Row.RowNumber, out var upload);

            if (result.Errors.Count == 0 && upload is not { Succeeded: false })
                continue;

            builder.AppendLine(string.Join(
                ',',
                Csv(result.Row.RowNumber.ToString(CultureInfo.InvariantCulture)),
                Csv(result.Row.VCode),
                Csv(result.Row.GeregistreerdDoor),
                Csv(result.Row.IpdcProduct),
                Csv(result.Row.Startdatum?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty),
                Csv(result.Row.Einddatum?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty),
                Csv(result.Errors.Count == 0 ? "Valid" : "Invalid"),
                Csv(string.Join("; ", result.Errors)),
                Csv(string.Join("; ", result.Warnings)),
                Csv(upload is null ? string.Empty : upload.Succeeded ? "Succeeded" : "Failed"),
                Csv(upload?.StatusCodeText ?? string.Empty),
                Csv(upload?.Message ?? string.Empty)));

            reportedRows++;
        }

        File.WriteAllText(reportPath, builder.ToString(), Encoding.UTF8);
        return reportedRows;
    }

    private static string Csv(string value)
        => $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
}
