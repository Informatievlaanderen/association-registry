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
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_New_But_Invalid_Contactgegeven
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly string _newContactgegevenWaarde;

    public With_A_New_But_Invalid_Contactgegeven()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        _newContactgegevenWaarde = fixture.Create<string>();

        var verenigingVolgensKbo = _scenario.VerenigingVolgensKbo;

        verenigingVolgensKbo.Contactgegevens = new ContactgegevensVolgensKbo()
        {
            Email = _newContactgegevenWaarde,
            Website = null,
            Telefoonnummer = null,
            GSM = null,
        };

        var command = new SyncKboCommand(_scenario.KboNummer);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new SyncKboCommandHandler(new MagdaGeefVerenigingNumberFoundServiceMock(verenigingVolgensKbo));

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
    public void Then_A_ContactgegevenWerdVerwijderd_And_ContactgegevenKonNietOvergenomenWorden_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .HaveCount(2)
           .And
           .ContainSingle(e => e.Equals(new ContactgegevenKonNietOvergenomenWordenUitKBO(
                                            Contactgegeventype.Email,
                                            ContactgegeventypeVolgensKbo.Email,
                                            _newContactgegevenWaarde)))
           .And
           .ContainSingle(e => e.GetType() == typeof(SynchronisatieMetKboWasSuccesvol));
    }
}
