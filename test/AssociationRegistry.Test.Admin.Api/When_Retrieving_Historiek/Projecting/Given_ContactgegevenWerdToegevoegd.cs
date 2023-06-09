namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAll();
        var projection = new BeheerVerenigingHistoriekProjection();
        var contactgegevenWerdToegevoegd = fixture.Create<TestEvent<ContactgegevenWerdToegevoegd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        projection.Apply(contactgegevenWerdToegevoegd, doc);


        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"{contactgegevenWerdToegevoegd.Data.Type} {contactgegevenWerdToegevoegd.Data.Waarde} werd toegevoegd.",
                nameof(ContactgegevenWerdToegevoegd),
                contactgegevenWerdToegevoegd.Data,
                contactgegevenWerdToegevoegd.Initiator,
                contactgegevenWerdToegevoegd.Tijdstip.ToBelgianDateAndTime()));
    }
}
