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
public class Given_ContactgegevenWerdOvergenomenUitKbo
{
    [Fact]
    public void Then_it_adds_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdOvergenomen = fixture.Create<TestEvent<ContactgegevenWerdOvergenomenUitKBO>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdOvergenomen, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactgegeven ‘{contactgegevenWerdOvergenomen.Data.TypeVolgensKbo}' werd overgenomen uit KBO.",
                nameof(ContactgegevenWerdOvergenomenUitKBO),
                contactgegevenWerdOvergenomen.Data,
                contactgegevenWerdOvergenomen.Initiator,
                contactgegevenWerdOvergenomen.Tijdstip.ToZuluTime()));
    }
}
