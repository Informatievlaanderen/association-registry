namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarSubvereniging;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

/// <summary>
/// This will result in a te wijzigen subtype
/// </summary>
[UnitTest]
public class Given_A_The_Same_Subtype
{
    [Fact]
    public async Task With_Relatie_Changes_Then_SubverenigingRelatieWerdGewijzigd()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario();
        var rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(scenario.GetVerenigingState());
        verenigingRepositoryMock.WithVereniging(rechtspersoonScenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(rechtspersoonScenario.VCode, null, null));

        await commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(scenario.VCode,
                                                 new SubverenigingRelatieWerdGewijzigd(
                                                     scenario.VCode, rechtspersoonScenario.VCode, rechtspersoonScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam));
    }

    [Fact]
    public async Task With_Detail_Changes_Then_SubverenigingDetailsWerdenGewijzigd()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario();
        var rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(scenario.GetVerenigingState());
        verenigingRepositoryMock.WithVereniging(rechtspersoonScenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(verenigingRepositoryMock);

        var subtypeIdentificatie = SubtypeIdentificatie.Create(fixture.Create<string>());
        var subtypeBeschrijving = SubtypeBeschrijving.Create(fixture.Create<string>());

        var command = new VerfijnSubtypeNaarSubverenigingCommand(scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(null, subtypeIdentificatie, subtypeBeschrijving));

        await commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(scenario.VCode,
                                                 new SubverenigingDetailsWerdenGewijzigd(
                                                     scenario.VCode, subtypeIdentificatie, subtypeBeschrijving));
    }

    [Fact]
    public async Task With_No_Changes_Then_Nothing()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario();
        var rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(scenario.GetVerenigingState());
        verenigingRepositoryMock.WithVereniging(rechtspersoonScenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(null, SubtypeIdentificatie.Create(scenario.VerenigingssubtypeWerdVerfijndNaarSubvereniging.SubverenigingVan.Identificatie), null));

        await commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldNotHaveAnySaves(scenario.VCode);
    }

    [Fact]
    public async Task With_All_Null_Values_Then_Throw_WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario();
        var rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(scenario.GetVerenigingState());
        verenigingRepositoryMock.WithVereniging(rechtspersoonScenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(null, null, null));

        var exception = await Assert.ThrowsAsync<WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben>(() => commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>())));
        exception.Message.Should().Be(ExceptionMessages.WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben);
    }
}
