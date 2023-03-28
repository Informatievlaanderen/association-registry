namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Projections.Historiek;
using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
using Events;
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
                .ToHistoriekProjectie();

        var startdatumString = projectEventOnHistoriekDocument.Event.Data.Startdatum!.Value.ToString(WellknownFormats.DateOnly);

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Startdatum werd gewijzigd naar '{startdatumString}'.",
                nameof(StartdatumWerdGewijzigd),
                new StartdatumWerdGewijzigdData(startdatumString),
                initiator,
                tijdstip));
    }
}

[UnitTest]
public class Given_StartdatumWerdGewijzigd_With_Null
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_with_null()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<StartdatumWerdGewijzigd>
                .ToHistoriekProjectie(e => e with { Startdatum = null });

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                "Startdatum werd verwijderd.",
                nameof(StartdatumWerdGewijzigd),
                new StartdatumWerdGewijzigdData(null),
                initiator,
                tijdstip));
    }
}
