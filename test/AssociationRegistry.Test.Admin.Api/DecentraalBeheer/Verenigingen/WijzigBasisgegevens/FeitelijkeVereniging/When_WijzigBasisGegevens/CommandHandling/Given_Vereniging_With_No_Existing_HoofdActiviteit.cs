﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Basisgegevens.FeitelijkeVereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Vereniging_With_No_Existing_HoofdActiviteit
{
    private FeitelijkeVerenigingWerdGeregistreerdWithNoHoofdActiviteitScenario _scenario;
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private Fixture _fixture;

    public Given_Vereniging_With_No_Existing_HoofdActiviteit()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithNoHoofdActiviteitScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task WithEmptyHoofdActiviteitenRequest_ThenNothing()
    {
        var command = new WijzigBasisgegevensCommand(_scenario.VCode,
                                                     HoofdactiviteitenVerenigingsloket: Array.Empty<HoofdactiviteitVerenigingsloket>());

        var commandHandler = new WijzigBasisgegevensCommandHandler();
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock,
            new ClockStub(_fixture.Create<DateOnly>()));

        _verenigingRepositoryMock.ShouldNotHaveSaved<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>();
    }
}
