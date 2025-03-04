namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.TeWijzigenNaarFeitelijkeVereniging;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Reeds_FeitelijkeVereniging_As_Subtype
{
    [Fact]
    public async Task Then_No_Event_Is_Saved()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new WijzigSubtypeCommandHandler(verenigingRepositoryMock);

        var command = new WijzigSubtypeCommand(
            VCode: scenario.VCode,
            SubtypeData: new WijzigSubtypeCommand.TeWijzigenNaarFeitelijkeVereniging(
                new Subtype(
                    Subtype.FeitelijkeVereniging.Code,
                    Subtype.FeitelijkeVereniging.Naam)));

        await commandHandler.Handle(new CommandEnvelope<WijzigSubtypeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldNotHaveSaved<SubtypeWerdVerfijndNaarFeitelijkeVereniging>();
    }
}
