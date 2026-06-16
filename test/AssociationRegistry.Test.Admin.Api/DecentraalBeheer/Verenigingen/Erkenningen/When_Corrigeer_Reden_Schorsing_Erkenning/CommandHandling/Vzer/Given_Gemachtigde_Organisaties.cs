namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerSchorsingErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.Wegwijs;
using Events;
using Xunit;

public class Given_Gemachtigde_Organisaties
{
    private readonly CorrigeerRedenSchorsingErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;
    private CorrigeerRedenSchorsingErkenningCommand _command;
    private CommandMetadata? _commandMetaData;

    public Given_Gemachtigde_Organisaties()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario();
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

        _commandMetaData = _fixture.Create<CommandMetadata>();
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
            new ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(
                _scenario.ErkenningWerdGeregistreerd.ErkenningId,
                [_commandMetaData.Initiator]
            ),
            new ErkenningRedenVanSchorsingWerdGecorrigeerd(
                _command.Erkenning.ErkenningId,
                _command.Erkenning.RedenSchorsing
            )
        );
    }
}
