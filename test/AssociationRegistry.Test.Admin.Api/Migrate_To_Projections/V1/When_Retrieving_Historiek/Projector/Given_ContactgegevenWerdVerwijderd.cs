namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdVerwijderd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdVerwijderd = fixture.Create<TestEvent<ContactgegevenWerdVerwijderd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdVerwijderd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"{contactgegevenWerdVerwijderd.Data.Type} '{contactgegevenWerdVerwijderd.Data.Waarde}' werd verwijderd.",
                nameof(ContactgegevenWerdVerwijderd),
                contactgegevenWerdVerwijderd.Data,
                contactgegevenWerdVerwijderd.Initiator,
                contactgegevenWerdVerwijderd.Tijdstip.FormatAsZuluTime()));
    }
}

[UnitTest]
public class Given_ContactgegevenWerdVerwijderdUitKbo
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdVerwijderd = fixture.Create<TestEvent<ContactgegevenWerdVerwijderdUitKBO>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdVerwijderd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"In KBO werd contactgegeven ‘{contactgegevenWerdVerwijderd.Data.TypeVolgensKbo}' verwijderd.",
                nameof(ContactgegevenWerdVerwijderdUitKBO),
                contactgegevenWerdVerwijderd.Data,
                contactgegevenWerdVerwijderd.Initiator,
                contactgegevenWerdVerwijderd.Tijdstip.FormatAsZuluTime()));
    }
}
