namespace AssociationRegistry.Test.Admin.Api.Magda.When_GeefOnderneming.Service.KorteNaam;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using AutoFixture;
using FluentAssertions;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Kbo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_GeefOndernemingResponseBody_With_AfgekorteNaam_Null
{
    private readonly Result<VerenigingVolgensKbo> _verenigingVolgensKbo;

    public Given_A_GeefOndernemingResponseBody_With_AfgekorteNaam_Null()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var magdaFacade = new Mock<IMagdaClient>();
        var responseEnvelope = fixture.Create<ResponseEnvelope<GeefOndernemingResponseBody>>();

        responseEnvelope.Body!.GeefOndernemingResponse!.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming.Namen.AfgekorteNamen = new[]
        {
            new NaamOndernemingType
            {
                Naam = null,
                Taalcode = "nl",
            },
        };

        magdaFacade.Setup(facade =>
                              facade.GeefOnderneming(
                                  It.IsAny<string>(),
                                  It.IsAny<MagdaCallReference>()))
                   .ReturnsAsync(responseEnvelope);

        var service =
            new MagdaGeefVerenigingService(
                Mock.Of<IMagdaCallReferenceRepository>(),
                magdaFacade.Object,
                new TemporaryMagdaVertegenwoordigersSection(),
                new NullLogger<MagdaGeefVerenigingService>());

        _verenigingVolgensKbo =
            service.GeefVereniging(
                        fixture.Create<KboNummer>(),
                        fixture.Create<CommandMetadata>(),
                        CancellationToken.None)
                   .GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_It_Returns_A_VerenigingVolgensKbo_With_Empty_String_For_KorteNaam()
    {
        _verenigingVolgensKbo.Data.KorteNaam.Should().BeEmpty();
        _verenigingVolgensKbo.Data.KorteNaam.Should().NotBeNull();
    }
}
