namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarSubvereniging;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.Subvereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

/// <summary>
/// SubverenigingDetailsWerdenGewijzigd only changes identificatie and beschrijving: the koepel stays.
/// </summary>
public class Given_SubverenigingDetailsWerdenGewijzigd
{
    private readonly Fixture _fixture;
    private readonly VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario _scenario;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _koepelScenario;
    private readonly SubverenigingRelatieWerdGewijzigd _relatieWerdGewijzigd;
    private readonly SubverenigingDetailsWerdenGewijzigd _detailsWerdenGewijzigd;
    private readonly VerenigingState _state;

    public Given_SubverenigingDetailsWerdenGewijzigd()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario();
        _koepelScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _relatieWerdGewijzigd = new SubverenigingRelatieWerdGewijzigd(
            _scenario.VCode,
            _koepelScenario.VCode,
            _koepelScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam
        );

        _detailsWerdenGewijzigd = new SubverenigingDetailsWerdenGewijzigd(
            _scenario.VCode,
            _fixture.Create<string>(),
            _fixture.Create<string>()
        );

        // relatie and details changed in one command: SubverenigingVan.Wijzig emits the relatie first
        _state = _scenario.GetVerenigingState().Apply(_relatieWerdGewijzigd).Apply(_detailsWerdenGewijzigd);
    }

    [Fact]
    public void Then_The_State_Keeps_The_Koepel()
    {
        _state
            .Verenigingssubtype.Should()
            .Be(
                new SubverenigingSubtype(
                    new SubverenigingVan
                    {
                        AndereVereniging = _relatieWerdGewijzigd.AndereVereniging,
                        AndereVerenigingNaam = _relatieWerdGewijzigd.AndereVerenigingNaam,
                        Identificatie = _detailsWerdenGewijzigd.Identificatie,
                        Beschrijving = _detailsWerdenGewijzigd.Beschrijving,
                    }
                )
            );
    }

    [Fact]
    public async ValueTask With_The_Same_Koepel_Then_Nothing()
    {
        var aggregateSessionMock = new MultipleVerenigingAggregateSessionMock(_state);
        aggregateSessionMock.WithVereniging(_koepelScenario.GetVerenigingState());

        var command = new VerfijnSubtypeNaarSubverenigingCommand(
            _scenario.VCode,
            new SubverenigingVanDto(_koepelScenario.VCode, null, null)
        );

        await new VerfijnSubtypeNaarSubverenigingCommandHandler(aggregateSessionMock).Handle(
            new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>())
        );

        aggregateSessionMock.ShouldNotHaveAnySaves(_scenario.VCode);
    }
}
