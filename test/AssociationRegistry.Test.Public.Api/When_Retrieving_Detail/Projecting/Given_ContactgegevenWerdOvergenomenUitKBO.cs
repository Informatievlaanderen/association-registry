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
public class Given_ContactgegevenWerdOvergenomenUitKBO
{
    [Fact]
    public void Then_it_adds_the_contactgegeven_to_the_detail()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var contactgegevenWerdOvergenomen = fixture.Create<TestEvent<ContactgegevenWerdOvergenomenUitKBO>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdOvergenomen, doc);

        doc.Contactgegevens.Should()
            .ContainSingle(c => c.ContactgegevenId == contactgegevenWerdOvergenomen.Data.ContactgegevenId)
            .Which.Should().BeEquivalentTo(
                new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdOvergenomen.Data.ContactgegevenId,
                    Type = contactgegevenWerdOvergenomen.Data.Type,
                    Waarde = contactgegevenWerdOvergenomen.Data.Waarde,
                });
        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
        doc.DatumLaatsteAanpassing.Should().Be(contactgegevenWerdOvergenomen.Tijdstip.ToBelgianDate());
    }
}

[UnitTest]
public class Given_ContactgegevenKonNietOvergenomenWordenUitKBO
{
    [Fact]
    public void Then_it_adds_the_contactgegeven_to_the_detail()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var contactgegevenKonNietOvergenomenWordenUitKbo = fixture.Create<TestEvent<ContactgegevenKonNietOvergenomenWordenUitKBO>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(contactgegevenKonNietOvergenomenWordenUitKbo, doc);

        doc.Contactgegevens.Should()
            .ContainEquivalentOf(
                new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    Type = contactgegevenKonNietOvergenomenWordenUitKbo.Data.Type,
                    Waarde = contactgegevenKonNietOvergenomenWordenUitKbo.Data.Waarde,
                });
        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
        doc.DatumLaatsteAanpassing.Should().Be(contactgegevenKonNietOvergenomenWordenUitKbo.Tijdstip.ToBelgianDate());
    }
}
