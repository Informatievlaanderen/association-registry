﻿namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;

public class V009_VerenigingWerdGeregistreerd_ForDuplicateForce : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V009_VerenigingWerdGeregistreerd_ForDuplicateForce()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999009";
        Naam = "Vereniging 009";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Contactgegevens = fixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>().Select(
                (contactgegeven, w) => contactgegeven with
                {
                    IsPrimair = w == 0,
                }
            ).ToArray(),
            Vertegenwoordigers = fixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>().Select(
                (vertegenwoordiger, i) => vertegenwoordiger with
                {
                    IsPrimair = i == 0,
                }).ToArray(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public DateOnly? Startdatum
        => FeitelijkeVerenigingWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
