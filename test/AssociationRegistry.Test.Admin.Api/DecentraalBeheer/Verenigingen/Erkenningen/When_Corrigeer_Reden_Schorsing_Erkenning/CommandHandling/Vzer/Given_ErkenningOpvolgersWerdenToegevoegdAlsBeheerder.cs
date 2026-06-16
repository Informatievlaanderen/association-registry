namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerSchorsingErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AssociationRegistry.Test.Common.StubsMocksFakes.Wegwijs;
using AutoFixture;
using Xunit;

public class Given_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder
{
    private readonly CorrigeerRedenSchorsingErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpGeschorsteErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;
    private CorrigeerRedenSchorsingErkenningCommand _command;
    private CommandMetadata? _commandMetaData;

    public Given_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpGeschorsteErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new CorrigeerRedenSchorsingErkenningCommandHandler(_verenigingRepositoryMock);

        var teSchorsenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        _command = _fixture.Create<CorrigeerRedenSchorsingErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeCorrigerenRedenSchorsingErkenning>() with
            {
                ErkenningId = teSchorsenErkenningId,
            },
        };

        _commandMetaData = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningOpvolgersWerdenToegevoegdAlsBeheerder.Beheerders.First(),
        };
    }

    [Fact]
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_ErkenningRedenVanSchorsingWerdGecorrigeerd()
    {
        var organisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub().WithGemachtigdeOrganisaties([
            _commandMetaData.Initiator,
        ]);

        await _commandHandler.Handle(
            new CommandEnvelope<CorrigeerRedenSchorsingErkenningCommand>(_command, _commandMetaData),
            organisatieBevoegdheidService.Object
        );

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningRedenVanSchorsingWerdGecorrigeerd(
                _command.Erkenning.ErkenningId,
                _command.Erkenning.RedenSchorsing
            )
        );
    }
}
