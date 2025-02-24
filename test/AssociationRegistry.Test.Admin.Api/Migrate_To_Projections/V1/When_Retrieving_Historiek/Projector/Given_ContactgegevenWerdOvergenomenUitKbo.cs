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
                contactgegevenWerdOvergenomen.Tijdstip.FormatAsZuluTime()));
    }
}

[UnitTest]
public class Given_ContactgegevenWerdGewijzigdUitKbo
{
    [Fact]
    public void Then_it_adds_the_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdgewijzigdUitKbo = fixture.Create<TestEvent<ContactgegevenWerdGewijzigdInKbo>>();

        var doc = new BeheerVerenigingHistoriekDocument();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdgewijzigdUitKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"In KBO werd contactgegeven ‘{contactgegevenWerdgewijzigdUitKbo.Data.TypeVolgensKbo}' gewijzigd.",
                nameof(ContactgegevenWerdGewijzigdInKbo),
                contactgegevenWerdgewijzigdUitKbo.Data,
                contactgegevenWerdgewijzigdUitKbo.Initiator,
                contactgegevenWerdgewijzigdUitKbo.Tijdstip.FormatAsZuluTime()));
    }
}
