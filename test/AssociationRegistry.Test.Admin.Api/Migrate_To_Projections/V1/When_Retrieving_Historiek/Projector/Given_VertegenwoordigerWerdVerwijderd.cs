﻿namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdVerwijderd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var vertegenwoordigerWerdVerwijderd = fixture.Create<TestEvent<VertegenwoordigerWerdVerwijderd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(vertegenwoordigerWerdVerwijderd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Vertegenwoordiger '{vertegenwoordigerWerdVerwijderd.Data.Voornaam} {vertegenwoordigerWerdVerwijderd.Data.Achternaam}' werd verwijderd.",
                nameof(VertegenwoordigerWerdVerwijderd),
                VertegenwoordigerWerdVerwijderdData.Create(vertegenwoordigerWerdVerwijderd.Data),
                vertegenwoordigerWerdVerwijderd.Initiator,
                vertegenwoordigerWerdVerwijderd.Tijdstip.FormatAsZuluTime()));
    }
}
