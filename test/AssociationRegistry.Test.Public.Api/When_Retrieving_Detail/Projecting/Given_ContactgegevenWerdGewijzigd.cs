namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using Events;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using FluentAssertions;
using Framework;
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
                Type = contactgegevenWerdGewijzigd.Data.Type,
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
                    ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                    Type = contactgegevenWerdGewijzigd.Data.Type,
                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                });
        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
        doc.DatumLaatsteAanpassing.Should().Be(contactgegevenWerdGewijzigd.Tijdstip.ToBelgianDate());
    }
}
