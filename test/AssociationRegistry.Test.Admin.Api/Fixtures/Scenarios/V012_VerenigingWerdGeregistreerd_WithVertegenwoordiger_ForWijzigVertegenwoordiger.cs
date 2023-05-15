﻿namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;

public class V012_VerenigingWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V012_VerenigingWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999012";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Vertegenwoordigers = fixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>().Select(
                (vertegenwoordiger, w) => vertegenwoordiger with
                {
                    IsPrimair = w == 0,
                }
            ).ToArray(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public DateOnly? Startdatum
        => FeitelijkeVerenigingWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
