namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using Events;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Formats;
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

        var doc = new BeheerVerenigingHistoriekDocument();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenKonNietOvergenomenWorden, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactgegeven ‘{contactgegevenKonNietOvergenomenWorden.Data.TypeVolgensKbo}' kon niet overgenomen worden uit KBO.",
                nameof(ContactgegevenKonNietOvergenomenWordenUitKBO),
                contactgegevenKonNietOvergenomenWorden.Data,
                contactgegevenKonNietOvergenomenWorden.Initiator,
                contactgegevenKonNietOvergenomenWorden.Tijdstip.FormatAsZuluTime()));
    }
}
