namespace AssociationRegistry.Test.Magda.GeefPersoonService;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Magda.Persoon;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Common.AutoFixture;
using FluentAssertions;
using Integrations.Magda;
using Integrations.Magda.Persoon;
using Integrations.Magda.Persoon.Models;
using Integrations.Magda.Persoon.Models.RegistreerInschrijving0200;
using Integrations.Magda.Persoon.Validation;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Middleware;
using Moq;
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

        var sut = new MagdaGeefPersoonService(_magdaClient.Object,
                                              Mock.Of<IMagdaRegistreerInschrijvingValidator>(),
                                              Mock.Of<IMagdaGeefPersoonValidator>(),
                                              NullLogger<MagdaGeefPersoonService>.Instance);
        var personenUitKsz = await sut.GeefPersonen(command.Vertegenwoordigers.Select(GeefPersoonRequest.From).ToArray(),
                                                    _commandMetadata, CancellationToken.None);

        personenUitKsz.HeeftOverledenPersonen.Should().BeTrue();
    }

    [Fact]
    public async ValueTask With_NietOverleden_Vertegenwoordigers_Returns_NietOverleden()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        SetUpInschrijvingen(command);
        SetUpNietOverleden(command);

        var sut = new MagdaGeefPersoonService(_magdaClient.Object,
                                              Mock.Of<IMagdaRegistreerInschrijvingValidator>(),
                                              Mock.Of<IMagdaGeefPersoonValidator>(),
                                              NullLogger<MagdaGeefPersoonService>.Instance);
        var personenUitKsz = await sut.GeefPersonen(command.Vertegenwoordigers.Select(GeefPersoonRequest.From).ToArray(),
                                                    _commandMetadata, CancellationToken.None);

        personenUitKsz.HeeftOverledenPersonen.Should().BeFalse();
    }

    [Fact]
    public async ValueTask RegistreertInschrijving_Foreach_Vertegenwoordiger()
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        SetUpInschrijvingen(command);
        SetUpNietOverleden(command);

        var sut = new MagdaGeefPersoonService(_magdaClient.Object,
                                              Mock.Of<IMagdaRegistreerInschrijvingValidator>(),
                                              Mock.Of<IMagdaGeefPersoonValidator>(),
                                              NullLogger<MagdaGeefPersoonService>.Instance);

        await sut.GeefPersonen(command.Vertegenwoordigers.Select(GeefPersoonRequest.From).ToArray(),
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

    private List<ResponseEnvelope<RegistreerInschrijvingResponseBody>> SetUpInschrijvingen(
        RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand command)
    {
        var responses = new List<ResponseEnvelope<RegistreerInschrijvingResponseBody>>();

        foreach (var vertegenwoordiger in command.Vertegenwoordigers)
        {
            var responseEnvelope = MagdaTestResponseFactory.RegistreerInschrijvingPersoon.WelGeslaagd;

            _magdaClient
               .Setup(x => x.RegistreerInschrijvingPersoon(
                          vertegenwoordiger.Insz,
                          AanroependeFunctie.RegistreerVzer,
                          _commandMetadata,
                          It.IsAny<CancellationToken>()))
               .ReturnsAsync(responseEnvelope);

            responses.Add(responseEnvelope);
        }

        return responses;
    }


    private List<ResponseEnvelope<GeefPersoonResponseBody>> SetUpNietOverleden(
        RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand command)
    {
        var responses = new List<ResponseEnvelope<GeefPersoonResponseBody>>();

        foreach (var v in command.Vertegenwoordigers)
        {
            var nietOverledenPersoon = MagdaTestResponseFactory.GeefPersoonResponses.NietOverledenPersoon;

            _magdaClient
               .Setup(s => s.GeefPersoon(
                          v.Insz,
                          AanroependeFunctie.RegistreerVzer,
                          _commandMetadata,
                          It.IsAny<CancellationToken>()))
               .ReturnsAsync(nietOverledenPersoon);

            responses.Add(nietOverledenPersoon);
        }

        return responses;
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
}

