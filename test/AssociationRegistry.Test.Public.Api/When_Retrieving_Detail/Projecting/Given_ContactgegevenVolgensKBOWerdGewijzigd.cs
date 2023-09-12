namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenVolgensKBOWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();

        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<TestEvent<ContactgegevenWerdOvergenomenUitKBO>>();

        var contactgegevenVolgensKboWerdGewijzigd = new TestEvent<ContactgegevenVolgensKBOWerdGewijzigd>(
            fixture.Create<ContactgegevenVolgensKBOWerdGewijzigd>()
                with
                {
                    ContactgegevenId = contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId,
                }
        );

        var doc = PubliekVerenigingDetailProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdOvergenomenUitKbo, doc);
        PubliekVerenigingDetailProjector.Apply(contactgegevenVolgensKboWerdGewijzigd, doc);

        doc.Contactgegevens.Should()
           .ContainSingle(c => c.ContactgegevenId == contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId)
           .Which.Should().BeEquivalentTo(
                new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId,
                    Type = contactgegevenWerdOvergenomenUitKbo.Data.Type,
                    Waarde = contactgegevenWerdOvergenomenUitKbo.Data.Waarde,
                    Beschrijving = contactgegevenVolgensKboWerdGewijzigd.Data.Beschrijving,
                    IsPrimair = contactgegevenVolgensKboWerdGewijzigd.Data.IsPrimair,
                });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
        doc.DatumLaatsteAanpassing.Should().Be(contactgegevenVolgensKboWerdGewijzigd.Tijdstip.ToBelgianDate());
    }
}
