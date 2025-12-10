namespace AssociationRegistry.Test.Magda.MagdaClient.When_RegistreerInschrijvingPersoon;

using AssociationRegistry.Framework;
using Hosts.Configuration;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Magda.Kbo;
using Common.Configuration;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using ResultaatEnumType = Integrations.Magda.Repertorium.RegistreerInschrijving0200.ResultaatEnumType;

public class Given_A_Valid_Insz
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task Then_It_Returns_RegistreerInschrijvingResponseBody()
    {
        var insz = "01234567889";
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;
        var magdaClient = MagdaClientTestSetup.CreateMagdaClient(_fixture, commandMetadata, insz);

        var response = await magdaClient.RegistreerInschrijvingPersoon(insz, aanroependeFunctie, commandMetadata, CancellationToken.None);

        using (new AssertionScope())
        {
            var repliek = response?.Body?.RegistreerInschrijvingResponse?.Repliek;
            repliek?.Uitzonderingen.Should().BeNullOrEmpty();

            var resultaat = repliek?.Antwoorden.Antwoord.Inhoud.Resultaat;

            resultaat.Should().NotBeNull();
            resultaat.Value.Should().BeOneOf(ResultaatEnumType.Item0, ResultaatEnumType.Item1);
        }
    }
}
