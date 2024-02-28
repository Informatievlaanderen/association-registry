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
public class With_A_Removed_Contactgegeven
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario _scenario;

    public With_A_Removed_Contactgegeven()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        fixture.Create<string>();

        var verenigingVolgensKbo = _scenario.VerenigingVolgensKbo;

        verenigingVolgensKbo.Contactgegevens = new ContactgegevensVolgensKbo()
        {
            Email = null,
            Website = null,
            Telefoonnummer = null,
            GSM = null,
        };

        var command = new SyncKboCommand(_scenario.KboNummer);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new SyncKboCommandHandler(new MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService(verenigingVolgensKbo));

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
           .ContainSingle(e => e.Equals(new ContactgegevenWerdVerwijderdUitKBO(
                                            _scenario.ContactgegevenWerdOvergenomenUitKBO.ContactgegevenId,
                                            _scenario.ContactgegevenWerdOvergenomenUitKBO.Contactgegeventype,
                                            _scenario.ContactgegevenWerdOvergenomenUitKBO.TypeVolgensKbo,
                                            _scenario.ContactgegevenWerdOvergenomenUitKBO.Waarde)))
           .And
           .ContainSingle(e => e.GetType() == typeof(KboSyncSuccessful));
    }
}
