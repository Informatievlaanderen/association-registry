﻿namespace AssociationRegistry.Test.Magda.GeefOndernemingService.Status;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Constants;
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

public class Given_A_GeefOndernemingResponseBody_Which_Is_In_Oprichting
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;

    public Given_A_GeefOndernemingResponseBody_Which_Is_In_Oprichting()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var responseEnvelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.StatusKBO = new StatusKBOType
        {
            Code = new CodeStatusKBOType
            {
                Value = StatusKBOCodes.InOprichting,
            },
        };

        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(responseEnvelope);

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object,
                                                  new TemporaryMagdaVertegenwoordigersSection(),
                                                  new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async ValueTask Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                   CancellationToken.None);

        result.IsSuccess().Should().BeTrue();
    }
}
