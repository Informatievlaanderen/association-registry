namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using Admin.MartenDb.VertegenwoordigerPersoonsgegevens;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;
using Persoonsgegevens;
using StubsMocksFakes.VerenigingsRepositories;
using StubsMocksFakes.VertegenwoordigerPersoonsgegevensRepositories;
using Vereniging;
using VertegenwoordigerPersoonsgegevens = Events.VertegenwoordigerPersoonsgegevens;

public abstract class CommandhandlerScenarioBase
{
    protected CommandhandlerScenarioBase()
    {
        Fixture = new Fixture().CustomizeAdminApi();
        VertegenwoordigerPersoonsgegevensRepository = new VertegenwoordigerPersoonsgegevensRepositoryMock();
    }

    public Fixture Fixture { get; set; }
    public abstract VCode VCode { get; }
    public abstract IEnumerable<IEvent> Events();
    public VertegenwoordigerPersoonsgegevensRepositoryMock VertegenwoordigerPersoonsgegevensRepository { get; }

    public VerenigingState GetVerenigingState()
    {
        var verenigingState = new VerenigingState()
        {
            VertegenwoordigerPersoonsgegevensRepository = VertegenwoordigerPersoonsgegevensRepository
        };

        foreach (var evnt in Events())
        {
            if (evnt is VertegenwoordigerWerdToegevoegd vertegenwoordigerWerdToegevoegd)
            {
                VertegenwoordigerPersoonsgegevensRepository.Save(Fixture.Create<AssociationRegistry.Persoonsgegevens.VertegenwoordigerPersoonsgegevens>()
                                                                     with
                                                                     {
                                                                         RefId = vertegenwoordigerWerdToegevoegd.RefId,
                                                                         VertegenwoordigerId = vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                                                                         VCode = VCode,
                                                                     });
            }
            verenigingState = verenigingState.Apply((dynamic)evnt);
        }

        return verenigingState;
    }
}
