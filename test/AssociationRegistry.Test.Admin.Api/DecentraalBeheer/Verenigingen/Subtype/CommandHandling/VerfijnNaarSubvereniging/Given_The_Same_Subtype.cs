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
using Vereniging.Subtypes.Subvereniging;
using Xunit;

/// <summary>
/// This will result in a te wijzigen subtype
/// </summary>
public class Given_The_Same_Subtype
{
    private readonly Fixture _fixture;
    private readonly VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario _scenario;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _rechtspersoonScenario;
    private readonly VerfijnSubtypeNaarSubverenigingCommandHandler _commandHandler;
    private readonly MultipleVerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_The_Same_Subtype()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario();
        _rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(_scenario.GetVerenigingState());
        _verenigingRepositoryMock.WithVereniging(_rechtspersoonScenario.GetVerenigingState());
        _commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask With_Relatie_Changes_Then_SubverenigingRelatieWerdGewijzigd()
    {
        var command = new VerfijnSubtypeNaarSubverenigingCommand(_scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(_rechtspersoonScenario.VCode, null, null));

        await _commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(_scenario.VCode,
                                                 new SubverenigingRelatieWerdGewijzigd(
                                                     _scenario.VCode, _rechtspersoonScenario.VCode, _rechtspersoonScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam));
    }

    [Fact]
    public async ValueTask With_Detail_Changes_Then_SubverenigingDetailsWerdenGewijzigd()
    {
        var subtypeIdentificatie = SubverenigingIdentificatie.Create(_fixture.Create<string>());
        var subtypeBeschrijving = SubverenigingBeschrijving.Create(_fixture.Create<string>());

        var command = new VerfijnSubtypeNaarSubverenigingCommand(_scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(null, subtypeIdentificatie, subtypeBeschrijving));

        await _commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(_scenario.VCode,
                                                 new SubverenigingDetailsWerdenGewijzigd(
                                                     _scenario.VCode, subtypeIdentificatie, subtypeBeschrijving));
    }

    [Fact]
    public async ValueTask With_No_Changes_Then_Nothing()
    {
       var command = new VerfijnSubtypeNaarSubverenigingCommand(_scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(null, SubverenigingIdentificatie.Create(_scenario.VerenigingssubtypeWerdVerfijndNaarSubvereniging.SubverenigingVan.Identificatie), null));

        await _commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldNotHaveAnySaves(_scenario.VCode);
    }

    [Fact]
    public async ValueTask With_All_Null_Values_Then_Throw_WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben()
    {
        var command = new VerfijnSubtypeNaarSubverenigingCommand(_scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(null, null, null));

        var exception = await Assert.ThrowsAsync<WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben>(() => _commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>())));
        exception.Message.Should().Be(ExceptionMessages.WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben);
    }
}
