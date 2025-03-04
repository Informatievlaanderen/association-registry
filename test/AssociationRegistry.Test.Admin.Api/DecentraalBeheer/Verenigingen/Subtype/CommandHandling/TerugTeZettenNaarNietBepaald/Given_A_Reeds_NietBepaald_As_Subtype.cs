namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.TerugTeZettenNaarNietBepaald;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Reeds_FeitelijkeVereniging_As_Subtype
{
    [Fact]
    public async Task Then_No_Event_Is_Saved()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new WijzigSubtypeCommandHandler(verenigingRepositoryMock);

        var command = new WijzigSubtypeCommand(
            VCode: scenario.VCode,
            SubtypeData: new WijzigSubtypeCommand.TerugTeZettenNaarNogNietBepaald(
                new Subtype(
                    Subtype.NogNietBepaald.Code,
                    Subtype.NogNietBepaald.Naam)));

        await commandHandler.Handle(new CommandEnvelope<WijzigSubtypeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldNotHaveSaved<SubtypeWerdTerugGezetNaarNogNietBepaald>();
    }
}
