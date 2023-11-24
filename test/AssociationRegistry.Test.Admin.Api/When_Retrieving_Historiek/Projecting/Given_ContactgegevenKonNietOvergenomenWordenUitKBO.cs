namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenKonNietOvergenomenWordenUitKBO
{
    [Fact]
    public void Then_it_adds_a_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenKonNietOvergenomenWorden = fixture.Create<TestEvent<ContactgegevenKonNietOvergenomenWordenUitKBO>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenKonNietOvergenomenWorden, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactgegeven ‘{contactgegevenKonNietOvergenomenWorden.Data.Contactgegeventype}' kon niet overgenomen worden uit KBO.",
                nameof(ContactgegevenKonNietOvergenomenWordenUitKBO),
                contactgegevenKonNietOvergenomenWorden.Data,
                contactgegevenKonNietOvergenomenWorden.Initiator,
                contactgegevenKonNietOvergenomenWorden.Tijdstip.ToZuluTime()));
    }
}
