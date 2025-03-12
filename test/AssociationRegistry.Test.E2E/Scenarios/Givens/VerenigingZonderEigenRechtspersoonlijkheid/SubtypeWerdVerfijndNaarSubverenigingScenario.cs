namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Events;
using EventStore;
using Framework.TestClasses;
using Vereniging;

public class SubtypeWerdVerfijndNaarSubverenigingScenario : IScenario
{
    public VzerAndKboVerenigingWerdenGeregistreerdScenario BaseScenario = new();
    public VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging { get; set; }
    private CommandMetadata Metadata;

    public SubtypeWerdVerfijndNaarSubverenigingScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var givenEvents = await BaseScenario.GivenEvents(service);

        VerenigingssubtypeWerdVerfijndNaarSubvereniging = fixture.Create<VerenigingssubtypeWerdVerfijndNaarSubvereniging>() with
        {
            VCode = BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            SubverenigingVan = new Registratiedata.SubverenigingVan(fixture.Create<VCode>(), // because we want to change it to the VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
                                                                    fixture.Create<string>(), // because we want to change it to the VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
                                                                    fixture.Create<string>(), fixture.Create<string>())
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return givenEvents.Append(new KeyValuePair<string, IEvent[]>(VerenigingssubtypeWerdVerfijndNaarSubvereniging.VCode, [VerenigingssubtypeWerdVerfijndNaarSubvereniging]))
                          .ToArray();
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
