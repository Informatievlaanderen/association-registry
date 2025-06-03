namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Locaties.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using EventFactories;
using Moq;
using Vereniging.Geotags;
using Xunit;

public class Given_A_MaatschappelijkeZetel
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario _scenario;
    private readonly WijzigMaatschappelijkeZetelCommand _command;
    private GeotagsCollection _geotags;
    private readonly Mock<IGeotagsService> _geotagsService;

    public Given_A_MaatschappelijkeZetel()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var factory = new Faktory(fixture);

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario();
        _verenigingRepositoryMock = factory.VerenigingsRepository.Mock(_scenario.GetVerenigingState());

        (_geotagsService, _geotags) = factory.GeotagsService.ReturnsRandomGeotags();

        _command = new WijzigMaatschappelijkeZetelCommand(
            _scenario.VCode,
            new WijzigMaatschappelijkeZetelCommand.Locatie(
                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
                fixture.Create<bool>(), fixture.Create<string>()));

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigMaatschappelijkeZetelCommandHandler(_verenigingRepositoryMock, _geotagsService.Object);

        commandHandler.Handle(
            new CommandEnvelope<WijzigMaatschappelijkeZetelCommand>(_command, commandMetadata)).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_GeotagService_Is_Called_With_UpdatedLocations()
    {
        _geotagsService.Verify(x => x.CalculateGeotags(It.Is<IEnumerable<Locatie>>(x => x.Count() == 1 &&
                                                                             x.Single().LocatieId == _scenario
                                                                                .MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie
                                                                                .LocatieId),
                                                       Array.Empty<Werkingsgebied>()),
                               Times.Once
        );
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
                _command.TeWijzigenLocatie.IsPrimair!.Value),
            EventFactory.GeotagsWerdenBepaald(_scenario.VCode, _geotags)
        );
    }
}
