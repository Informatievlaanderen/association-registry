namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

public class MaatschappelijkeZetelWerdVerwijderdUitKboScenario : IScenario
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo { get; set; }
    public MaatschappelijkeZetelWerdVerwijderdUitKbo MaatschappelijkeZetelWerdVerwijderdUitKbo { get; set; }

    private CommandMetadata Metadata;

    public MaatschappelijkeZetelWerdVerwijderdUitKboScenario() { }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = await service.GetNext(),
            };

        MaatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();

        MaatschappelijkeZetelWerdVerwijderdUitKbo = fixture.Create<MaatschappelijkeZetelWerdVerwijderdUitKbo>() with
        {
            Locatie = MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                [
                    VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                    MaatschappelijkeZetelWerdOvergenomenUitKbo,
                    MaatschappelijkeZetelWerdVerwijderdUitKbo,
                ]
            ),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata() => Metadata;
}
