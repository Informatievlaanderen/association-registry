namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarFeitelijkeVereniging;

using AssociationRegistry.DecentraalBeheer.Subtype;
using Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Vereniging;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Different_Subtype
{
    [Fact]
    public async Task Then_It_Saves_A_SubtypeWerdVerfijndNaarFeitelijkeVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarFeitelijkeVerenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarFeitelijkeVerenigingCommand(scenario.VCode);

        await commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarFeitelijkeVerenigingCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(
                scenario.VCode,
                new Registratiedata.Subtype(Verenigingssubtype.FeitelijkeVereniging.Code,
                                            Verenigingssubtype.FeitelijkeVereniging.Naam)));
    }
}
