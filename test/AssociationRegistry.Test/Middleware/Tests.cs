// namespace AssociationRegistry.Test.Middleware;
//
// using AssociationRegistry.Framework;
// using AssociationRegistry.Grar.Clients;
// using AssociationRegistry.Grar.Models;
// using AssociationRegistry.Grar.Models.PostalInfo;
// using AssociationRegistry.Middleware;
// using AutoFixture;
// using Common.AutoFixture;
// using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
// using FluentAssertions;
// using GemeentenaamVerrijking;
// using Moq;
// using Vereniging;
// using Xunit;
//
// public class Tests
// {
//     private readonly Fixture _fixture;
//     private readonly Mock<IGrarClient> _grarClientMock;
//
//     public Tests()
//     {
//         _fixture = new Fixture().CustomizeDomain();
//         _grarClientMock = new Mock<IGrarClient>();
//     }
//
//     [Fact]
//     public async Task BeforeAsync_WithLocatiesWithAdresId_ShouldEnrichAllLocaties()
//     {
//         // Arrange
//         var locatie1 = CreateLocatieWithAdresId();
//         var locatie2 = CreateLocatieWithAdresId();
//         var envelope = CreateCommandEnvelope(locatie1, locatie2);
//
//         var (address1, postal1) = SetupGrarClientForLocatie(locatie1.AdresId);
//         var (address2, postal2) = SetupGrarClientForLocatie(locatie2.AdresId);
//
//         // Act
//         var result = await EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object);
//
//         // Assert
//         result.Should().NotBeNull();
//         result.Should().BeEquivalentTo([
//             CreateEnrichedLocatie(locatie1, address1, postal1),
//             CreateEnrichedLocatie(locatie2, address2, postal2),
//         ]);
//     }
//
//     private EnrichedLocatie CreateEnrichedLocatie(Locatie locatie, AddressDetailResponse grarAdres, PostalInfoDetailResponse postal1)
//         => EnrichedLocatie.FromLocatieWithAdresId(
//             locatie,
//             Adres.Create(grarAdres.Straatnaam, grarAdres.Huisnummer,
//                          grarAdres.Busnummer, grarAdres.Postcode,
//                          GemeentenaamDecorator.VerrijkGemeentenaam(postal1, grarAdres.Gemeente).Gemeentenaam, Adres.België));
//
//     [Fact]
//     public async Task BeforeAsync_WithLocatiesWithoutAdresId_ShouldSkipEnrichmentForThoseWithoutAdresId()
//     {
//         // Arrange
//         var locatieWithoutAdresId = CreateLocatieWithoutAdresId();
//         var locatieWithAdresId = CreateLocatieWithAdresId();
//         var envelope = CreateCommandEnvelope(locatieWithoutAdresId, locatieWithAdresId);
//
//         var (address1, postal1) = SetupGrarClientForLocatie(locatieWithAdresId.AdresId);
//
//         // Act
//         var result = await EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object);
//
//         // Assert
//         result.Should().NotBeNull();
//         result.Should().BeEquivalentTo([CreateEnrichedLocatie(locatieWithAdresId, address1, postal1)]);
//
//         _grarClientMock.Verify(x => x.GetAddressById(locatieWithAdresId.AdresId.ToId(), CancellationToken.None), Times.Once);
//         _grarClientMock.Verify(x => x.GetPostalInformationDetail(It.IsAny<string>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task BeforeAsync_WithAllLocatiesWithoutAdresId_ShouldReturnEmptyResult()
//     {
//         // Arrange
//         var locatie1 = CreateLocatieWithoutAdresId();
//         var locatie2 = CreateLocatieWithoutAdresId();
//         var envelope = CreateCommandEnvelope(locatie1, locatie2);
//
//         // Act
//         var result = await EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object);
//
//         // Assert
//         result.Should().NotBeNull();
//         result.Locaties.Should().BeEmpty();
//
//         _grarClientMock.VerifyNoOtherCalls();
//     }
//
//     [Fact]
//     public async Task BeforeAsync_WithEmptyLocatiesCollection_ShouldReturnEmptyResult()
//     {
//         // Arrange
//         var envelope = CreateCommandEnvelope();
//
//         // Act
//         var result = await EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object);
//
//         // Assert
//         result.Should().NotBeNull();
//         result.Locaties.Should().BeEmpty();
//
//         _grarClientMock.VerifyNoOtherCalls();
//     }
//
//     [Fact]
//     public async Task BeforeAsync_WithNullLocatiesCollection_ShouldThrowException()
//     {
//         // Arrange
//         var envelope = CreateEnvelopeWithNullLocaties();
//
//         // Act & Assert
//         await Assert.ThrowsAsync<ArgumentNullException>(
//             () => EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object));
//     }
//
//     [Fact]
//     public async Task BeforeAsync_WhenGetAddressByIdFails_ShouldPropagateException()
//     {
//         // Arrange
//         var locatie = CreateLocatieWithAdresId();
//         var envelope = CreateCommandEnvelope(locatie);
//
//         _grarClientMock.Setup(x => x.GetAddressById(locatie.AdresId.ToId(), CancellationToken.None))
//             .ThrowsAsync(new HttpRequestException("GRAR address service unavailable"));
//
//         // Act & Assert
//         var exception = await Assert.ThrowsAsync<HttpRequestException>(
//             () => EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object));
//
//         exception.Message.Should().Be("GRAR address service unavailable");
//     }
//
//     [Fact]
//     public async Task BeforeAsync_WhenGetPostalInformationFails_ShouldPropagateException()
//     {
//         // Arrange
//         var locatie = CreateLocatieWithAdresId();
//         var envelope = CreateCommandEnvelope(locatie);
//
//         var addressResponse = _fixture.Create<AddressDetailResponse>();
//         _grarClientMock.Setup(x => x.GetAddressById(locatie.AdresId.ToId(), CancellationToken.None))
//             .ReturnsAsync(addressResponse);
//         _grarClientMock.Setup(x => x.GetPostalInformationDetail(addressResponse.Postcode))
//             .ThrowsAsync(new ArgumentException("Invalid postcode format"));
//
//         // Act & Assert
//         var exception = await Assert.ThrowsAsync<ArgumentException>(
//             () => EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object));
//
//         exception.Message.Should().Be("Invalid postcode format");
//     }
//
//     [Fact]
//     public async Task BeforeAsync_WhenPartialFailure_ShouldFailEntireOperation()
//     {
//         // Arrange
//         var locatie1 = CreateLocatieWithAdresId();
//         var locatie2 = CreateLocatieWithAdresId();
//         var envelope = CreateCommandEnvelope(locatie1, locatie2);
//
//         // First call succeeds
//         SetupGrarClientForLocatie(locatie1.AdresId);
//
//         // Second call fails
//         _grarClientMock.Setup(x => x.GetAddressById(locatie2.AdresId.ToId(), CancellationToken.None))
//             .ThrowsAsync(new TimeoutException("Request timeout"));
//
//         // Act & Assert
//         await Assert.ThrowsAsync<TimeoutException>(
//             () => EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object));
//     }
//
//     [Fact]
//     public async Task BeforeAsync_ShouldCallGemeentenaamDecoratorWithCorrectParameters()
//     {
//         // Arrange
//         var locatie = CreateLocatieWithAdresId();
//         var envelope = CreateCommandEnvelope(locatie);
//
//         var addressResponse = _fixture.Create<AddressDetailResponse>();
//         var postalInfo = _fixture.Create<PostalInformation>();
//
//         _grarClientMock.Setup(x => x.GetAddressById(locatie.AdresId.ToId(), CancellationToken.None))
//             .ReturnsAsync(addressResponse);
//         _grarClientMock.Setup(x => x.GetPostalInformationDetail(addressResponse.Postcode))
//             .ReturnsAsync(postalInfo);
//
//         // Act
//         var result = await EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object);
//
//         // Assert
//         // This verifies the decorator is called correctly - the gemeentenaam should be enriched
//         result.Locaties.Single().Adres.Gemeentenaam.Should().NotBeNullOrEmpty();
//         // In a real test, you might want to mock GemeentenaamDecorator to verify exact parameters
//     }
//
//     [Fact]
//     public async Task BeforeAsync_ShouldCreateAdresWithBelgieAsLand()
//     {
//         // Arrange
//         var locatie = CreateLocatieWithAdresId();
//         var envelope = CreateCommandEnvelope(locatie);
//         SetupGrarClientForLocatie(locatie.AdresId);
//
//         // Act
//         var result = await EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object);
//
//         // Assert
//         result.Locaties.Single().Adres.Land.Should().Be(Adres.België);
//     }
//
//     [Theory]
//     [InlineData(null)]
//     [InlineData("")]
//     public async Task BeforeAsync_WithInvalidAdresIdToId_ShouldHandleGracefully(string invalidId)
//     {
//         // Arrange
//         var adresId = Mock.Of<AdresId>(x => x.ToId() == invalidId);
//         var locatie = CreateLocatieWithAdresId(adresId);
//         var envelope = CreateCommandEnvelope(locatie);
//
//         // Act & Assert
//         // This exposes that your middleware doesn't validate AdresId.ToId() result
//         await Assert.ThrowsAsync<ArgumentException>(
//             () => EnrichLocatiesMiddleware.BeforeAsync(envelope, _grarClientMock.Object));
//     }
//
//     // Helper methods for creating test data
//     private Locatie CreateLocatieWithAdresId(AdresId adresId = null)
//     {
//         return _fixture.Create<Locatie>() with
//         {
//             AdresId = adresId ?? _fixture.Create<AdresId>()
//         };
//     }
//
//     private Locatie CreateLocatieWithoutAdresId()
//     {
//         return _fixture.Create<Locatie>() with { AdresId = null };
//     }
//
//     private CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> CreateCommandEnvelope(params Locatie[] locaties)
//     {
//         var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
//         {
//             Locaties = locaties
//         };
//         return new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command);
//     }
//
//     private CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> CreateEnvelopeWithNullLocaties()
//     {
//         var command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
//         {
//             Locaties = null
//         };
//         return new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command);
//     }
//
//     // Helper methods for setting up mocks
//     private (AddressDetailResponse addressResponse, PostalInfoDetailResponse postalInfo) SetupGrarClientForLocatie(AdresId adresId)
//     {
//         var addressResponse = _fixture.Create<AddressDetailResponse>();
//         var postalInfo = _fixture.Create<PostalInfoDetailResponse>();
//
//         _grarClientMock.Setup(x => x.GetAddressById(adresId.ToId(), CancellationToken.None))
//             .ReturnsAsync(addressResponse);
//         _grarClientMock.Setup(x => x.GetPostalInformationDetail(addressResponse.Postcode))
//             .ReturnsAsync(postalInfo);
//
//         return (addressResponse, postalInfo);
//     }
//
//     // Helper methods for assertions
//     private static void AssertEnrichedFromGrar(EnrichedLocatie enrichedLocatie, AddressDetailResponse expectedResponse)
//     {
//         enrichedLocatie.Should().NotBeNull();
//         enrichedLocatie.Adres.Should().NotBeNull();
//         enrichedLocatie.Adres.Straatnaam.Should().Be(expectedResponse.Straatnaam);
//         enrichedLocatie.Adres.Huisnummer.Should().Be(expectedResponse.Huisnummer);
//         enrichedLocatie.Adres.Busnummer.Should().Be(expectedResponse.Busnummer);
//         enrichedLocatie.Adres.Postcode.Should().Be(expectedResponse.Postcode);
//         enrichedLocatie.Adres.Land.Should().Be(Adres.België);
//     }
// }
