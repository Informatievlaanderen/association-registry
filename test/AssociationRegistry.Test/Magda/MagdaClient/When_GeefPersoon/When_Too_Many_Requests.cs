namespace AssociationRegistry.Test.Magda.MagdaClient.When_GeefPersoon;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.Configuration;
using FluentAssertions;
using Hosts.Configuration;
using Integrations.Magda;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Shared.Exceptions;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Resources;
using Xunit;

public class When_Too_Many_Requests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task Then_It_Returns_ResponsePersoonsBody()
    {
        var insz = "toomanyrequests";
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;
        var magdaClient = MagdaClientTestSetup.CreateMagdaClient(_fixture, commandMetadata, insz);
        var magdaException = await Assert.ThrowsAsync<MagdaException>(() => magdaClient.GeefPersoon(insz, aanroependeFunctie, commandMetadata,
                                                                          CancellationToken.None));

        magdaException.Message.Should().Be(string.Format(ExceptionMessages.MagdaException, "GeefPersoonDienst",
                                               "99995 - Te veel gelijktijdige bevragingen"));
    }
}
