namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet.Seed;

using AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.ElasticSearch;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.DuplicateVerenigingDetection;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using Newtonsoft.Json;
using Public.Api.Verenigingen.DetailAll;
using Public.Api.Verenigingen.DetailAll.ResponseModels;
using System.Collections.ObjectModel;
using System.Globalization;
using Xunit.Abstractions;
using Adres = Vereniging.Adres;
using Locatie = Vereniging.Locatie;

public class DuplicateDetectionTest
{
    private readonly Adres? _adres;
    protected readonly Fixture _fixture;
    private readonly ElasticClient _elastic;
    private readonly string _duplicateDetectionIndex;
    private readonly ITestOutputHelper _helper;
    protected SearchDuplicateVerenigingDetectionService _duplicateVerenigingDetectionService;
    public IReadOnlyCollection<DuplicateDetectionSeedLine> DubbelDetectieData { get; private set; }
    public IReadOnlyCollection<DuplicateDetectionSeedLine> VerwachteUnieke { get; private set; }

    public DuplicateDetectionTest(string duplicateDetectionIndex, ITestOutputHelper helper)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _duplicateDetectionIndex = duplicateDetectionIndex;
        _helper = helper;

        _elastic = ElasticSearchExtensions.CreateElasticClient(new ElasticSearchOptionsSection()
        {
            Uri = "http://localhost:9200",
            Username = "elastic",
            Password = "local_development",
            Indices = new ElasticSearchOptionsSection.IndicesOptionsSection()
            {
                DuplicateDetection = _duplicateDetectionIndex,
            }
        }, new TestOutputLogger(helper, duplicateDetectionIndex));

        _adres = _fixture.Create<Adres>() with
        {
            Postcode = "8500",
            Gemeente = Gemeentenaam.Hydrate("Kortrijk"),
        };

        InitializeAsync().GetAwaiter().GetResult();
    }



    public async Task InsertGeregistreerdeVerenigingen(IReadOnlyCollection<DuplicateDetectionSeedLine> readVerwachtDubbels)
    {
        var toRegisterDuplicateDetectionDocuments = readVerwachtDubbels.Select(x => new DuplicateDetectionDocument() with
        {
            Naam = x.GeregistreerdeNaam,
            VerenigingsTypeCode = Verenigingstype.FeitelijkeVereniging.Code,
            HoofdactiviteitVerenigingsloket = [],
            Locaties = [_fixture.Create<DuplicateDetectionDocument.Locatie>() with
            {
                Gemeente = _adres.Gemeente.Naam, Postcode = _adres.Postcode
            }]
        });

        foreach (var doc in toRegisterDuplicateDetectionDocuments)
        {
            await _elastic.IndexDocumentAsync(doc);
        }

        await _elastic.Indices.RefreshAsync(Indices.AllIndices);
    }

    public static IReadOnlyCollection<DuplicateDetectionSeedLine> ReadSeed(string associationregistryTestAdminApiDuplicatedetectionGivenAnExtensiveDatasetVerwachtdubbelsCsv)
        => ReadSeedFile(associationregistryTestAdminApiDuplicatedetectionGivenAnExtensiveDatasetVerwachtdubbelsCsv);

    private static IReadOnlyCollection<DuplicateDetectionSeedLine> ReadSeedFile(string associationregistryTestAdminApiDuplicatedetectionGivenAnExtensiveDatasetVerwachtdubbelsCsv)
    {
        var resourceName = associationregistryTestAdminApiDuplicatedetectionGivenAnExtensiveDatasetVerwachtdubbelsCsv;
        var assembly = typeof(Then_Some_Duplicates_Are_Expected).Assembly;
        var stream =  assembly.GetResource(resourceName);

        using var streamReader = new StreamReader(stream);
        using var csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            Quote = '"',
        });

        var records = csvReader.GetRecords<DuplicateDetectionSeedLine>()
                               .ToArray();

        return new ReadOnlyCollection<DuplicateDetectionSeedLine>(records);
    }

    public async Task InitializeAsync()
    {
        if(_elastic.Indices.ExistsAsync(_duplicateDetectionIndex).GetAwaiter().GetResult().Exists)
            _elastic.Indices.DeleteAsync(_duplicateDetectionIndex).GetAwaiter().GetResult();

        _elastic.Indices.CreateDuplicateDetectionIndex(_duplicateDetectionIndex);

        _duplicateVerenigingDetectionService = new SearchDuplicateVerenigingDetectionService(
            _elastic, MinimumScore.Default, NullLogger<SearchDuplicateVerenigingDetectionService>.Instance);

        DubbelDetectieData = ReadSeed("AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet.Seed.verwachte_dubbels.csv");
        await InsertGeregistreerdeVerenigingen(DubbelDetectieData);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicatesFor(string teRegistrerenNaam)
        => await _duplicateVerenigingDetectionService.GetDuplicates(VerenigingsNaam.Create(teRegistrerenNaam),
        [
            _fixture.Create<Locatie>() with
            {
                Adres = _adres,
            },
        ], includeScore: true, minimumScoreOverride: new MinimumScore(3));
}
record Line(VerenigingLine Vereniging);
record VerenigingLine(string Naam);

public class TestOutputLogger : ILogger
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly string _categoryName;

    public TestOutputLogger(ITestOutputHelper outputHelper, string categoryName)
    {
        _outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
        _categoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null; // Scopes are not implemented
    }

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        => true;

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        var message = formatter(state, exception);

        if (!string.IsNullOrEmpty(message))
        {
            var logEntry = $"[{logLevel}] {_categoryName}: {message}";
            _outputHelper.WriteLine(logEntry);
        }

        if (exception != null)
        {
            _outputHelper.WriteLine(exception.ToString());
        }
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        var msg = formatter(state, exception);

        if (!string.IsNullOrEmpty(msg))
        {
            var logEntry = $"[{logLevel}] {_categoryName}: {msg}";
            _outputHelper.WriteLine(logEntry);
        }

        if (exception != null)
        {
            _outputHelper.WriteLine(exception.ToString());
        }
    }
}

public class TestOutputLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _outputHelper;

    public TestOutputLoggerProvider(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new TestOutputLogger(_outputHelper, categoryName);
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
