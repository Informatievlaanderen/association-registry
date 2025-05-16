namespace AssociationRegistry.Test.Admin.Api;

using AssociationRegistry.Admin.Schema.Detail;
using CsvHelper;
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class CSVExporter
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly ITestOutputHelper _logger;

    public CSVExporter(EventsInDbScenariosFixture fixture, ITestOutputHelper logger)
    {
        _fixture = fixture;
        _logger = logger;
    }

    [Fact(Skip = "To migrate to a real test")]
    public async Task ExportCSV()
    {
        await using var writer = new StreamWriter("output.csv");
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await using var session = _fixture.DocumentStore.LightweightSession();

        session.Query<BeheerVerenigingDetailDocument>().ToList().Should().NotBeEmpty();

        var documents = session.Query<BeheerVerenigingDetailDocument>().ToAsyncEnumerable();

        await foreach (var vereniging in documents)
        {
            foreach (var hoofdactiviteitVerenigingsloket in vereniging.HoofdactiviteitenVerenigingsloket)
            {
                csv.WriteRecord(new HoofdactiviteitenRecord(
                                    hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Naam,
                                    vereniging.VCode));

                await csv.NextRecordAsync();
            }
        }

        await csv.FlushAsync();

        var lines = await File.ReadAllLinesAsync("output.csv");

        lines.Should().NotBeEmpty();

        foreach (var line in lines)
        {
            _logger.WriteLine(line);
        }
    }

    public record HoofdactiviteitenRecord(string Code, string Naam, string VCode);
}
