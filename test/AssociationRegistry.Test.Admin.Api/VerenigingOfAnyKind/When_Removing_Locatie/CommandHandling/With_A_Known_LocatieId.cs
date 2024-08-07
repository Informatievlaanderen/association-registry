﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Removing_Locatie.CommandHandling;

using Acties.VerwijderLocatie;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Known_LocatieId
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario _scenario;

    public With_A_Known_LocatieId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new VerwijderLocatieCommand(_scenario.VCode, _scenario.LocatieWerdToegevoegd.Locatie.LocatieId);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new VerwijderLocatieCommandHandler(_verenigingRepositoryMock);

        commandHandler.Handle(new CommandEnvelope<VerwijderLocatieCommand>(command, commandMetadata))
                      .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingOfAnyKind>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_VertegenwoordigerWerdVerwijderd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new LocatieWerdVerwijderd(
                _scenario.VCode, _scenario.LocatieWerdToegevoegd.Locatie)
        );
    }
}
