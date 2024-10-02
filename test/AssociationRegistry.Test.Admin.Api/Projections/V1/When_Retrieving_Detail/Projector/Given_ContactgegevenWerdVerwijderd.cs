namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging.Bronnen;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdVerwijderd
{
    [Fact]
    public void Then_it_removes_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdVerwijderd = fixture.Create<TestEvent<ContactgegevenWerdVerwijderd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        doc.Contactgegevens = doc.Contactgegevens.Append(
            new Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdVerwijderd.Data.ContactgegevenId,
                Contactgegeventype = fixture.Create<string>(),
                Waarde = fixture.Create<string>(),
                Beschrijving = fixture.Create<string>(),
                IsPrimair = true,
            }
        ).ToArray();

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdVerwijderd, doc);

        doc.Contactgegevens.Should().NotContain(
            new Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdVerwijderd.Data.ContactgegevenId,
                Contactgegeventype = contactgegevenWerdVerwijderd.Data.Type,
                Waarde = contactgegevenWerdVerwijderd.Data.Waarde,
                Beschrijving = contactgegevenWerdVerwijderd.Data.Beschrijving,
                IsPrimair = contactgegevenWerdVerwijderd.Data.IsPrimair,
                Bron = Bron.Initiator,
            });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}

[UnitTest]
public class Given_ContactgegevenWerdVerwijderdUitKbo
{
    [Fact]
    public void Then_it_removes_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdVerwijderd = fixture.Create<TestEvent<ContactgegevenWerdVerwijderdUitKBO>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        doc.Contactgegevens = doc.Contactgegevens.Append(
            new Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdVerwijderd.Data.ContactgegevenId,
                Contactgegeventype = fixture.Create<string>(),
                Waarde = fixture.Create<string>(),
                Beschrijving = fixture.Create<string>(),
                IsPrimair = true,
            }
        ).ToArray();

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdVerwijderd, doc);

        doc.Contactgegevens.Should().NotContain(
            contactgegeven => contactgegeven.ContactgegevenId == contactgegevenWerdVerwijderd.Data.ContactgegevenId);

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
