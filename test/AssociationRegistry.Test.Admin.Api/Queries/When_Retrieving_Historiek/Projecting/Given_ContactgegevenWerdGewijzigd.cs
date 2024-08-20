namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdGewijzigd = fixture.Create<TestEvent<ContactgegevenWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"{contactgegevenWerdGewijzigd.Data.Contactgegeventype} '{contactgegevenWerdGewijzigd.Data.Waarde}' werd gewijzigd.",
                nameof(ContactgegevenWerdGewijzigd),
                contactgegevenWerdGewijzigd.Data,
                contactgegevenWerdGewijzigd.Initiator,
                contactgegevenWerdGewijzigd.Tijdstip.ToZuluTime()));
    }
}

[UnitTest]
public class Given_ContactgegevenWerdInBeheerGenomenDoorKbo
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdInBeheerGenomenDoorKbo = fixture.Create<TestEvent<ContactgegevenWerdInBeheerGenomenDoorKbo>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdInBeheerGenomenDoorKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"{contactgegevenWerdInBeheerGenomenDoorKbo.Data.Contactgegeventype} '{contactgegevenWerdInBeheerGenomenDoorKbo.Data.Waarde}' werd in beheer genomen door KBO.",
                nameof(ContactgegevenWerdInBeheerGenomenDoorKbo),
                contactgegevenWerdInBeheerGenomenDoorKbo.Data,
                contactgegevenWerdInBeheerGenomenDoorKbo.Initiator,
                contactgegevenWerdInBeheerGenomenDoorKbo.Tijdstip.ToZuluTime()));
    }
}
