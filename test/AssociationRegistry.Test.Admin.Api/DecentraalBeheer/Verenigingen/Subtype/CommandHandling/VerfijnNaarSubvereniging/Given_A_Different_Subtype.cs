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
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly MultipleVerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerfijnSubtypeNaarSubverenigingCommandHandler _commandHandler;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _rechtspersoonScenario;

    public Given_A_Different_Subtype()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(_scenario.GetVerenigingState());
        _commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(_verenigingRepositoryMock);
        _rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _verenigingRepositoryMock.WithVereniging(_rechtspersoonScenario.GetVerenigingState());
    }

    [Fact]
    public async ValueTask Then_It_Saves_A_VerenigingssubtypeWerdVerfijndNaarSubvereniging()
    {
        var command = new VerfijnSubtypeNaarSubverenigingCommand(_scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(_rechtspersoonScenario.VCode, null, null));

        await _commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(_scenario.VCode,
            new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
                _scenario.VCode, new Registratiedata.SubverenigingVan(_rechtspersoonScenario.VCode, _rechtspersoonScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam, string.Empty, string.Empty)));
    }

    [Fact]
    public async ValueTask With_Invalid_VCode_Then_Throws_VCodeFormaatIsOngeldig()
    {
       var command = new VerfijnSubtypeNaarSubverenigingCommand(_scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(null, null, null));

        await Assert.ThrowsAsync<VCodeFormaatIsOngeldig>(() => _commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>())));
    }
}
