namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarSubvereniging;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Invalid_Command
{
    [Fact]
    public async Task With_Invalid_VCode_Then_Throws_VCodeFormaatIsOngeldig()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        var rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(scenario.GetVerenigingState());
        verenigingRepositoryMock.WithVereniging(rechtspersoonScenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(null, null, null));

        await Assert.ThrowsAsync<VCodeFormaatIsOngeldig>(() => commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>())));
    }
}
