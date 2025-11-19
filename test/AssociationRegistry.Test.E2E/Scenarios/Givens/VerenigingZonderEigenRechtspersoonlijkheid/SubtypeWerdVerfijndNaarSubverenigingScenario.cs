namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

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
            VCode = BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            SubverenigingVan = new Registratiedata.SubverenigingVan(fixture.Create<VCode>(), // because we want to change it to the VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
                                                                    fixture.Create<string>(), // because we want to change it to the VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
                                                                    fixture.Create<string>(), fixture.Create<string>())
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return [
            new(BaseScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode, [BaseScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd]),
            new(BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, [BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VerenigingssubtypeWerdVerfijndNaarSubvereniging]),
        ];
    }

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => BaseScenario.GivenVertegenwoordigerPersoonsgegevens();

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
