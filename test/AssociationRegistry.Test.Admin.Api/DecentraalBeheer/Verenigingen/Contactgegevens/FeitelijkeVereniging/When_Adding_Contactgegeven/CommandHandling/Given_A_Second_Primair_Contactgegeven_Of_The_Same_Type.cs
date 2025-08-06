namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Adding_Contactgegeven.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Acties.Contactgegevens.VoegContactgegevenToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class Given_A_Second_Primair_Contactgegeven_Of_The_Same_Type
{
    private readonly VoegContactgegevenToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;

    public Given_A_Second_Primair_Contactgegeven_Of_The_Same_Type()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegContactgegevenToeCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_MultiplePrimaryContactgegevens_Is_Thrown()
    {
        var command = new VoegContactgegevenToeCommand(
            _scenario.VCode,
            Contactgegeven.Create(
                Contactgegeventype.Labels.Email,
                waarde: "test2@example.org",
                _fixture.Create<string?>(),
                isPrimair: true));

        var handleCall = async ()
            => await _commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        await handleCall.Should()
                        .ThrowAsync<MeerderePrimaireContactgegevensZijnNietToegestaan>()
                        .WithMessage(new MeerderePrimaireContactgegevensZijnNietToegestaan(Contactgegeventype.Email.ToString()).Message);
    }
}
