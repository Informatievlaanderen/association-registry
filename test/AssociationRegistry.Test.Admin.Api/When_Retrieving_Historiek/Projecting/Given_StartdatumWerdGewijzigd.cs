namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AutoFixture;
using Events;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_StartdatumWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<StartdatumWerdGewijzigd>
                .ToHistoriekProjectie(
                    new Fixture().CustomizeAll().Create<StartdatumWerdGewijzigd>());

        var startdatumString = projectEventOnHistoriekDocument.Event.Data.Startdatum!.Value.ToString(WellknownFormats.DateOnly);
        
        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            $"Startdatum vereniging werd gewijzigd naar '{startdatumString}'.");
    }
}

[UnitTest]
public class Given_StartdatumWerdGewijzigd_With_Null
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_with_empty_string()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<StartdatumWerdGewijzigd>
                .ToHistoriekProjectie(
                    new Fixture().CustomizeAll().Create<StartdatumWerdGewijzigd>()
                        with { Startdatum = null });

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            $"Startdatum vereniging werd gewijzigd naar ''.");
    }
}
