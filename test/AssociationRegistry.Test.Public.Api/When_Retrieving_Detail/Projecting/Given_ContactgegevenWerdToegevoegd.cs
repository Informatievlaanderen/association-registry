namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using Events;
using AssociationRegistry.Public.Schema.Detail;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_contactgegeven_to_the_detail()
    {
        var contactgegevenWerdToegevoegd = new ContactgegevenWerdToegevoegd(
            ContactgegevenId: 666,
            Type: "telefoon",
            Waarde: "007",
            Omschrijving: "James Bond",
            IsPrimair: false);

        var projectEventOnDetailDocument =
            WhenApplying<ContactgegevenWerdToegevoegd>
                .ToDetailProjectie(
                    _ => contactgegevenWerdToegevoegd);

        projectEventOnDetailDocument.DocumentAfterChanges.Contactgegevens.Should().Contain(
            new PubliekVerenigingDetailDocument.Contactgegeven()
            {
                ContactgegevenId = contactgegevenWerdToegevoegd.ContactgegevenId,
                Type = contactgegevenWerdToegevoegd.Type,
                Waarde = contactgegevenWerdToegevoegd.Waarde,
                Omschrijving = contactgegevenWerdToegevoegd.Omschrijving,
                IsPrimair = contactgegevenWerdToegevoegd.IsPrimair,
            });
    }
}
