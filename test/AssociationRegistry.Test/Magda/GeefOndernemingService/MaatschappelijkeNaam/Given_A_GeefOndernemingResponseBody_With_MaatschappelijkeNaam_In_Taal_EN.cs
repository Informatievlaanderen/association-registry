namespace AssociationRegistry.Test.Magda.GeefOndernemingService.MaatschappelijkeNaam;

using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Kbo;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_GeefOndernemingResponseBody_With_MaatschappelijkeNaam_In_Taal_EN
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;
    private readonly string _verenigingNaam;

    public Given_A_GeefOndernemingResponseBody_With_MaatschappelijkeNaam_In_Taal_EN()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var responseEnvelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        _verenigingNaam = _fixture.Create<string>();

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Namen.MaatschappelijkeNamen = new[]
        {
            new NaamOndernemingType
            {
                Naam = _verenigingNaam,
                Taalcode = "en",
            },
            new NaamOndernemingType
            {
                Naam = _fixture.Create<string>(),
                Taalcode = _fixture.Create<string>(),
            },
        };

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(responseEnvelope);

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object,
                                                  new TemporaryMagdaVertegenwoordigersSection(),
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async Task Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

        result.IsSuccess().Should().BeTrue();
    }

    [Fact]
    public async Task Then_It_Returns_A_VerenigingVolgensKbo()
    {
        var kboNummer = _fixture.Create<KboNummer>();
        var result = await _service.GeefVereniging(kboNummer, _fixture.Create<CommandMetadata>(), CancellationToken.None);

        using (new AssertionScope())
        {
            var verenigingVolgensKbo = result.Should().BeOfType<Result<VerenigingVolgensKbo>>().Subject.Data;
            verenigingVolgensKbo.KboNummer.Should().BeEquivalentTo(kboNummer);
            verenigingVolgensKbo.Naam.Should().Be(_verenigingNaam);
        }
    }
}
