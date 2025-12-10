namespace AssociationRegistry.Test.Magda.MagdaClient.When_RegistreerInschrijvingPersoon;

using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda.Shared.Exceptions;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Resources;
using AutoFixture;
using FluentAssertions;
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
        var magdaException = await Assert.ThrowsAsync<MagdaException>(() => magdaClient.RegistreerInschrijvingPersoon(insz, aanroependeFunctie, commandMetadata,
                                                                          CancellationToken.None));

        magdaException.Message.Should().Be(string.Format(ExceptionMessages.MagdaException, "RegistreerInschrijving0200Dienst",
                                               "99995 - Te veel gelijktijdige bevragingen"));
    }
}
