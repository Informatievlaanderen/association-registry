namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_SyncKbo.CommandHandling;

using Acties.SyncKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using FluentAssertions;
using Framework;
using Kbo;
using Moq;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_No_Changes
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly Mock<IMagdaGeefVerenigingService> _magdaGeefVerenigingService;

    public With_No_Changes()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _magdaGeefVerenigingService = new Mock<IMagdaGeefVerenigingService>();

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new SyncKboCommand(KboNummer.Create(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer));
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new SyncKboCommandHandler(new MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService(
                                                           _scenario.VerenigingVolgensKbo
                                                       ));

        commandHandler.Handle(
            new CommandEnvelope<SyncKboCommand>(command, commandMetadata),
            _verenigingRepositoryMock).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.KboNummer);
    }

    [Fact]
    public void Then_Only_A_KboSyncSuccessful_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .HaveCount(1)
           .And
           .ContainSingle(e => e.GetType() == typeof(SynchronisatieMetKboWasSuccesvol));

    }
}
