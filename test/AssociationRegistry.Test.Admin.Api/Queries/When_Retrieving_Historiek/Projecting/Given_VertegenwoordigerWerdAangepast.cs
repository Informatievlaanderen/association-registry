<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_VertegenwoordigerWerdAangepast.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_VertegenwoordigerWerdAangepast.cs

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
public class Given_VertegenwoordigerWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_vertegenwoordiger_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var vertegenwoordigerWerdGewijzigd = fixture.Create<TestEvent<VertegenwoordigerWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(vertegenwoordigerWerdGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Vertegenwoordiger '{vertegenwoordigerWerdGewijzigd.Data.Voornaam} {vertegenwoordigerWerdGewijzigd.Data.Achternaam}' werd gewijzigd.",
                nameof(VertegenwoordigerWerdGewijzigd),
                vertegenwoordigerWerdGewijzigd.Data,
                vertegenwoordigerWerdGewijzigd.Initiator,
                vertegenwoordigerWerdGewijzigd.Tijdstip.ToZuluTime()));
    }
}
