namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarSubvereniging;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AutoFixture;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Different_Subtype
{
    [Fact]
    public async Task Then_It_Saves_A_SubtypeWerdVerfijndNaarSubvereniging()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(
            scenario.VCode,
            new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(
                fixture.Create<VCode>(),
                SubtypeIdentificatie.Create(fixture.Create<string>()),
                SubtypeBeschrijving.Create(fixture.Create<string>())
            ));

        await commandHandler.Handle(
            new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
                scenario.VCode,
                new Registratiedata.SubverenigingVan(
                    command.SubverenigingVan.AndereVereniging!,
                    command.SubverenigingVan.AndereVerenigingNaam!,
                    command.SubverenigingVan.Identificatie!,
                    command.SubverenigingVan.Beschrijving!)
            ));
    }
}
