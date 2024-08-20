<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_VertegenwoordigerWerdToegevoegd.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_VertegenwoordigerWerdToegevoegd.cs

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_vertegenwoordiger_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var vertegenwoordigerWerdToegevoegd = fixture.Create<TestEvent<VertegenwoordigerWerdToegevoegd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(vertegenwoordigerWerdToegevoegd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"'{vertegenwoordigerWerdToegevoegd.Data.Voornaam} {vertegenwoordigerWerdToegevoegd.Data.Achternaam}' werd toegevoegd als vertegenwoordiger.",
                nameof(VertegenwoordigerWerdToegevoegd),
                VertegenwoordigerData.Create(vertegenwoordigerWerdToegevoegd.Data),
                vertegenwoordigerWerdToegevoegd.Initiator,
                vertegenwoordigerWerdToegevoegd.Tijdstip.ToZuluTime()));
    }
}

[UnitTest]
public class Given_VertegenwoordigerWerdOvergenomenUitKBO
{
    [Fact]
    public void Then_it_adds_the_vertegenwoordiger_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var vertegenwoordigerWerdOvergenomenUitKbo = fixture.Create<TestEvent<VertegenwoordigerWerdOvergenomenUitKBO>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(vertegenwoordigerWerdOvergenomenUitKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Vertegenwoordiger '{vertegenwoordigerWerdOvergenomenUitKbo.Data.Voornaam} {vertegenwoordigerWerdOvergenomenUitKbo.Data.Achternaam}' werd overgenomen uit KBO.",
                nameof(VertegenwoordigerWerdOvergenomenUitKBO),
                VertegenwoordigerData.Create(vertegenwoordigerWerdOvergenomenUitKbo.Data),
                vertegenwoordigerWerdOvergenomenUitKbo.Initiator,
                vertegenwoordigerWerdOvergenomenUitKbo.Tijdstip.ToZuluTime()));
    }
}
