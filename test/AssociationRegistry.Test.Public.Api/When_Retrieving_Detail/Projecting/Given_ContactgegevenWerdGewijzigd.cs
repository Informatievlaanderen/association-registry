namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using JsonLdContext;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var contactgegevenWerdGewijzigd = fixture.Create<TestEvent<ContactgegevenWerdGewijzigd>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        doc.Contactgegevens = doc.Contactgegevens.Append(
            new PubliekVerenigingDetailDocument.Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                Contactgegeventype = contactgegevenWerdGewijzigd.Data.Contactgegeventype,
                Waarde = fixture.Create<string>(),
                Beschrijving = fixture.Create<string>(),
                IsPrimair = fixture.Create<bool>(),
            }).ToArray();

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdGewijzigd, doc);

        doc.Contactgegevens.Should()
           .ContainSingle(c => c.ContactgegevenId == contactgegevenWerdGewijzigd.Data.ContactgegevenId)
           .Which.Should().BeEquivalentTo(
                new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Contactgegeven.CreateWithIdValues(contactgegevenWerdGewijzigd.StreamKey!,
                                                                     contactgegevenWerdGewijzigd.Data.ContactgegevenId.ToString()),
                        JsonLdType.Contactgegeven.Type),
                    ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                    Contactgegeventype = contactgegevenWerdGewijzigd.Data.Contactgegeventype,
                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
