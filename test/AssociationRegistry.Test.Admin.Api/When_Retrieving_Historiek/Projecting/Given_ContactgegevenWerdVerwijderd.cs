namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
using Events;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdVerwijderd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<ContactgegevenWerdVerwijderd>
                .ToHistoriekProjectie();

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"{projectEventOnHistoriekDocument.Event.Data.Type} {projectEventOnHistoriekDocument.Event.Data.Waarde} werd verwijderd.",
                nameof(ContactgegevenWerdVerwijderd),
                projectEventOnHistoriekDocument.Event.Data,
                initiator,
                tijdstip));
    }
}
