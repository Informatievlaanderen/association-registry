namespace AssociationRegistry.Test.Acm.Api.Fixtures.Scenarios;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VertegenwoordigerWerdOvergenomenUitKBO VertegenwoordigerWerdOvergenomenUitKBO;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly CommandMetadata Metadata;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003007";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        VertegenwoordigerWerdOvergenomenUitKBO = fixture.Create<VertegenwoordigerWerdOvergenomenUitKBO>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public string Insz
        => VertegenwoordigerWerdOvergenomenUitKBO.Insz;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VertegenwoordigerWerdOvergenomenUitKBO,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
