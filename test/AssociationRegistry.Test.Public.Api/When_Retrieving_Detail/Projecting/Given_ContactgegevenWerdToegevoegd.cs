namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using Events;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_contactgegeven_to_the_detail()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var contactgegevenWerdToegevoegd = fixture.Create<TestEvent<ContactgegevenWerdToegevoegd>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdToegevoegd, doc);

        doc.Contactgegevens.Should()
            .ContainSingle(c => c.ContactgegevenId == contactgegevenWerdToegevoegd.Data.ContactgegevenId)
            .Which.Should().BeEquivalentTo(
                new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                    Type = contactgegevenWerdToegevoegd.Data.Type,
                    Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                    Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
                });
    }
}
