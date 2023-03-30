namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
using Events;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_contactgegeven()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<ContactgegevenWerdToegevoegd>
                .ToHistoriekProjectie();

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (initiator, tijdstip) => new BeheerVerenigingHistoriekGebeurtenis(
                $"{projectEventOnHistoriekDocument.Event.Data.Type} contactgegeven werd toegevoegd.",
                nameof(ContactgegevenWerdToegevoegd),
                projectEventOnHistoriekDocument.Event.Data,
                initiator,
                tijdstip));
    }
}
