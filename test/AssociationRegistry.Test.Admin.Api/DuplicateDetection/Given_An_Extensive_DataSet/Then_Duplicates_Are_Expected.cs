namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet;

using AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.ElasticSearch;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.ObjectModel;
using System.Globalization;
using Xunit;

public record DuplicateDetectionSeedLine(
    [property:Name("Naam")]string Naam,
    [property:Name("Gemeentenaam")]string Gemeentenaam,
    [property:Name("Postcode")]string Postcode);

public class Then_Duplicates_Are_Expected
{
    [Fact]
    public async Task Test()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var dupIndex = "duplicate_test_koen";

        var elastic = ElasticSearchExtensions.CreateElasticClient(new ElasticSearchOptionsSection()
        {
            Uri = "http://localhost:9200",
            Username = "elastic",
            Password = "local_development",
            Indices = new ElasticSearchOptionsSection.IndicesOptionsSection()
            {
                DuplicateDetection = dupIndex,
            }
        }, NullLogger.Instance);

        if((await elastic.Indices.ExistsAsync(dupIndex)).Exists)
                await elastic.Indices.DeleteAsync(dupIndex);

        elastic.Indices.CreateDuplicateDetectionIndex(dupIndex);

        var duplicateDetectionSeedLines = Read().Select(x => new DuplicateDetectionDocument() with
        {
            Naam = x.Naam,
            VerenigingsTypeCode = Verenigingstype.FeitelijkeVereniging.Code,
            HoofdactiviteitVerenigingsloket = [],
            Locaties = [fixture.Create<DuplicateDetectionDocument.Locatie>() with{ Gemeente = x.Gemeentenaam, Postcode = x.Postcode}]
        });

        var batches = duplicateDetectionSeedLines.Chunk(500);

        foreach (var batch in batches)
        {
            await elastic.BulkAsync(b => b
                                        .Index(dupIndex)
                                        .IndexMany(batch));
        }

        var duplicateVerenigingDetectionService = new SearchDuplicateVerenigingDetectionService(elastic, NullLogger<SearchDuplicateVerenigingDetectionService>.Instance);

        var duplicates = await duplicateVerenigingDetectionService.GetDuplicates(VerenigingsNaam.Create("Ryugi Kortrijk"),
                                                                                         new[]
                                                                                         {
                                                                                             fixture.Create<Locatie>() with{
                                                                                                 Adres = fixture.Create<Adres>() with
                                                                                                 {
                                                                                                     Postcode = "8500",
                                                                                                     Gemeente = Gemeentenaam.Hydrate("Kortrijk"),
                                                                                                 }
                                                                                             }
                                                                                         });

        duplicates.Select(x => x.Naam).Should().BeEquivalentTo(["Ruygo Judoschool KORTRIJK"]);
    }

    public static IReadOnlyCollection<DuplicateDetectionSeedLine> Read()
    {
        var resourceName = "AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet.data.csv";
        var assembly = typeof(Then_Duplicates_Are_Expected).Assembly;
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
}
