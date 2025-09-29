namespace AssociationRegistry.Test.Magda.GeefOndernemingService.Rechtsvorm;

using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Constants;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Integrations.Magda.Models.GeefOnderneming;
using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;

using Xunit;

public class Given_A_GeefOndernemingResponseBody_With_Rechtsvorm_With_Unknown_Ending
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;

    public Given_A_GeefOndernemingResponseBody_With_Rechtsvorm_With_Unknown_Ending()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var responseEnvelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Rechtsvormen = new[]
        {
            new RechtsvormExtentieType
            {
                Code = new CodeRechtsvormType
                {
                    Value = RechtsvormCodes.VZW,
                },
                DatumEinde = null,
            },
        };

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(responseEnvelope);

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object,
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

        result.IsSuccess().Should().BeTrue();
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_VerenigingVolgensKbo()
    {
        var kboNummer = _fixture.Create<KboNummer>();
        var result = await _service.GeefVereniging(kboNummer, _fixture.Create<CommandMetadata>(), CancellationToken.None);

        using (new AssertionScope())
        {
            var verenigingVolgensKbo = result.Should().BeOfType<Result<VerenigingVolgensKbo>>().Subject.Data;
            verenigingVolgensKbo.KboNummer.Should().BeEquivalentTo(kboNummer);
            verenigingVolgensKbo.Type.Should().Be(Verenigingstype.VZW);
        }
    }
}
