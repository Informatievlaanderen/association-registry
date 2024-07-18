namespace AssociationRegistry.Test.Admin.Api.Magda.When_GeefOnderneming.Service.Rechtsvorm;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_GeefOndernemingResponseBody_Which_Is_Not_A_Rechtspersoon
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;

    public Given_A_GeefOndernemingResponseBody_Which_Is_Not_A_Rechtspersoon()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var envelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        envelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.SoortOnderneming = new SoortOndernemingType
        {
            Code = new CodeSoortOndernemingType
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
