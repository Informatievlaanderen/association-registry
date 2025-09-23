namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet.Seed;

using AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.ElasticSearch;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;
using Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Elastic.Clients.Elasticsearch;
using System.Collections.ObjectModel;
using System.Globalization;

using ITestOutputHelper = Xunit.ITestOutputHelper;
using Adres = AssociationRegistry.DecentraalBeheer.Vereniging.Adressen.Adres;
using Locatie = AssociationRegistry.DecentraalBeheer.Vereniging.Locatie;

public class DuplicateDetectionTest
{
    private readonly Adres? _adres;
    protected readonly Fixture _fixture;
    private readonly ElasticsearchClient _elastic;
    private readonly string _duplicateDetectionIndex;
    private readonly ITestOutputHelper _helper;
    protected ZoekDuplicateVerenigingenQuery DuplicateVerenigingenQuery;
    private ElasticSearchOptionsSection _elasticSearchOptionsSection;
    public IReadOnlyCollection<DuplicateDetectionSeedLine> DubbelDetectieData { get; private set; }
    public IReadOnlyCollection<DuplicateDetectionSeedLine> VerwachteUnieke { get; private set; }

    public DuplicateDetectionTest(string duplicateDetectionIndex, ITestOutputHelper helper)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _duplicateDetectionIndex = duplicateDetectionIndex;
        _helper = helper;

        _elasticSearchOptionsSection = new ElasticSearchOptionsSection()
        {
            Uri = "http://localhost:9200",
            Username = "elastic",
            Password = "local_development",
            Indices = new ElasticSearchOptionsSection.IndicesOptionsSection()
            {
                DuplicateDetection = _duplicateDetectionIndex,
            }
        };

        _elastic = ElasticSearchExtensions.CreateElasticClient(_elasticSearchOptionsSection, new TestOutputLogger(helper, duplicateDetectionIndex));

        _adres = _fixture.Create<Adres>() with
        {
            Postcode = "8500",
            Gemeente = Gemeentenaam.Hydrate("kortrijk"),
        };

        InitializeAsync().GetAwaiter().GetResult();
    }

    public async Task InsertGeregistreerdeVerenigingen(IReadOnlyCollection<DuplicateDetectionSeedLine> readVerwachtDubbels)
    {
        var toRegisterDuplicateDetectionDocuments = readVerwachtDubbels.Select(x => new DuplicateDetectionDocument() with
        {
            Naam = x.GeregistreerdeNaam,
            VerenigingsTypeCode = Verenigingstype.FeitelijkeVereniging.Code,
            VerenigingssubtypeCode = VerenigingssubtypeCode.NietBepaald.Code,
            HoofdactiviteitVerenigingsloket = [],
            Locaties =
            [
                _fixture.Create<DuplicateDetectionDocument.Locatie>() with
                {
                    Gemeente = _adres.Gemeente.Naam, Postcode = _adres.Postcode
                }
            ]
        });

        foreach (var doc in toRegisterDuplicateDetectionDocuments)
        {
            await _elastic.IndexAsync(doc);
        }

        await _elastic.Indices.RefreshAsync(Indices.All);
    }

    public static IReadOnlyCollection<DuplicateDetectionSeedLine> ReadSeed(
        string associationregistryTestAdminApiDuplicatedetectionGivenAnExtensiveDatasetVerwachtdubbelsCsv)
        => ReadSeedFile(associationregistryTestAdminApiDuplicatedetectionGivenAnExtensiveDatasetVerwachtdubbelsCsv);

    private static IReadOnlyCollection<DuplicateDetectionSeedLine> ReadSeedFile(
        string associationregistryTestAdminApiDuplicatedetectionGivenAnExtensiveDatasetVerwachtdubbelsCsv)
    {
        var resourceName = associationregistryTestAdminApiDuplicatedetectionGivenAnExtensiveDatasetVerwachtdubbelsCsv;
        var assembly = typeof(DuplicateDetectionTest).Assembly;
        var stream = assembly.GetResource(resourceName);

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

    public async ValueTask InitializeAsync()
    {
        if (_elastic.Indices.ExistsAsync(_duplicateDetectionIndex).GetAwaiter().GetResult().Exists)
            _elastic.Indices.DeleteAsync(_duplicateDetectionIndex).GetAwaiter().GetResult();

        await _elastic.CreateDuplicateDetectionIndexAsync(_duplicateDetectionIndex);

        DuplicateVerenigingenQuery = new ZoekDuplicateVerenigingenQuery(
            _elastic,_elasticSearchOptionsSection, MinimumScore.Default, NullLogger<ZoekDuplicateVerenigingenQuery>.Instance);

        DubbelDetectieData =
            ReadSeed("AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet.Seed.verwachte_dubbels.csv");

        await InsertGeregistreerdeVerenigingen(DubbelDetectieData);
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicatesFor(string teRegistrerenNaam)
        => await DuplicateVerenigingenQuery.ExecuteAsync(VerenigingsNaam.Create(teRegistrerenNaam),
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

    public void Log<TState>(
        Microsoft.Extensions.Logging.LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
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
