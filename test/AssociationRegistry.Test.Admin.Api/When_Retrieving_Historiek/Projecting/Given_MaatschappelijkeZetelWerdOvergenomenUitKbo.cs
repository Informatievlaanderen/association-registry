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
public class Given_MaatschappelijkeZetelWerdOvergenomenUitKbo
{
    [Fact]
    public void Then_it_adds_the_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var maatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<TestEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(maatschappelijkeZetelWerdOvergenomenUitKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd overgenomen uit KBO.",
                nameof(MaatschappelijkeZetelWerdOvergenomenUitKbo),
                maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie,
                maatschappelijkeZetelWerdOvergenomenUitKbo.Initiator,
                maatschappelijkeZetelWerdOvergenomenUitKbo.Tijdstip.ToBelgianDateAndTime()));
    }
}

[UnitTest]
public class Given_MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo
{
    [Fact]
    public void Then_it_adds_the_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var maatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<TestEvent<MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(maatschappelijkeZetelWerdOvergenomenUitKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                "De locatie met type ‘Maatschappelijke zetel volgens KBO’ kon niet overgenomen worden uit KBO.",
                nameof(MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo),
                maatschappelijkeZetelWerdOvergenomenUitKbo.Data,
                maatschappelijkeZetelWerdOvergenomenUitKbo.Initiator,
                maatschappelijkeZetelWerdOvergenomenUitKbo.Tijdstip.ToBelgianDateAndTime()));
    }
}
