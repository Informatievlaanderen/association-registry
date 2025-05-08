namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.When_Wijzig_ContactgegevenFromKbo.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Contactgegevens.WijzigContactgegevenFromKbo;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Xunit;

public class Given_A_Contactgegeven
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario _scenario;
    private readonly WijzigContactgegevenFromKboCommand _command;

    public Given_A_Contactgegeven()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        _command = new WijzigContactgegevenFromKboCommand(_scenario.VCode,
                                                          new WijzigContactgegevenFromKboCommand.CommandContactgegeven(
                                                              _scenario.ContactgegevenWerdOvergenomenUitKBO.ContactgegevenId,
                                                              fixture.Create<string>(), fixture.Create<bool>()));

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigContactgegevenFromKboCommandHandler(_verenigingRepositoryMock);

        commandHandler.Handle(
            new CommandEnvelope<WijzigContactgegevenFromKboCommand>(_command, commandMetadata)).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_MaatschappelijkeZetelVolgensKBOWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenUitKBOWerdGewijzigd(_command.Contactgegeven.ContacgegevenId, _command.Contactgegeven.Beschrijving!,
                                                  _command.Contactgegeven.IsPrimair!.Value)
        );
    }
}
