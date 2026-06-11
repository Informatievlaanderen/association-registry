namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using global::AutoFixture;
using AssociationRegistry.Framework;
using AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;
using EventStore;

public class V084_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithActieveErkenning_ForSearchOnErkenning
    : IEventsInDbScenario
{
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ActieveErkenningWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V084_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithActieveErkenning_ForSearchOnErkenning()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999084";

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        ActieveErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Actief.Value,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public IEvent[] GetEvents() =>
        new IEvent[] { VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, ActieveErkenningWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata() => Metadata;
}
