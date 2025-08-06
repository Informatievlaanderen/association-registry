namespace AssociationRegistry.Test.Magda.RegistreerInschrijvingService;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Exceptions;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.RegistreerInschrijving;
using AssociationRegistry.Magda.Repertorium.RegistreerInschrijving;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
using Xunit;

public class Given_Repliek_Uitzondering_Fout
{
    private readonly MagdaRegistreerInschrijvingService _service;
    private readonly Fixture _fixture;
    private readonly string _diagnose;

    public Given_Repliek_Uitzondering_Fout()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var magdaClient = new Mock<IMagdaClient>();
        _diagnose = _fixture.Create<string>();
        var responseEnvelope = CreateResponseEnvelope(_diagnose);


        magdaClient.Setup(facade => facade.RegistreerInschrijving(It.IsAny<string>(), It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(responseEnvelope);

        _service = new MagdaRegistreerInschrijvingService(Mock.Of<IMagdaCallReferenceRepository>(),
                                                          magdaClient.Object,
                                                          new NullLogger<MagdaRegistreerInschrijvingService>());
    }

    private ResponseEnvelope<RegistreerInschrijvingResponseBody> CreateResponseEnvelope(string diagnose)
    {
        var responseEnvelope = _fixture.Create<ResponseEnvelope<RegistreerInschrijvingResponseBody>>();

        responseEnvelope.Body!.RegistreerInschrijvingResponse!.Repliek.Uitzonderingen =
        [
            new UitzonderingType()
            {
                Type = UitzonderingTypeType.FOUT,
                Diagnose = diagnose,
            },
        ];

        return responseEnvelope;
    }

    [Fact]
    public async ValueTask Then_It_Throws_A_MagdaException()
    {
        var result = async () => await _service.RegistreerInschrijving(_fixture.Create<KboNummer>(), _fixture.Create<CommandMetadata>(),
                                                           CancellationToken.None);

        var exception = await Assert.ThrowsAsync<MagdaException>(result);
        exception.InnerException!.Should().BeOfType<MagdaRepliekException>();
        exception.InnerException!.Message.Should().Be($"De volgende fouten hebben zich voorgedaan bij het aanroepen van de Magda RegistreerInschrijvingDienst.{Environment.NewLine}{_diagnose}{Environment.NewLine}");
    }
}
