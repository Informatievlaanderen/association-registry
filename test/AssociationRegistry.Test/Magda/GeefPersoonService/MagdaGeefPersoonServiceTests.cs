namespace AssociationRegistry.Test.Magda.GeefPersoonService;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Integrations.Magda;
using Integrations.Magda.Persoon;
using Integrations.Magda.Shared.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Middleware;
using Moq;
using Resources;
using Xunit;

public class GeefPersoonServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IMagdaClient> _magdaClient;
    private readonly CommandMetadata _commandMetadata;

    public GeefPersoonServiceTests()
    {
        _magdaClient = new Mock<IMagdaClient>();
        _fixture = new Fixture().CustomizeAdminApi();
        _commandMetadata = _fixture.Create<CommandMetadata>();
    }

    [Fact]
    public async ValueTask With_At_Least_One_Overleden_Vertegenwoordigers_Returns_Overleden()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        SetUpInschrijvingen(command);
        SetUpFirstPersoonAsOverleden(command);

        var sut = new MagdaGeefPersoonService(_magdaClient.Object, NullLogger<MagdaGeefPersoonService>.Instance);
        var personenUitKsz = await sut.GeefPersonen(command.Vertegenwoordigers,
                                              _commandMetadata, CancellationToken.None);

        personenUitKsz.HeeftOverledenPersonen.Should().BeTrue();
    }

    [Fact]
    public async ValueTask With_NietOverleden_Vertegenwoordigers_Returns_NietOverleden()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        SetUpInschrijvingen(command);
        SetUpNietOverleden(command);

        var sut = new MagdaGeefPersoonService(_magdaClient.Object, NullLogger<MagdaGeefPersoonService>.Instance);
        var personenUitKsz = await sut.GeefPersonen(command.Vertegenwoordigers,
                                                    _commandMetadata, CancellationToken.None);

        personenUitKsz.HeeftOverledenPersonen.Should().BeFalse();
    }

    [Fact]
    public async ValueTask RegistreertInschrijving_Foreach_Vertegenwoordiger()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        SetUpInschrijvingen(command);
        SetUpNietOverleden(command);

        var sut = new MagdaGeefPersoonService(_magdaClient.Object, NullLogger<MagdaGeefPersoonService>.Instance);
        await sut.GeefPersonen(command.Vertegenwoordigers,
                                                    _commandMetadata, CancellationToken.None);

        command.Vertegenwoordigers
               .ToList()
               .ForEach(v =>
                            _magdaClient.Verify(x => x.RegistreerInschrijvingPersoon(v.Insz,
                                                                                     AanroependeFunctie.RegistreerVzer,
                                                                                     _commandMetadata,
                                                                                     It.IsAny<CancellationToken>()),
                                                Times.Once()));
    }

    [Theory]
    [InlineData("30002")]
    [InlineData("30003")]
    [InlineData("30004")]
    public async ValueTask MagdaGeefPersoonService_RegistreerInschrijvingPersoon_ThrowsDomainExceptionWhenUitzonderingDoorGebruiekr(
        string foutCode)
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();
        foreach (var vertegenwoordiger in command.Vertegenwoordigers)
        {
            _magdaClient.Setup(x => x.RegistreerInschrijvingPersoon(vertegenwoordiger.Insz, AanroependeFunctie.RegistreerVzer,
                                                                    _commandMetadata, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(MagdaTestResponseFactory.RegistreerInschrijvingPersoonResponses.NietGeslaagd(foutCode));
        }

        var sut = new MagdaGeefPersoonService(_magdaClient.Object, NullLogger<MagdaGeefPersoonService>.Instance);

        var exception = await Assert.ThrowsAsync<EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz>(async () =>
        {
            await sut.GeefPersonen(command.Vertegenwoordigers,
                                   _commandMetadata, CancellationToken.None);
        });

        exception.Message.Should().Be(ExceptionMessages.EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz);
    }

    [Fact]
    public async ValueTask MagdaGeefPersoonService_RegistreerInschrijvingPersoon_ThrowsExceptionWhenMagdaClientThrowsException()
    {
        var foutCode = _fixture.Create<string>();

        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();
        foreach (var vertegenwoordiger in command.Vertegenwoordigers)
        {
            _magdaClient.Setup(x => x.RegistreerInschrijvingPersoon(vertegenwoordiger.Insz, AanroependeFunctie.RegistreerVzer,
                                                                    _commandMetadata, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(MagdaTestResponseFactory.RegistreerInschrijvingPersoonResponses.NietGeslaagd(foutCode));
        }

        var sut = new MagdaGeefPersoonService(_magdaClient.Object, NullLogger<MagdaGeefPersoonService>.Instance);

        var magdaException = await Assert.ThrowsAsync<MagdaException>(async () =>
        {
            await sut.GeefPersonen(command.Vertegenwoordigers,
                                   _commandMetadata, CancellationToken.None);
        });

        magdaException.Message.Should().Be(ExceptionMessages.MagdaException);
    }


    private void SetUpInschrijvingen(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand command)
    {
        foreach (var vertegenwoordiger in command.Vertegenwoordigers)
        {
            _magdaClient.Setup(x => x.RegistreerInschrijvingPersoon(vertegenwoordiger.Insz, AanroependeFunctie.RegistreerVzer,
                                                                    _commandMetadata, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(MagdaTestResponseFactory.RegistreerInschrijvingPersoonResponses.WelGeslaagd);
        }
    }

    private void SetUpFirstPersoonAsOverleden(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand command)
    {
        // Setup first vertegenwoordiger as overleden
        var eersteVertegenwoordiger = command.Vertegenwoordigers.First();
        _magdaClient.Setup(s => s.GeefPersoon(eersteVertegenwoordiger.Insz,
                                              AanroependeFunctie.RegistreerVzer,
                                              _commandMetadata,
                                              It.IsAny<CancellationToken>()))
                    .ReturnsAsync(MagdaTestResponseFactory.GeefPersoonResponses.OverledenPersoon);

        // Setup rest as niet overleden
        command.Vertegenwoordigers
               .Skip(1)
               .ToList()
               .ForEach(v =>
                            _magdaClient.Setup(s => s.GeefPersoon(v.Insz,
                                                                  AanroependeFunctie.RegistreerVzer,
                                                                  _commandMetadata,
                                                                  It.IsAny<CancellationToken>()))
                                        .ReturnsAsync(MagdaTestResponseFactory.GeefPersoonResponses.NietOverledenPersoon));
    }

    private void SetUpNietOverleden(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand command)
    {
        command.Vertegenwoordigers
               .ToList()
               .ForEach(v =>
                            _magdaClient.Setup(s => s.GeefPersoon(v.Insz, AanroependeFunctie.RegistreerVzer, _commandMetadata,
                                                                  It.IsAny<CancellationToken>()))
                                        .ReturnsAsync(MagdaTestResponseFactory.GeefPersoonResponses.NietOverledenPersoon));
    }
}

