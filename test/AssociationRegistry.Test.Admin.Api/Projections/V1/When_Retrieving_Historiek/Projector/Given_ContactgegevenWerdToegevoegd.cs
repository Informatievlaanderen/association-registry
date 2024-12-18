namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Formats;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdToegevoegd = fixture.Create<TestEvent<ContactgegevenWerdToegevoegd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdToegevoegd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"{contactgegevenWerdToegevoegd.Data.Contactgegeventype} '{contactgegevenWerdToegevoegd.Data.Waarde}' werd toegevoegd.",
                nameof(ContactgegevenWerdToegevoegd),
                contactgegevenWerdToegevoegd.Data,
                contactgegevenWerdToegevoegd.Initiator,
                contactgegevenWerdToegevoegd.Tijdstip.FormatAsZuluTime()));
    }
}
