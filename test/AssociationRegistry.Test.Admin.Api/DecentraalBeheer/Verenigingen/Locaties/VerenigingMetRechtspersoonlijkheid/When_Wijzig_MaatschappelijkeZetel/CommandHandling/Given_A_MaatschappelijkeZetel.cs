namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Locaties.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Xunit;

public class Given_A_MaatschappelijkeZetel
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario _scenario;
    private readonly WijzigMaatschappelijkeZetelCommand _command;

    public Given_A_MaatschappelijkeZetel()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        _command = new WijzigMaatschappelijkeZetelCommand(
            _scenario.VCode,
            new WijzigMaatschappelijkeZetelCommand.Locatie(
                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
                fixture.Create<bool>(), fixture.Create<string>()));

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigMaatschappelijkeZetelCommandHandler(_verenigingRepositoryMock);

        commandHandler.Handle(
            new CommandEnvelope<WijzigMaatschappelijkeZetelCommand>(_command, commandMetadata)).GetAwaiter().GetResult();
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
            new MaatschappelijkeZetelVolgensKBOWerdGewijzigd(
                _command.TeWijzigenLocatie.LocatieId,
                _command.TeWijzigenLocatie.Naam!,
                _command.TeWijzigenLocatie.IsPrimair!.Value)
        );
    }
}
