namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Projections.Detail;
using Events;
using FluentAssertions;
using Vereniging;
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
            Beschrijving: "James Bond",
            IsPrimair: false);

        var projectEventOnDetailDocument =
            When<ContactgegevenWerdVerwijderd>
                .Applying(_ => contactgegevenWerdVerwijderd)
                .ToDetailProjectie(
                    d => d with
                    {
                        Contactgegevens = d.Contactgegevens.Append(
                            new BeheerVerenigingDetailDocument.Contactgegeven()
                            {
                                ContactgegevenId = contactgegevenWerdVerwijderd.ContactgegevenId,
                                Type = contactgegevenWerdVerwijderd.Type,
                                Waarde = contactgegevenWerdVerwijderd.Waarde,
                                Beschrijving = contactgegevenWerdVerwijderd.Beschrijving,
                                IsPrimair = contactgegevenWerdVerwijderd.IsPrimair,
                            }).ToArray(),
                    });

        projectEventOnDetailDocument.Contactgegevens.Should().NotContain(
            new BeheerVerenigingDetailDocument.Contactgegeven()
            {
                ContactgegevenId = contactgegevenWerdVerwijderd.ContactgegevenId,
                Type = contactgegevenWerdVerwijderd.Type,
                Waarde = contactgegevenWerdVerwijderd.Waarde,
                Beschrijving = contactgegevenWerdVerwijderd.Beschrijving,
                IsPrimair = contactgegevenWerdVerwijderd.IsPrimair,
            });
    }
}
