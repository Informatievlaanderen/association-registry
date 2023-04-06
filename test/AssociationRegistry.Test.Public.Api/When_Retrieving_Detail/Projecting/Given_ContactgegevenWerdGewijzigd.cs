namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using ContactGegevens;
using Events;
using AssociationRegistry.Public.Schema.Detail;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_contactgegeven()
    {
        var contactgegevenWerdGewijzigd = new ContactgegevenWerdGewijzigd(
            ContactgegevenId: 666,
            Type: ContactgegevenType.Telefoon,
            Waarde: "007",
            Omschrijving: "James Bond",
            IsPrimair: false);

        var projectEventOnDetailDocument =
            When<ContactgegevenWerdGewijzigd>
                .Applying(_ => contactgegevenWerdGewijzigd)
                .ToDetailProjectie(
                    d => d with
                    {
                        Contactgegevens = d.Contactgegevens.Append(
                            new PubliekVerenigingDetailDocument.Contactgegeven
                            {
                                ContactgegevenId = contactgegevenWerdGewijzigd.ContactgegevenId,
                                Type = contactgegevenWerdGewijzigd.Type,
                                Waarde = "006",
                                Omschrijving = "Alec Trevelyan",
                                IsPrimair = true,
                            }).ToArray(),
                    });

        projectEventOnDetailDocument.Contactgegevens.Should().Contain(
            new PubliekVerenigingDetailDocument.Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdGewijzigd.ContactgegevenId,
                Type = contactgegevenWerdGewijzigd.Type,
                Waarde = contactgegevenWerdGewijzigd.Waarde,
                Omschrijving = contactgegevenWerdGewijzigd.Omschrijving,
                IsPrimair = contactgegevenWerdGewijzigd.IsPrimair,
            });
    }
}
