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
                new ContactgegevenWerdToegevoegdData(
                    projectEventOnHistoriekDocument.Event.Data.ContactgegevenId,
                    projectEventOnHistoriekDocument.Event.Data.Type,
                    projectEventOnHistoriekDocument.Event.Data.Waarde,
                    projectEventOnHistoriekDocument.Event.Data.Omschrijving,
                    projectEventOnHistoriekDocument.Event.Data.IsPrimair),
                initiator,
                tijdstip));
    }
}
