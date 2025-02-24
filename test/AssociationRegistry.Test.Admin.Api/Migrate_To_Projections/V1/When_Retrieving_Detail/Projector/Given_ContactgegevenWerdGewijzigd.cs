namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdGewijzigd = fixture.Create<TestEvent<ContactgegevenWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        doc.Contactgegevens = new[]
        {
            new Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                Contactgegeventype = contactgegevenWerdGewijzigd.Data.Contactgegeventype,
                Waarde = fixture.Create<string>(),
                Beschrijving = fixture.Create<string>(),
                IsPrimair = true,
            },
        };

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdGewijzigd, doc);

        doc.Contactgegevens.Should().BeEquivalentTo(
            new[]
            {
                new Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                    Contactgegeventype = contactgegevenWerdGewijzigd.Data.Contactgegeventype,
                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                },
            });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
