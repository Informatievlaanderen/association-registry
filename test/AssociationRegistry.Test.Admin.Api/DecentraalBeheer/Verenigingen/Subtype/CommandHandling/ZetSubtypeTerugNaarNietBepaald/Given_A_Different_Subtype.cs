namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.ZetSubtypeTerugNaarNietBepaald;

using AssociationRegistry.DecentraalBeheer.Subtype;
using Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Vereniging;
using AutoFixture;
using Xunit;

public class Given_A_Different_Subtype
{
    [Fact]
    public async ValueTask Then_It_Saves_A_SubtypeWerdTerugGezetNaarNietBepaald()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new ZetSubtypeTerugNaarNietBepaaldCommandHandler(verenigingRepositoryMock);

        var command = new ZetSubtypeTerugNaarNietBepaaldCommand(scenario.VCode);

        await commandHandler.Handle(
            new CommandEnvelope<ZetSubtypeTerugNaarNietBepaaldCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(new VerenigingssubtypeWerdTerugGezetNaarNietBepaald(scenario.VCode));
    }
}
