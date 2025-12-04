namespace AssociationRegistry.Test.Middleware;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using CommandHandling.DecentraalBeheer.Middleware;
using Common.AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

public class GeefPersonenMiddlewareTests
{
    private readonly Fixture _fixture;
    private readonly CommandMetadata _commandMetadata;
    private readonly Mock<IGeefPersoonService> _geefPersoonsService;

    public GeefPersonenMiddlewareTests()
    {
        _fixture = new Fixture().CustomizeDomain();
        _commandMetadata = _fixture.Create<CommandMetadata>();
        _geefPersoonsService = new Mock<IGeefPersoonService>();
    }

    [Fact]
    public async ValueTask With_No_Vertegenwoordigers_Returns_Empty()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Vertegenwoordigers = [],
        };

        var personenUitKsz = await GeefPersonenMiddleware.BeforeAsync(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                                                                         command,
                                                                         _commandMetadata), _geefPersoonsService.Object, CancellationToken.None);

        personenUitKsz.Should().BeEquivalentTo(PersonenUitKsz.Empty);
    }

    [Fact]
    public async ValueTask With_PersonenUitKsz_Returns_PersonenUitKsz()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        var personenUitKsz = _fixture.CreateMany<PersoonUitKsz>().ToArray();

        _geefPersoonsService.Setup(x => x.GeefPersonen(command.Vertegenwoordigers, _commandMetadata, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new PersonenUitKsz(personenUitKsz));

        var result = await GeefPersonenMiddleware.BeforeAsync(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                                                                         command,
                                                                         _commandMetadata), _geefPersoonsService.Object, CancellationToken.None);

        result.Should().BeEquivalentTo(new PersonenUitKsz(personenUitKsz));
    }
}
