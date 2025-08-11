namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using AssociationRegistry.Primitives;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Moq;
using Xunit;

public class With_A_Startdatum_In_The_Future
{
    private readonly CommandEnvelope<WijzigBasisgegevensCommand> _commandEnvelope;
    private readonly WijzigBasisgegevensCommandHandler _commandHandler;
    private readonly VerenigingRepositoryMock _repositoryMock;

    public With_A_Startdatum_In_The_Future()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var command = fixture.Create<WijzigBasisgegevensCommand>() with
        {
            Startdatum = NullOrEmpty<Datum>.Create(fixture.Create<Datum>()),
        };

        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new WijzigBasisgegevensCommandHandler(Mock.Of<IGeotagsService>());
        _commandEnvelope = new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata);
    }

    [Fact]
    public async ValueTask Then_it_throws_an_StartdatumIsInFutureException()
    {
        var method = () => _commandHandler.Handle(
            _commandEnvelope,
            _repositoryMock,
            new ClockStub(_commandEnvelope.Command.Startdatum.Value!.Value.AddDays(-1)));

        await method.Should().ThrowAsync<StartdatumMagNietInToekomstZijn>();
    }
}
