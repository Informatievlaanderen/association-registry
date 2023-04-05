namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.ContactGegevens;
using AssociationRegistry.Events;
using AssociationRegistry.Public.Schema.Detail;
using FluentAssertions;
using Parlot.Fluent;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdVerwijderd
{
    [Fact]
    public void Then_it_removes_the_contactgegeven()
    {
        var contactgegevenWerdVerwijderd = new ContactgegevenWerdVerwijderd(
            ContactgegevenId: 666,
            Type: ContactgegevenType.Telefoon,
            Waarde: "007",
            Omschrijving: "James Bond",
            IsPrimair: false);

        var projectEventOnDetailDocument =
            When<ContactgegevenWerdVerwijderd>
                .Applying(_ => contactgegevenWerdVerwijderd)
                .ToDetailProjectie(
                    d => d with
                    {
                        Contactgegevens = d.Contactgegevens.Append(
                            new PubliekVerenigingDetailDocument.Contactgegeven()
                            {
                                ContactgegevenId = contactgegevenWerdVerwijderd.ContactgegevenId,
                                Type = Enum.GetName(contactgegevenWerdVerwijderd.Type)!,
                                Waarde = contactgegevenWerdVerwijderd.Waarde,
                                Omschrijving = contactgegevenWerdVerwijderd.Omschrijving,
                                IsPrimair = contactgegevenWerdVerwijderd.IsPrimair,
                            }).ToArray()
                    });

        projectEventOnDetailDocument.Contactgegevens.Should().NotContain(
            new PubliekVerenigingDetailDocument.Contactgegeven()
            {
                ContactgegevenId = contactgegevenWerdVerwijderd.ContactgegevenId,
                Type = Enum.GetName(contactgegevenWerdVerwijderd.Type),
                Waarde = contactgegevenWerdVerwijderd.Waarde,
                Omschrijving = contactgegevenWerdVerwijderd.Omschrijving,
                IsPrimair = contactgegevenWerdVerwijderd.IsPrimair,
            });
    }
}
