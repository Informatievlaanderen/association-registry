namespace AssociationRegistry.Test.Magda.GeefOndernemingService.Bankrekeningnummers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Onderneming;
using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using AssociationRegistry.Integrations.Magda.Onderneming.Models.GeefOnderneming;
using AssociationRegistry.Integrations.Magda.Shared.Models;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Xunit;

public class Given_Bankrekeningnummers
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;
    private ResponseEnvelope<GeefOndernemingResponseBody>? _envelope;

    public Given_Bankrekeningnummers()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        _envelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        _envelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Bankrekeningen =
        [
            _fixture.Create<BankrekeningType>(),
        ];

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(_envelope);


        _service = new MagdaGeefVerenigingService(magdaFacade.Object,
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,_fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

        result.IsSuccess().Should().BeTrue();
    }

    [Fact]
    public async ValueTask Then_It_Returns_Bankrekeningnummers()
    {
        var kboNummer = _fixture.Create<KboNummer>();
        var result = await _service.GeefVereniging(kboNummer, AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid,_fixture.Create<CommandMetadata>(), CancellationToken.None);

        using (new AssertionScope())
        {
            var verenigingVolgensKbo = result.Should().BeOfType<Result<VerenigingVolgensKbo>>().Subject.Data;
            verenigingVolgensKbo.Bankrekeningnummers.Should().NotBeNullOrEmpty();

            verenigingVolgensKbo.Bankrekeningnummers.Should().BeEquivalentTo(_envelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden
                                                                                     .Antwoord.Inhoud.Onderneming.Bankrekeningen.Select(
                                                                                          x => new BankrekeningnummerVolgensKbo()
                                                                                          {
                                                                                              Iban = x.IBAN,
                                                                                          }).ToArray());
        }
    }
}
