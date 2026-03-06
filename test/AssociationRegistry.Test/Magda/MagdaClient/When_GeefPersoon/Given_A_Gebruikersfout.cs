namespace AssociationRegistry.Test.Magda.MagdaClient.When_GeefPersoon;

using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Test.Common.Configuration;
using AutoFixture;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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
    public async Task With_Gebruikersfout_Then_Throws_EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz(string insz)
    {
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;
        var magdaClient = MagdaClientTestSetup.CreateMagdaClient(_fixture, commandMetadata, insz);

        var exception = await Assert.ThrowsAsync<EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz>(
            () => magdaClient.GeefPersoon(insz,
                                                aanroependeFunctie,
                                                commandMetadata,
                                                CancellationToken.None));

        exception.Message.Should().Be(ExceptionMessages.EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz);
    }
}
