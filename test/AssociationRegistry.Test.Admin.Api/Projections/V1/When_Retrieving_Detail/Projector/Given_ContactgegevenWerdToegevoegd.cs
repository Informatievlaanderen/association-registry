namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Vereniging.Bronnen;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_contactgegeven_to_the_detail()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdToegevoegd = fixture.Create<TestEvent<ContactgegevenWerdToegevoegd>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdToegevoegd, doc);

        doc.Contactgegevens.Should().ContainEquivalentOf(
            new Contactgegeven
            {
                JsonLdMetadata = new JsonLdMetadata
                {
                    Id = JsonLdType.Contactgegeven.CreateWithIdValues(
                        doc.VCode, contactgegevenWerdToegevoegd.Data.ContactgegevenId.ToString()),
                    Type = JsonLdType.Contactgegeven.Type,
                },
                ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                Contactgegeventype = contactgegevenWerdToegevoegd.Data.Contactgegeventype,
                Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
                Bron = Bron.Initiator,
            });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
