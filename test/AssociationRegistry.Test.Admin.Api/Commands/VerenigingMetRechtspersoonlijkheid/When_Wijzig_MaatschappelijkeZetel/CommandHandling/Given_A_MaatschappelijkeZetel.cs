namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.CommandHandling;

using Acties.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
