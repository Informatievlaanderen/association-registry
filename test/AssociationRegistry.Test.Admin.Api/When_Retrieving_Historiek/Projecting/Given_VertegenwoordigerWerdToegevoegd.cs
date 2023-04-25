namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
using Events;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_vertegenwoordiger_gebeurtenis()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<VertegenwoordigerWerdToegevoegd>
                .ToHistoriekProjectie();

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"{projectEventOnHistoriekDocument.Event.Data.Voornaam} " +
                $"{projectEventOnHistoriekDocument.Event.Data.Achternaam} werd toegevoegd als vertegenwoordiger.",
                nameof(VertegenwoordigerWerdToegevoegd),
                projectEventOnHistoriekDocument.Event.Data,
                initiator,
                tijdstip));
    }
}
