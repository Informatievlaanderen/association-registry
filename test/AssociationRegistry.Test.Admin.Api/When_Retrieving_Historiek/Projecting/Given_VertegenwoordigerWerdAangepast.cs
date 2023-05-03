namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
using Events;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdAangepast
{
    [Fact]
    public void Then_it_updates_the_vertegenwoordiger_gebeurtenis()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<VertegenwoordigerWerdAangepast>
                .ToHistoriekProjectie();

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Id {projectEventOnHistoriekDocument.Event.Data.VertegenwoordigerId} werd gewijzigd als vertegenwoordiger.",
                nameof(VertegenwoordigerWerdAangepast),
                projectEventOnHistoriekDocument.Event.Data,
                initiator,
                tijdstip));
    }
}
