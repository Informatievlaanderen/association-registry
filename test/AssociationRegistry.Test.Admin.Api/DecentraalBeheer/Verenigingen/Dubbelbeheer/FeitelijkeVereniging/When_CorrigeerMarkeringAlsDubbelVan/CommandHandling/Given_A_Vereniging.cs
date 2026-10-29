namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_CorrigeerMarkeringAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.CorrigeerMarkeringAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Xunit;
using VerenigingStatus = AssociationRegistry.Admin.Schema.Constants.VerenigingStatus;

public class Given_A_Vereniging
{
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGemarkeerdAlsDubbelVanScenario _scenario;
    private readonly MultipleVerenigingAggregateSessionMock _aggregateSessionMock;
    private readonly CorrigeerMarkeringAlsDubbelVanCommandHandler _commandHandler;
    private readonly VCode _vCodeAuthentiekeVereniging;

    public Given_A_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingWerdGemarkeerdAlsDubbelVanScenario();
        _vCodeAuthentiekeVereniging = VCode.Create(
            _scenario.VerenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging
        );

        var authentiekeVereniging = new VerenigingState()
            .Apply(
                _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
                {
                    VCode = _vCodeAuthentiekeVereniging,
                }
            )
            .Apply(new VerenigingAanvaarddeDubbeleVereniging(_vCodeAuthentiekeVereniging, _scenario.VCode));

        _aggregateSessionMock = new MultipleVerenigingAggregateSessionMock(
            _scenario.GetVerenigingState(),
            expectedLoadingDubbel: true
        );
        _aggregateSessionMock.WithVereniging(authentiekeVereniging);

        _commandHandler = new CorrigeerMarkeringAlsDubbelVanCommandHandler(_aggregateSessionMock);
    }

    private async Task Handle() =>
        await _commandHandler.Handle(
            new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(
                _fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with
                {
                    VCode = _scenario.VCode,
                },
                _fixture.Create<CommandMetadata>()
            )
        );

    [Fact]
    public async ValueTask Then_It_Saves_A_MarkeringDubbeleVerengingWerdGecorrigeerd_Event()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSaved(
            _scenario.VCode,
            new MarkeringDubbeleVerengingWerdGecorrigeerd(
                _scenario.VCode,
                _vCodeAuthentiekeVereniging,
                VerenigingStatus.Actief
            )
        );
    }

    [Fact]
    public async ValueTask Then_The_Authentieke_Vereniging_Aanvaardt_The_Correctie()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSaved(
            _vCodeAuthentiekeVereniging,
            new VerenigingAanvaarddeCorrectieDubbeleVereniging(_vCodeAuthentiekeVereniging, _scenario.VCode)
        );
    }

    [Fact]
    public async ValueTask Then_Both_Verenigingen_Are_Saved_In_One_Transaction()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSavedTransactional(_scenario.VCode, _vCodeAuthentiekeVereniging);
    }
}
