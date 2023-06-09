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
public class Given_ContactgegevenWerdVerwijderd
{
    [Fact]
    public void Then_it_removes_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAll();
        var contactgegevenWerdVerwijderd = fixture.Create<TestEvent<ContactgegevenWerdVerwijderd>>();
        var projector = new PubliekVerenigingDetailProjection();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();
        doc.Contactgegevens = doc.Contactgegevens.Append(
            new PubliekVerenigingDetailDocument.Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdVerwijderd.Data.ContactgegevenId,
                Type = fixture.Create<string>(),
                Waarde = fixture.Create<string>(),
                Beschrijving = fixture.Create<string>(),
                IsPrimair = fixture.Create<bool>(),
            }).ToArray();

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdVerwijderd, doc);

        doc.Contactgegevens.Should().NotContain(c => c.ContactgegevenId == contactgegevenWerdVerwijderd.Data.ContactgegevenId);
    }
}
