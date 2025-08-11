namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Contracts.JsonLdContext;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_ContactgegevenUitKBOWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();

        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<TestEvent<ContactgegevenWerdOvergenomenUitKBO>>();

        var contactgegevenUitKboWerdGewijzigd = new TestEvent<ContactgegevenUitKBOWerdGewijzigd>(
            fixture.Create<ContactgegevenUitKBOWerdGewijzigd>()
                with
                {
                    ContactgegevenId = contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId,
                }
        );

        var doc = PubliekVerenigingDetailProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdOvergenomenUitKbo, doc);
        PubliekVerenigingDetailProjector.Apply(contactgegevenUitKboWerdGewijzigd, doc);

        doc.Contactgegevens.Should()
           .ContainSingle(c => c.ContactgegevenId == contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId)
           .Which.Should().BeEquivalentTo(
                new PubliekVerenigingDetailDocument.Types.Contactgegeven
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Contactgegeven.CreateWithIdValues(contactgegevenWerdOvergenomenUitKbo.StreamKey!,
                                                                     contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId.ToString()),
                        JsonLdType.Contactgegeven.Type),
                    ContactgegevenId = contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId,
                    Contactgegeventype = contactgegevenWerdOvergenomenUitKbo.Data.Contactgegeventype,
                    Waarde = contactgegevenWerdOvergenomenUitKbo.Data.Waarde,
                    Beschrijving = contactgegevenUitKboWerdGewijzigd.Data.Beschrijving,
                    IsPrimair = contactgegevenUitKboWerdGewijzigd.Data.IsPrimair,
                });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
