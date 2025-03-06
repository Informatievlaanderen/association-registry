namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet;

using AutoFixture;
using DuplicateVerenigingDetection;
using FluentAssertions;
using Seed;
using Vereniging;
using Xunit;

public class Then_MunicipaltyStopwords_Are_Not_Applied_To_Gemeente: DuplicateDetectionTest
{
    private readonly ITestOutputHelper _helper;


    public Then_MunicipaltyStopwords_Are_Not_Applied_To_Gemeente(ITestOutputHelper helper): base("duplicates", helper)
    {
        _helper = helper;

    }

    [Fact]
    public async Task With_Gemeente_From_Stopwords()
    {
        var result = await _duplicateVerenigingDetectionService.GetDuplicates(VerenigingsNaam.Create("Ruygi KORTRIJK"),
        [
            _fixture.Create<Locatie>() with
            {
                Adres = _fixture.Create<Adres>() with
                {
                    Gemeente = Gemeentenaam.Hydrate("kortrijk"),
                    Postcode = _fixture.Create<string>(),
                },
            },
        ], includeScore: true, minimumScoreOverride: new MinimumScore(1));

        result.SingleOrDefault(x => x.Naam == "Ruygi KORTRIJK").Should().NotBeNull();
    }
}
