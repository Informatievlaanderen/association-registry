namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;

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
public class Given_ContactgegevenWerdInBeheerGenomenDoorKbo
{
    [Fact]
    public void Then_it_modifies_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdInBeheerGenomenDoorKbo = fixture.Create<TestEvent<ContactgegevenWerdInBeheerGenomenDoorKbo>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        doc.Contactgegevens = new[]
        {
            new Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdInBeheerGenomenDoorKbo.Data.ContactgegevenId,
                Contactgegeventype = contactgegevenWerdInBeheerGenomenDoorKbo.Data.Contactgegeventype,
                Waarde = contactgegevenWerdInBeheerGenomenDoorKbo.Data.Waarde,
                Beschrijving = string.Empty,
                IsPrimair = true,
                Bron = Bron.Initiator,
            },
        };

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdInBeheerGenomenDoorKbo, doc);

        doc.Contactgegevens.Should().BeEquivalentTo(
            new[]
            {
                new Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdInBeheerGenomenDoorKbo.Data.ContactgegevenId,
                    Contactgegeventype = contactgegevenWerdInBeheerGenomenDoorKbo.Data.Contactgegeventype,
                    Waarde = contactgegevenWerdInBeheerGenomenDoorKbo.Data.Waarde,
                    Beschrijving = string.Empty,
                    IsPrimair = true,
                    Bron = Bron.KBO,
                },
            });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
