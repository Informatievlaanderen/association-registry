using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace AssociationRegistry.Test.Middleware;

using CommandHandling.DecentraalBeheer.Acties.DubbelDetectie;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using CommandHandling.DecentraalBeheer.Middleware;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.DubbelDetectie;
using Wolverine;

public class DuplicateDetectionMiddlewareTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IDuplicateVerenigingDetectionService> _duplicateServiceMock;
    private readonly ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> _logger;
    private readonly VerrijkteAdressenUitGrar _verrijkteAdressen;
    private readonly Mock<IRapporteerDubbeleVerenigingenService> _rapporteerService;
    private readonly CancellationToken _cancellationToken;

    public DuplicateDetectionMiddlewareTests()
    {
        _fixture = new Fixture().CustomizeDomain();
        _duplicateServiceMock = new Mock<IDuplicateVerenigingDetectionService>();
        _logger = NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance;
        _verrijkteAdressen = _fixture.Create<VerrijkteAdressenUitGrar>();
        _rapporteerService = new Mock<IRapporteerDubbeleVerenigingenService>();
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task WithValidBevestigingstoken_ShouldReturnSkip()
    {
        // Arrange
        var envelope = CreateCommand(skipDuplicateDetection: true, _fixture.Create<string>());
        var bevestigingstokenHelper = WithValidBevestigingstoken(envelope);

        // Act
        var result = await DuplicateDetectionMiddleware.BeforeAsync(
            envelope, _verrijkteAdressen, _duplicateServiceMock.Object, _rapporteerService.Object, bevestigingstokenHelper.Object, _logger, _cancellationToken);

        // Assert
        result.Should().Be(PotentialDuplicatesFound.Skip(envelope.Command.Bevestigingstoken));
    }

    [Fact]
    public async Task WithValidBevestigingstoken_ShouldNotUseDuplicateDetectionService()
    {
        // Arrange
        var envelope = CreateCommand(skipDuplicateDetection: true, _fixture.Create<string>());
        var bevestigingstokenHelper = WithValidBevestigingstoken(envelope);

        // Act
        await DuplicateDetectionMiddleware.BeforeAsync(
            envelope, _verrijkteAdressen, _duplicateServiceMock.Object, _rapporteerService.Object, bevestigingstokenHelper.Object, _logger, _cancellationToken);

        // Assert
        _duplicateServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WithoutBevestigingstoken_AndNoDuplicatesFound_ShouldReturnNone()
    {
        // Arrange
        var envelope = CreateCommand(skipDuplicateDetection: false);
        SetupDuplicateServiceReturnsNoDuplicates();
        var bevestigingstokenHelper = Mock.Of<IBevestigingsTokenHelper>();

        // Act
        var result = await DuplicateDetectionMiddleware.BeforeAsync(
            envelope, _verrijkteAdressen, _duplicateServiceMock.Object, _rapporteerService.Object, bevestigingstokenHelper, _logger, _cancellationToken);

        // Assert
        result.Should().Be(PotentialDuplicatesFound.None);
        VerifyDuplicateServiceCalledWithCorrectParameters(envelope);

        _rapporteerService.Verify(x => x.RapporteerAsync(
                               It.IsAny<CommandEnvelope<RapporteerDubbeleVerenigingenMessage>>(),
                               It.IsAny<CancellationToken>()
                           ), Times.Never);
    }

    [Fact]
    public async Task WhenSkipDuplicateDetectionIsFalseAndDuplicatesFound_ShouldReturnSome_And_Send_RapporteerDubbeleVerenigingenCommand()
    {
        // Arrange
        var envelope = CreateCommand(skipDuplicateDetection: false);
        var duplicates = _fixture.CreateMany<DuplicaatVereniging>(3).ToArray();
        SetupDuplicateServiceReturnsDuplicates(duplicates);
        var (bevestigingstokenHelper, bevestigingstoken) = RandomBevestigingstokenHelper(envelope);

        // Act
        var result = await DuplicateDetectionMiddleware.BeforeAsync(
            envelope, _verrijkteAdressen, _duplicateServiceMock.Object, _rapporteerService.Object, bevestigingstokenHelper.Object, _logger, _cancellationToken);

        // Assert
        result.Should().NotBe(PotentialDuplicatesFound.None);
        result.PotentialDuplicates.Should().BeEquivalentTo(duplicates);
        VerifyDuplicateServiceCalledWithCorrectParameters(envelope);

        _rapporteerService.Verify(x => x.RapporteerAsync(
                                      It.Is<CommandEnvelope<RapporteerDubbeleVerenigingenMessage>>(c =>
                                                  c.Command.Bevestigingstoken == bevestigingstoken
                                               && c.Command.Naam == envelope.Command.Naam
                                               && c.Command.Locaties.SequenceEqual(envelope.Command.Locaties)
                                               && c.Command.GedetecteerdeDubbels.SequenceEqual(duplicates)
                                               && c.Metadata.Initiator == WellknownOvoNumbers.DigitaalVlaanderenOvoNumber
                                      ),
                                      It.IsAny<CancellationToken>()
                                  ), Times.Once);
    }

    [Fact]
    public async Task WithNoLocations_ShouldReturnNone()
    {
        var envelope = CreateCommand(skipDuplicateDetection: false);
        envelope = envelope with
        {
            Command = envelope.Command with
            {
                Locaties = [],
            },
        };

        var bevestigingstokenHelper = Mock.Of<IBevestigingsTokenHelper>();

        var duplicates = _fixture.CreateMany<DuplicaatVereniging>(3).ToArray();
        SetupDuplicateServiceReturnsDuplicates(duplicates);

        var result = await DuplicateDetectionMiddleware.BeforeAsync(
            envelope, VerrijkteAdressenUitGrar.Empty, _duplicateServiceMock.Object, _rapporteerService.Object, bevestigingstokenHelper, _logger, _cancellationToken);

        result.Should().Be(PotentialDuplicatesFound.None);
    }

    [Fact]
    public async Task ShouldPassEnrichedLocatiesToDuplicateService()
    {
        // Arrange
        var locatieWithAdresId = _fixture.Create<Locatie>() with
        {
            AdresId = _fixture.Create<AdresId>(),
            Adres = null
        };
        var enrichedAdres = _fixture.Create<Adres>();

        var envelope = CreateCommandWithSpecificLocaties(locatieWithAdresId);
        var verrijkteAdressen = new VerrijkteAdressenUitGrar(
            new Dictionary<string, Adres> { { locatieWithAdresId.AdresId.Bronwaarde, enrichedAdres } });

        SetupDuplicateServiceReturnsNoDuplicates();

        var bevestigingstokenHelper = Mock.Of<IBevestigingsTokenHelper>();

        // Act
        await DuplicateDetectionMiddleware.BeforeAsync(
            envelope, verrijkteAdressen, _duplicateServiceMock.Object, _rapporteerService.Object, bevestigingstokenHelper, _logger, _cancellationToken);

        // Assert
        _duplicateServiceMock.Verify(
            x => x.ExecuteAsync(
                envelope.Command.Naam,
                It.Is<DuplicateVerenigingZoekQueryLocaties>(query =>
                                                                query.Postcodes.Contains(enrichedAdres.Postcode) &&
                                                                query.Gemeentes.Contains(enrichedAdres.Gemeente.Naam)),
                false,
                It.IsAny<MinimumScore>()),
            Times.Once);
    }

    // Helper methods for creating test data

    private CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> CreateCommand(
        bool skipDuplicateDetection,
        string bevestigingstoken = "")
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Bevestigingstoken = bevestigingstoken
        };
        return new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, _fixture.Create<CommandMetadata>());
    }

    private CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> CreateCommandWithSpecificLocaties(params Locatie[] locaties)
    {
        var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Locaties = locaties
        };
        return new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, _fixture.Create<CommandMetadata>());
    }

    // Helper methods for setting up mocks - using It.IsAny for complex object matching

    private void SetupDuplicateServiceReturnsNoDuplicates()
    {
        _duplicateServiceMock.Setup(x => x.ExecuteAsync(
                                        It.IsAny<VerenigingsNaam>(),
                                        It.IsAny<DuplicateVerenigingZoekQueryLocaties>(),
                                        It.IsAny<bool>(),
                                        It.IsAny<MinimumScore>()))
                             .ReturnsAsync([]);
    }

    private void SetupDuplicateServiceReturnsDuplicates(DuplicaatVereniging[] duplicates)
    {
        _duplicateServiceMock.Setup(x => x.ExecuteAsync(
                                        It.IsAny<VerenigingsNaam>(),
                                        It.IsAny<DuplicateVerenigingZoekQueryLocaties>(),
                                        It.IsAny<bool>(),
                                        It.IsAny<MinimumScore>()))
                             .ReturnsAsync(duplicates);
    }

    // Helper methods for verification - focusing on the important parameters

    private void VerifyDuplicateServiceCalledWithCorrectParameters(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope)
    {
        _duplicateServiceMock.Verify(
            x => x.ExecuteAsync(
                envelope.Command.Naam,
                It.IsAny<DuplicateVerenigingZoekQueryLocaties>(),
                false, // skipGemeenteCheck parameter
                It.IsAny<MinimumScore>()),
            Times.Once);
    }

    // Alternative verification approach - capture the actual query for inspection

    private void VerifyDuplicateServiceCalledAndCaptureQuery(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
        out DuplicateVerenigingZoekQueryLocaties capturedQuery)
    {
        DuplicateVerenigingZoekQueryLocaties captured = null;

        _duplicateServiceMock.Verify(
            x => x.ExecuteAsync(
                envelope.Command.Naam,
                It.Is<DuplicateVerenigingZoekQueryLocaties>(query => CaptureQuery(query, out captured)),
                false,
                It.IsAny<MinimumScore>()),
            Times.Once);

        capturedQuery = captured;
    }

    private static bool CaptureQuery(DuplicateVerenigingZoekQueryLocaties query, out DuplicateVerenigingZoekQueryLocaties captured)
    {
        captured = query;
        return true;
    }

    private static Mock<IBevestigingsTokenHelper> WithValidBevestigingstoken(CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope)
    {
        var bevestigingstokenHelper = new Mock<IBevestigingsTokenHelper>();

        bevestigingstokenHelper.Setup(x => x.IsValid(envelope.Command.Bevestigingstoken, envelope.Command.OriginalRequest))
                               .Returns(true);

        return bevestigingstokenHelper;
    }

    private static (Mock<IBevestigingsTokenHelper>, string) RandomBevestigingstokenHelper(CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope)
    {
        var bevestigingstokenHelper = new Mock<IBevestigingsTokenHelper>();

        var bevestigingstoken = Guid.NewGuid().ToString();

        bevestigingstokenHelper.Setup(x => x.Calculate(envelope.Command.OriginalRequest))
                               .Returns(bevestigingstoken);

        return (bevestigingstokenHelper, bevestigingstoken);
    }

    private static (Mock<IBevestigingsTokenHelper>, string) AlwaysValidBevestigingstokenHelperWithDuplicatesFound(CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope)
    {
        var bevestigingstokenHelper = new Mock<IBevestigingsTokenHelper>();

        bevestigingstokenHelper.Setup(x => x.IsValid(envelope.Command.Bevestigingstoken, envelope.Command.OriginalRequest))
                               .Returns(true);

        var bevestigingstoken = Guid.NewGuid().ToString();

        bevestigingstokenHelper.Setup(x => x.Calculate(envelope.Command.OriginalRequest))
                               .Returns(bevestigingstoken);

        return (bevestigingstokenHelper, bevestigingstoken);
    }

    private static Mock<IBevestigingsTokenHelper> AlwaysInvalidBevestigingstokenHelperWithNoDuplicatesFound(CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope)
    {
        var bevestigingstokenHelper = new Mock<IBevestigingsTokenHelper>();

        bevestigingstokenHelper.Setup(x => x.IsValid(envelope.Command.Bevestigingstoken, envelope.Command.OriginalRequest))
                               .Returns(false);

        var bevestigingstoken = Guid.NewGuid().ToString();

        bevestigingstokenHelper.Setup(x => x.Calculate(envelope.Command.OriginalRequest))
                               .Returns(bevestigingstoken);

        return bevestigingstokenHelper;
    }
}
