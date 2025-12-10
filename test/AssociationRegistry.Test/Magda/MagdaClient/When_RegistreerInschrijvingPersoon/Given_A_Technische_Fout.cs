namespace AssociationRegistry.Test.Magda.MagdaClient.When_RegistreerInschrijvingPersoon;

using AssociationRegistry.Framework;
using Hosts.Configuration;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Magda.Kbo;
using Common.Configuration;
using AutoFixture;
using FluentAssertions;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Shared.Exceptions;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Resources;
using Xunit;

public class Given_A_Technische_Fout
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task Then_It_Returns_RegistreerInschrijvingResponseBody()
    {
        var insz = "09876543210"; // see wiremock folder
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;
        var magdaClient = MagdaClientTestSetup.CreateMagdaClient(_fixture, commandMetadata, insz);

        var magdaException = await Assert.ThrowsAsync<MagdaException>(() => magdaClient.RegistreerInschrijvingPersoon(insz,
                                                                          aanroependeFunctie,
                                                                          commandMetadata,
                                                                          CancellationToken.None));



        magdaException.Message.Should().Be("Magda fouten: Er heeft zich een technisch probleem voorgedaan");
    }
}
