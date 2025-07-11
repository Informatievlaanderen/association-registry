﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Removing_Locatie.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Locaties.VerwijderLocatie;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Moq;
using Vereniging.Geotags;
using Xunit;

public class VerenigingMetRechtsPersoonlijkheid_With_One_Locatie
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithALocatieScenario _scenario;
    private Fixture _fixture;

    public VerenigingMetRechtsPersoonlijkheid_With_One_Locatie()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithALocatieScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async ValueTask Then_A_VertegenwoordigerWerdVerwijderd_Event_Is_Saved()
    {
        var command = new VerwijderLocatieCommand(_scenario.VCode, _scenario.LocatieWerdToegevoegd.Locatie.LocatieId);
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var commandHandler = new VerwijderLocatieCommandHandler(_verenigingRepositoryMock, Mock.Of<IGeotagsService>());
        await Assert.ThrowsAsync<LaatsteLocatieKanNietVerwijderdWorden>(async () => await commandHandler.Handle(
                                                                                    new CommandEnvelope<VerwijderLocatieCommand>(
                                                                                        command, commandMetadata),
                                                                                    CancellationToken.None));
    }
}
