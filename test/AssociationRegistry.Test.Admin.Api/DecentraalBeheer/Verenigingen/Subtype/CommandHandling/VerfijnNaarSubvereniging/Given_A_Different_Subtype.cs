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

/// <summary>
/// This wil result in a verfijn naar subvereniging
/// </summary>
[UnitTest]
public class Given_A_Different_Subtype
{
    [Fact]
    public async Task Then_It_Saves_A_VerenigingssubtypeWerdVerfijndNaarSubvereniging()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        var rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(scenario.GetVerenigingState());
        verenigingRepositoryMock.WithVereniging(rechtspersoonScenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(rechtspersoonScenario.VCode, null, null));

        await commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(scenario.VCode,
            new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
                scenario.VCode, new Registratiedata.SubverenigingVan(rechtspersoonScenario.VCode, rechtspersoonScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam, string.Empty, string.Empty)));
    }

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
