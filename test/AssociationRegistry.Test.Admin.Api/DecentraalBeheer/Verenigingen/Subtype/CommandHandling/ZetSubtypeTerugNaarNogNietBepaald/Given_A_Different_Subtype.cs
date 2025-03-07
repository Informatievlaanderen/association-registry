namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.ZetSubtypeTerugNaarNogNietBepaald;

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
    public async Task Then_It_Saves_A_SubtypeWerdTerugGezetNaarNogNietBepaald()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new ZetSubtypeTerugNaarNogNietBepaaldCommandHandler(verenigingRepositoryMock);

        var command = new ZetSubtypeTerugNaarNogNietBepaaldCommand(scenario.VCode);

        await commandHandler.Handle(new CommandEnvelope<ZetSubtypeTerugNaarNogNietBepaaldCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new SubtypeWerdTerugGezetNaarNogNietBepaald(
                scenario.VCode,
                new Registratiedata.Subtype(Verenigingssubtype.NogNietBepaald.Code,
                                            Verenigingssubtype.NogNietBepaald.Naam)));
    }
}
