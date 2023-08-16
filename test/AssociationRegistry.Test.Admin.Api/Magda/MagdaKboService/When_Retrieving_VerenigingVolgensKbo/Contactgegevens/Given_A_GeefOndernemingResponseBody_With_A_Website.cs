﻿namespace AssociationRegistry.Test.Admin.Api.Magda.MagdaKboService.When_Retrieving_VerenigingVolgensKbo.Contactgegevens;

using AssociationRegistry.Admin.Api.Magda;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Kbo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_GeefOndernemingResponseBody_With_A_Website
{
    private readonly MagdaGeefVerenigingService _service;
    private readonly Fixture _fixture;
    private readonly string _website;

    public Given_A_GeefOndernemingResponseBody_With_A_Website()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaFacade>();
        var envelope = _fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();
        _website = _fixture.Create<string>();
        envelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Adressen = new[]
        {
            new AdresOndernemingType
            {
                Type = new TypeAdresOndernemingType
                {
                    Code = new CodeTypeAdresOndernemingType
                    {
                        Value = AdresCodes.MaatschappelijkeZetel,
                    },
                },
                Descripties = new []
                {
                    new DescriptieType
                    {
                        Contact = new ContactType
                        {
                            Website = _website,
                        },
                    },
                },
            },
        };


        magdaFacade.Setup(facade => facade.GeefOnderneming(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
            .ReturnsAsync(envelope);

        _service = new MagdaGeefVerenigingService(Mock.Of<IMagdaCallReferenceRepository>(), magdaFacade.Object, new NullLogger<MagdaGeefVerenigingService>());
    }

    [Fact]
    public async Task Then_It_Returns_A_SuccessResult()
    {
        var result = await _service.GeefVereniging(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(), CancellationToken.None);
        result.IsSuccess().Should().BeTrue();
    }

    [Fact]
    public async Task Then_It_Returns_A_Website()
    {
        var kboNummer = _fixture.Create<KboNummer>();
        var result = await _service.GeefVereniging(kboNummer, _fixture.Create<CommandMetadata>(), CancellationToken.None);
        using (new AssertionScope())
        {
            var verenigingVolgensKbo = result.Should().BeOfType<Result<VerenigingVolgensKbo>>().Subject.Data;
            verenigingVolgensKbo.Contactgegevens!.Website.Should().Be(_website);
        }
    }
}
