namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Contracts.JsonLdContext;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

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
                new PubliekVerenigingDetailDocument.Types.Contactgegeven
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Contactgegeven.CreateWithIdValues(contactgegevenWerdToegevoegd.StreamKey!,
                                                                     contactgegevenWerdToegevoegd.Data.ContactgegevenId.ToString()),
                        JsonLdType.Contactgegeven.Type),
                    ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                    Contactgegeventype = contactgegevenWerdToegevoegd.Data.Contactgegeventype,
                    Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                    Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
                });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
