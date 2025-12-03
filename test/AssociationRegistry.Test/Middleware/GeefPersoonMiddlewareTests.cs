namespace AssociationRegistry.Test.Middleware;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using CommandHandling.DecentraalBeheer.Middleware;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Integrations.Magda;
using Moq;
using Xunit;

public class GeefPersoonMiddlewareTests
{
    private readonly Fixture _fixture;
    private readonly CommandMetadata _commandMetadata;
    private readonly Mock<IMagdaClient> _magdaClient;

    public GeefPersoonMiddlewareTests()
    {
        _fixture = new Fixture().CustomizeDomain();
        _commandMetadata = _fixture.Create<CommandMetadata>();
        _magdaClient = new Mock<IMagdaClient>();
    }

    [Fact]
    public async ValueTask With_No_Vertegenwoordigers_Returns_Empty()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Vertegenwoordigers = [],
        };

        var personenUitKsz = await GeefPersoonMiddleware.BeforeAsync(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                                                                         command,
                                                                         _commandMetadata), _magdaClient.Object, CancellationToken.None);

        personenUitKsz.Should().BeEquivalentTo(PersonenUitKsz.Empty);
    }
    [Fact]
    public async ValueTask With_At_Least_One_Overleden_Vertegenwoordigers_Returns_Overleden()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        // Setup first vertegenwoordiger as overleden
        var eersteVertegenwoordiger = command.Vertegenwoordigers.First();
        _magdaClient.Setup(s => s.GeefPersoon(eersteVertegenwoordiger.Insz,
                                              AanroependeFunctie.RegistreerVzer,
                                              _commandMetadata,
                                              It.IsAny<CancellationToken>()))
                    .ReturnsAsync(MagdaTestResponseFactory.OverledenPersoon);

        // Setup rest as niet overleden
        command.Vertegenwoordigers
               .Skip(1)
               .ToList()
               .ForEach(v =>
                            _magdaClient.Setup(s => s.GeefPersoon(v.Insz,
                                                                  AanroependeFunctie.RegistreerVzer,
                                                                  _commandMetadata,
                                                                  It.IsAny<CancellationToken>()))
                                        .ReturnsAsync(MagdaTestResponseFactory.NietOverledenPersoon));

        var personenUitKsz = await GeefPersoonMiddleware.BeforeAsync(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                                                                         command,
                                                                         _commandMetadata), _magdaClient.Object, CancellationToken.None);

        personenUitKsz.HeeftOverledenPersonen.Should().BeTrue();
    }

    [Fact]
    public async ValueTask With_NietOverleden_Vertegenwoordigers_Returns_NietOverleden()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        command.Vertegenwoordigers
               .ToList()
               .ForEach(v =>
                            _magdaClient.Setup(s => s.GeefPersoon(v.Insz, AanroependeFunctie.RegistreerVzer, _commandMetadata,
                                                                  It.IsAny<CancellationToken>()))
                                        .ReturnsAsync(MagdaTestResponseFactory.NietOverledenPersoon));

        var personenUitKsz = await GeefPersoonMiddleware.BeforeAsync(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                                                                         command,
                                                                         _commandMetadata), _magdaClient.Object, CancellationToken.None);

        personenUitKsz.HeeftOverledenPersonen.Should().BeFalse();
    }

    [Fact]
    public async ValueTask RegistreertInschrijving_Foreach_Vertegenwoordiger()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        command.Vertegenwoordigers
               .ToList()
               .ForEach(v =>
                            _magdaClient.Setup(s => s.GeefPersoon(v.Insz, AanroependeFunctie.RegistreerVzer, _commandMetadata,
                                                                  It.IsAny<CancellationToken>()))
                                        .ReturnsAsync(MagdaTestResponseFactory.NietOverledenPersoon));

        _ = await GeefPersoonMiddleware.BeforeAsync(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                                                        command,
                                                        _commandMetadata), _magdaClient.Object, CancellationToken.None);

        command.Vertegenwoordigers
               .ToList()
               .ForEach(v =>
                            _magdaClient.Verify(x => x.RegistreerInschrijvingPersoon(v.Insz,
                                                                                     AanroependeFunctie.RegistreerVzer,
                                                                                     _commandMetadata,
                                                                                     It.IsAny<CancellationToken>()),
                                                Times.Once()));
    }
}
