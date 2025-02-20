namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;

public class V082_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_ForDuplicateForce : IEventsInDbScenario
{
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V082_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_ForDuplicateForce()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999082";
        Naam = "Vereniging 082";

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Contactgegevens = fixture.CreateMany<Registratiedata.Contactgegeven>().Select(
                (contactgegeven, w) => contactgegeven with
                {
                    IsPrimair = w == 0,
                }
            ).ToArray(),
            Vertegenwoordigers = fixture.CreateMany<Registratiedata.Vertegenwoordiger>().Select(
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
        => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
