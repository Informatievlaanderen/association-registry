namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using System.Linq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdVerwijderdUitKbo
{
    [Fact]
    public void Then_it_removes_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var contactgegevenWerdVerwijderd = fixture.Create<TestEvent<ContactgegevenWerdVerwijderdUitKBO>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        doc.Contactgegevens = doc.Contactgegevens.Append(
            new PubliekVerenigingDetailDocument.Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdVerwijderd.Data.ContactgegevenId,
                Contactgegeventype = fixture.Create<string>(),
                Waarde = fixture.Create<string>(),
                Beschrijving = fixture.Create<string>(),
                IsPrimair = fixture.Create<bool>(),
            }).ToArray();

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdVerwijderd, doc);

        doc.Contactgegevens.Should().NotContain(c => c.ContactgegevenId == contactgegevenWerdVerwijderd.Data.ContactgegevenId);
        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
