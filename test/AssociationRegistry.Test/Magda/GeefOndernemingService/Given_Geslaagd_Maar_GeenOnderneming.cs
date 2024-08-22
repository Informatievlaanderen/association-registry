namespace AssociationRegistry.Test.Magda.GeefOndernemingService;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Geslaagd_Maar_GeenOnderneming
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;

    public Given_Geslaagd_Maar_GeenOnderneming()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var envelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        envelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.OndernemingOfVestiging =
            new OndernemingOfVestigingType
            {
                Code = new CodeOndernemingOfVestigingType
                {
                    Value = _fixture.Create<string>(),
                },
            };

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(envelope);

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object,
                                                  new TemporaryMagdaVertegenwoordigersSection(),
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async Task Then_It_Returns_A_FailureResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

        result.IsFailure().Should().BeTrue();
    }
}
