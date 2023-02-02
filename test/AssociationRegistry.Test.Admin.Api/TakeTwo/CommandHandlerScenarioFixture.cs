﻿namespace AssociationRegistry.Test.Admin.Api.TakeTwo;

using Vereniging;
using VerenigingRepositoryMock = Fakes.VerenigingRepositoryMock;

public class CommandHandlerScenarioFixture<TScenario> where TScenario: class, ICommandhandlerScenario, new()
{
    public readonly Vereniging Vereniging;
    public readonly TScenario Scenario;

    public VerenigingRepositoryMock VerenigingRepositoryMock
        => new(Vereniging);

    public CommandHandlerScenarioFixture()
    {
        Vereniging = new Vereniging();
        Scenario = new TScenario();

        foreach (var evnt in Scenario.Events())
        {
            Vereniging.Apply((dynamic)evnt);
        }
    }
}
