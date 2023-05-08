namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
using Events;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_vertegenwoordiger_gebeurtenis()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<VertegenwoordigerWerdGewijzigd>
                .ToHistoriekProjectie();

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"Vertegenwoordiger {projectEventOnHistoriekDocument.Event.Data.Voornaam} " +
                $"{projectEventOnHistoriekDocument.Event.Data.Achternaam} " +
                $"met ID {projectEventOnHistoriekDocument.Event.Data.VertegenwoordigerId} " +
                $"werd gewijzigd.",
                nameof(VertegenwoordigerWerdGewijzigd),
                projectEventOnHistoriekDocument.Event.Data,
                initiator,
                tijdstip));
    }
}
