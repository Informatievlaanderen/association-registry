namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Projections.Historiek;
using Events;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_NaamWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<NaamWerdGewijzigd>
                .ToHistoriekProjectie();

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Naam werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.Naam}'.", nameof(NaamWerdGewijzigd), i, t));
    }
}
