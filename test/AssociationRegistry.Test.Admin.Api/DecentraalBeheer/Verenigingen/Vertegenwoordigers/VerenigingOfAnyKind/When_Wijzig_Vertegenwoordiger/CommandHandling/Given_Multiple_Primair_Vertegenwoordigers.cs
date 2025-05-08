namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vertegenwoordigers.WijzigVertegenwoordiger;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Multiple_Primair_Vertegenwoordigers
{
    private readonly WijzigVertegenwoordigerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;

    public Given_Multiple_Primair_Vertegenwoordigers()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigVertegenwoordigerCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_MultiplePrimaryVertegenwoordiger_Is_Thrown()
    {
        var command = new WijzigVertegenwoordigerCommand(
            _scenario.VCode,
            new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
                _scenario.VertegenwoordigerWerdToegevoegd2.VertegenwoordigerId,
                Rol: null,
                Roepnaam: null,
                Email: null,
                Telefoon: null,
                Mobiel: null,
                SocialMedia: null,
                IsPrimair: true)); // <== changed value

        var handle = ()
            => _commandHandler.Handle(new CommandEnvelope<WijzigVertegenwoordigerCommand>(command, _fixture.Create<CommandMetadata>()));

        await handle.Should().ThrowAsync<MeerderePrimaireVertegenwoordigers>();
    }
}
