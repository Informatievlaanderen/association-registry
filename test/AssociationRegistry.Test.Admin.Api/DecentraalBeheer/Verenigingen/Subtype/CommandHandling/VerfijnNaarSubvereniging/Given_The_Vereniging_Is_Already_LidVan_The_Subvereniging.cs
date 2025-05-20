namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarSubvereniging;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Vereniging.Exceptions;
using Xunit;

public class Given_The_Vereniging_Is_Already_LidVan_The_Subvereniging
{
    [Fact]
    public async ValueTask For_Verfijnen_Naar_Subvereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new VerenigingKanGeenSubverenigingWordenWaarvanHijAlReedsLidIsScenario.VoorTeVerfijenSubverenigingScenario();
        var rechtspersoonScenario = new VerenigingKanGeenSubverenigingWordenWaarvanHijAlReedsLidIsScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(scenario.GetVerenigingState());
        verenigingRepositoryMock.WithVereniging(rechtspersoonScenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(rechtspersoonScenario.VCode, null, null));

        var exception = await Assert.ThrowsAnyAsync<VerenigingKanGeenSubverenigingWordenWaarvanZijAlReedsLidIs>(() => commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>())));
        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenSubverenigingWordenWaarvanZijAlReedsLidIs);
    }

    [Fact]
    public async ValueTask For_Wijzig_Subvereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new VerenigingKanGeenSubverenigingWordenWaarvanHijAlReedsLidIsScenario.VoorTeWijzigenSubverenigingScenario();
        var rechtspersoonScenario = new VerenigingKanGeenSubverenigingWordenWaarvanHijAlReedsLidIsScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(scenario.GetVerenigingState());
        verenigingRepositoryMock.WithVereniging(rechtspersoonScenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(rechtspersoonScenario.VCode, null, null));

        var exception = await Assert.ThrowsAnyAsync<VerenigingKanGeenSubverenigingWordenWaarvanZijAlReedsLidIs>(() => commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>())));
        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenSubverenigingWordenWaarvanZijAlReedsLidIs);
    }
}
