namespace AssociationRegistry.Test.Magda.MagdaClient.When_RegistreerInschrijvingPersoon;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_A_Gebruikersfout
{
    private readonly Fixture _fixture = new();

    [Theory]
    [InlineData("0725459040")]
    [InlineData("0725459041")]
    [InlineData("0725459042")]
    [InlineData("0725459043")]
    [InlineData("0725459044")]
    public async Task Then_It_Returns_RegistreerInschrijvingResponseBody(string insz)
    {
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;
        var magdaClient = MagdaClientTestSetup.CreateMagdaClient(_fixture, commandMetadata, insz);

        var exception = await Assert.ThrowsAsync<EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz>(() => magdaClient.RegistreerInschrijvingPersoon(insz,
                                                                     aanroependeFunctie,
                                                                     commandMetadata,
                                                                     CancellationToken.None));

        exception.Message.Should().Be(ExceptionMessages.EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz);
    }

}
