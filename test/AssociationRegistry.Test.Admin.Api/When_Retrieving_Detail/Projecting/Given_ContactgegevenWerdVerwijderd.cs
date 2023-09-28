namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Vereniging.Bronnen;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdVerwijderd
{
    [Fact]
    public void Then_it_removes_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdVerwijderd = fixture.Create<TestEvent<ContactgegevenWerdVerwijderd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();
        doc.Contactgegevens = doc.Contactgegevens.Append(
            new BeheerVerenigingDetailDocument.Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdVerwijderd.Data.ContactgegevenId,
                Type = fixture.Create<string>(),
                Waarde = fixture.Create<string>(),
                Beschrijving = fixture.Create<string>(),
                IsPrimair = true,
            }
        ).ToArray();

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdVerwijderd, doc);

        doc.Contactgegevens.Should().NotContain(
            new BeheerVerenigingDetailDocument.Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdVerwijderd.Data.ContactgegevenId,
                Type = contactgegevenWerdVerwijderd.Data.Type,
                Waarde = contactgegevenWerdVerwijderd.Data.Waarde,
                Beschrijving = contactgegevenWerdVerwijderd.Data.Beschrijving,
                IsPrimair = contactgegevenWerdVerwijderd.Data.IsPrimair,
                Bron = Bron.Initiator,
            });
        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
