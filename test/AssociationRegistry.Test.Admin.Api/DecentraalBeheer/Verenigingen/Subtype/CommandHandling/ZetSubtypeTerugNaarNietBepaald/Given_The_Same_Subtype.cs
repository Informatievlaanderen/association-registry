namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.ZetSubtypeTerugNaarNietBepaald;

using AssociationRegistry.DecentraalBeheer.Subtype;
using Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_The_Same_Subtype
{
    [Fact]
    public async Task Then_No_Event_Is_Saved()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new ZetSubtypeTerugNaarNietBepaaldCommandHandler(verenigingRepositoryMock);

        var command = new ZetSubtypeTerugNaarNietBepaaldCommand(scenario.VCode);

        await commandHandler.Handle(new CommandEnvelope<ZetSubtypeTerugNaarNietBepaaldCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(new VerenigingssubtypeWerdTerugGezetNaarNietBepaald(scenario.VCode));
    }
}
