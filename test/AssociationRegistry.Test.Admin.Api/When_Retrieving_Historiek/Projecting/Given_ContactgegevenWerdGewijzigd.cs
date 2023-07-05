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
                $"'{contactgegevenWerdGewijzigd.Data.Type} {contactgegevenWerdGewijzigd.Data.Waarde}' werd gewijzigd.",
                nameof(ContactgegevenWerdGewijzigd),
                contactgegevenWerdGewijzigd.Data,
                contactgegevenWerdGewijzigd.Initiator,
                contactgegevenWerdGewijzigd.Tijdstip.ToBelgianDateAndTime()));
    }
}
