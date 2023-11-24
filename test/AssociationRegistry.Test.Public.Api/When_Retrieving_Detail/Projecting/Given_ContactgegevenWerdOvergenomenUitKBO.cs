namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

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
        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<TestEvent<ContactgegevenWerdOvergenomenUitKBO>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdOvergenomenUitKbo, doc);

        doc.Contactgegevens.Should()
           .ContainSingle(c => c.ContactgegevenId == contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId)
           .Which.Should().BeEquivalentTo(
                new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId,
                    Contactgegeventype = contactgegevenWerdOvergenomenUitKbo.Data.Type,
                    Waarde = contactgegevenWerdOvergenomenUitKbo.Data.Waarde,
                    Beschrijving = "",
                    IsPrimair = false,
                });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
